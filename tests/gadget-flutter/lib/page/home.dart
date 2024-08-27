import 'dart:io';
import 'package:device_info_plus/device_info_plus.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:openid_client/openid_client_io.dart';
import 'package:url_launcher/url_launcher.dart';
import 'dart:async';
import 'package:http/http.dart' as http;
import 'dart:convert';

class HomePage extends StatefulWidget {
  const HomePage({super.key});

  @override
  _HomePageState createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  final String _clientId = '7001D8C1-9FD4-45F6-B8F5-91B827B8576D';
  final String _applicationId = '2DE4423E-F36B-1410-81D6-007EB43F7292';
  static const String _issuer =
      'https://ahs-test01-idp-sea-wa.azurewebsites.net';
  final List<String> _scopes = <String>[
    'alarm-data',
    'client-data',
    'user-data'
  ];
  String logoutUrl = '';

  @override
  void initState() {
    super.initState();

    FirebaseMessaging.onMessage.listen((RemoteMessage message) {
      print('Got a message whilst in the foreground!');
      print('Message data: ${message.data}');

      if (message.notification != null) {
        print('Message also contained a notification: ${message.notification}');
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(_isLogin ? "Home" : "Welcome back!"),
      ),
      body: Center(
        child: Container(
          padding: const EdgeInsets.all(10.0),
          child: Scrollbar(
            child: SingleChildScrollView(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.center,
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      mainAxisAlignment: MainAxisAlignment.start,
                      children: [
                        Text.rich(
                          TextSpan(
                            text: 'Home Tenant Id: ', // default text style
                            children: <TextSpan>[
                              TextSpan(
                                  text: _homeTenantId,
                                  style: const TextStyle(
                                    fontWeight: FontWeight.bold,
                                  ))
                            ],
                          ),
                        ),
                        Text.rich(
                          TextSpan(
                            text:
                                'Home Subscription Id: ', // default text style
                            children: <TextSpan>[
                              TextSpan(
                                  text: _homeSubscriptionId,
                                  style: const TextStyle(
                                      fontWeight: FontWeight.bold))
                            ],
                          ),
                        ),
                        Text.rich(
                          TextSpan(
                            text: 'User Id: ', // default text style
                            children: <TextSpan>[
                              TextSpan(
                                  text: _userId,
                                  style: const TextStyle(
                                      fontWeight: FontWeight.bold))
                            ],
                          ),
                        ),
                        Text.rich(
                          TextSpan(
                            text: 'UPN: ', // default text style
                            children: <TextSpan>[
                              TextSpan(
                                  text: _upn,
                                  style: const TextStyle(
                                      fontWeight: FontWeight.bold))
                            ],
                          ),
                        ),
                        Text.rich(
                          TextSpan(
                            text:
                                'Subscription Access Token: ', // default text style
                            children: <TextSpan>[
                              TextSpan(
                                  text: _subscriptionAccessToken,
                                  style: const TextStyle(
                                      fontWeight: FontWeight.bold))
                            ],
                          ),
                        ),
                        Text.rich(
                          TextSpan(
                            text: 'Gadgte Id: ', // default text style
                            children: <TextSpan>[
                              TextSpan(
                                  text: _gadgetId,
                                  style: const TextStyle(
                                      fontWeight: FontWeight.bold))
                            ],
                          ),
                        ),
                        Text.rich(
                          TextSpan(
                            text: 'Gadget Name: ', // default text style
                            children: <TextSpan>[
                              TextSpan(
                                  text: _gadgetName,
                                  style: const TextStyle(
                                      fontWeight: FontWeight.bold))
                            ],
                          ),
                        ),
                        Text.rich(
                          TextSpan(
                            text: 'Gadget Status: ', // default text style
                            children: <TextSpan>[
                              TextSpan(
                                  text: _gadgetStatus,
                                  style: const TextStyle(
                                      fontWeight: FontWeight.bold))
                            ],
                          ),
                        ),
                      ]),
                  if (!_isLogin)
                    ElevatedButton(
                      child: const Text("Login"),
                      onPressed: () async {
                        tokenResponse = await authenticate(
                            Uri.parse(_issuer), _clientId, _scopes);
                        print(tokenResponse?.accessToken);
                      },
                    ),
                  if (_isLogin)
                    Container(
                      margin: const EdgeInsets.only(top: 10.0),
                      child: ElevatedButton(
                          child: const Text("Logout"),
                          onPressed: () async {
                            logout();
                          }),
                    ),
                  if (_isLogin)
                    ElevatedButton(
                        child: const Text("Get User Info"),
                        onPressed: () async {
                          getUserInfo();
                        }),
                  if (_isLogin)
                    ElevatedButton(
                        child: const Text("Get Subscription Access Token"),
                        onPressed: () async {
                          getSubscriptionToken();
                        }),
                  if (_isLogin)
                    ElevatedButton(
                        child: const Text("Register/Re-register Gadget"),
                        onPressed: () async {
                          registerGadget();
                        }),
                  if (_isLogin)
                    ElevatedButton(
                        child: const Text("Unregister Gadget"),
                        onPressed: () async {
                          unregisterGadget();
                        }),
                  if (_isLogin)
                    ElevatedButton(
                        child: const Text("Store Push Token"),
                        onPressed: () async {
                          storePushToken();
                        }),
                  if (_isLogin)
                    ElevatedButton(
                        child: const Text("Acknowledge"),
                        onPressed: () async {
                          acknowledge();
                        })
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }

  var _isLogin = false;
  var _userId = '';
  var _upn = '';
  var _homeTenantId = '';
  var _homeSubscriptionId = '';
  var _subscriptionAccessToken = '';
  var _gadgetId = '';
  var _gadgetName = '';
  var _gadgetStatus = '';
  TokenResponse? tokenResponse;

  Future<TokenResponse?> authenticate(
      Uri uri, String clientId, List<String> scopes) async {
    // create the client
    var issuer = await Issuer.discover(uri);
    var client = Client(issuer, clientId);

    // create a function to open a browser with an url
    urlLauncher(String url) async {
      if (await canLaunchUrl(Uri.parse(url))) {
        await launchUrl(Uri.parse(url), mode: LaunchMode.inAppWebView);
      } else {
        throw 'Could not launch $url';
      }
    }

    // create an authenticator
    var authenticator = Authenticator(
      client,
      scopes: scopes,
      urlLancher: urlLauncher,
      port: 4000,
    );

    try {
      authenticator.flow.scopes.remove('email');
      // starts the authentication
      var credential = await authenticator.authorize();
      // close the webview when finished
      closeInAppWebView();

      var res = await credential.getTokenResponse();

      setState(() {
        logoutUrl = credential.generateLogoutUrl().toString();

        if (res.accessToken != null) {
          _isLogin = true;
        }
      });

      return res;
    } on Exception catch (exception) {
      // only executed if error is of type Exception
      print(exception);
    } catch (error) {
      // executed for errors of all types other than Exception
      print(error);
    }

    return null;
  }

  Future<void> getUserInfo() async {
    var url =
        'https://ahs-test01-ppm-be-sea-wa.azurewebsites.net/usr/users/info';
    var accessToken = tokenResponse?.accessToken;

    var response = await http
        .get(Uri.parse(url), headers: {"Authorization": "Bearer $accessToken"});
    final body = json.decode(response.body);

    setState(() {
      _userId = body['id'];
      _upn = body['upn'];
      _homeTenantId = body['homeTenantId'];
      _homeSubscriptionId = body['homeSubscriptionId'];
    });
  }

  Future<void> getSubscriptionToken() async {
    var url = 'https://ahs-test01-idp-sea-wa.azurewebsites.net/connect/token';
    Map<String, dynamic> body = {
      'grant_type': 'user_switch_tenant',
      'client_id': _clientId,
      'tenantId': _homeTenantId,
      'subscriptionId': _homeSubscriptionId,
      'applicationId': _applicationId,
      'token': tokenResponse?.accessToken
    };

    var response = await http.post(Uri.parse(url),
        headers: {"content-type": "application/x-www-form-urlencoded"},
        body: body);

    final responseBody = json.decode(response.body);

    setState(() {
      _subscriptionAccessToken = responseBody['access_token'];
    });
  }

  static Future<List<String>> getDeviceDetails() async {
    // https://pub.dev/packages/device_info_plus/example
    String deviceName = '';
    String deviceVersion = '';
    String identifier = '';
    final DeviceInfoPlugin deviceInfoPlugin = DeviceInfoPlugin();

    try {
      if (Platform.isAndroid) {
        var build = await deviceInfoPlugin.androidInfo;
        deviceName = build.model;
        deviceVersion = build.version.toString();
        identifier = build.serialNumber; //UUID for Android
      } else if (Platform.isIOS) {
        var data = await deviceInfoPlugin.iosInfo;
        deviceName = data.name;
        deviceVersion = data.systemVersion;
        identifier = data.identifierForVendor ?? ''; //UUID for iOS
      }
    } on PlatformException {
      print('Failed to get platform version');
    }

    return [deviceName, deviceVersion, identifier];
  }

  Future<void> registerGadget() async {
    var url = 'https://ahs-test01-ppm-be-sea-wa.azurewebsites.net/usr/gadgets';
    var deviceInfo = await getDeviceDetails();
    Map<String, dynamic> body = {
      'userId': _userId,
      'name': '${deviceInfo[0]}_${deviceInfo[2]}'
    };
    String jsonBody = json.encode(body);

    var response = await http.post(Uri.parse(url),
        headers: {
          "Authorization": "Bearer $_subscriptionAccessToken",
          "content-type": "application/json; charset=utf-8"
        },
        body: jsonBody);

    final responseBody = json.decode(response.body);

    setState(() {
      _gadgetId = responseBody['id'];
      _gadgetName = responseBody['name'];
      _gadgetStatus = responseBody['status'] ? 'Active' : 'Inactive';
    });
  }

  Future<void> unregisterGadget() async {
    var url =
        'https://ahs-test01-ppm-be-sea-wa.azurewebsites.net/usr/gadgets/unregister';
    var deviceInfo = await getDeviceDetails();
    Map<String, dynamic> body = {
      'userId': _userId,
      'name': '${deviceInfo[0]}_${deviceInfo[2]}'
    };
    String jsonBody = json.encode(body);

    var response = await http.patch(Uri.parse(url),
        headers: {
          "Authorization": "Bearer $_subscriptionAccessToken",
          "content-type": "application/json; charset=utf-8"
        },
        body: jsonBody);

    final responseBody = json.decode(response.body);

    setState(() {
      _gadgetId = responseBody['id'];
      _gadgetName = responseBody['name'];
      _gadgetStatus = responseBody['status'] ? 'Active' : 'Inactive';
    });
  }

  Future<void> storePushToken() async {
    final fcmToken = await FirebaseMessaging.instance.getToken();
    await FirebaseMessaging.instance.setAutoInitEnabled(true);
    print("FCMToken $fcmToken");

    var url =
        'https://ahs-test01-ppm-be-sea-wa.azurewebsites.net/usr/gadgets/store-push-service-token';
    var deviceInfo = await getDeviceDetails();
    Map<String, dynamic> body = {
      'userId': _userId,
      'name': '${deviceInfo[0]}_${deviceInfo[2]}',
      "pushServiceToken": fcmToken
    };
    String jsonBody = json.encode(body);

    var response = await http.patch(Uri.parse(url),
        headers: {
          "Authorization": "Bearer $_subscriptionAccessToken",
          "content-type": "application/json; charset=utf-8"
        },
        body: jsonBody);

    print('response ${response}');

    FirebaseMessaging.instance.onTokenRefresh.listen((fcmToken) {
      // TODO: If necessary send token to application server.

      // Note: This callback is fired at each app startup and whenever a new
      // token is generated.
    }).onError((err) {
      // Error getting token.
    });
  }

  Future<void> acknowledge() async {
    var url =
        'https://ahs-test01-ppm-be-sea-wa.azurewebsites.net/alm/alarms/acknowledge';

    Map<String, dynamic> body = {
      'ids': ["b21e0a6b-f3e6-400c-b0d9-6a50a35f8691"],
      'comment': '',
    };
    String jsonBody = json.encode(body);

    var response = await http.put(Uri.parse(url),
        headers: {
          "Authorization": "Bearer $_subscriptionAccessToken",
          "content-type": "application/json; charset=utf-8"
        },
        body: jsonBody);

    print("response ${response}");
  }

  Future<void> logout() async {
    if (await canLaunchUrl(Uri.parse(logoutUrl))) {
      await launchUrl(Uri.parse(logoutUrl), mode: LaunchMode.inAppWebView);
    } else {
      throw 'Could not launch $logoutUrl';
    }

    await Future.delayed(Duration(seconds: 3));
    closeInAppWebView();

    setState(() {
      _isLogin = false;
      _userId = '...';
      _upn = '...';
      _homeTenantId = '...';
      _homeSubscriptionId = '...';
      _subscriptionAccessToken = '...';
      _gadgetId = '...';
      _gadgetName = '...';
      _gadgetStatus = '...';
    });
  }
}
