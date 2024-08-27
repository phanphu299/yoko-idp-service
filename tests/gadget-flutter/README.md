# Gadget (Alarm Management)

Updating Firebase Cloud Messaging (FCM) in Flutter via the command line involves a few steps. Here's a general guide to help you update FCM in your Flutter project:

1. Ensure Firebase CLI is Installed: Before proceeding, make sure you have the Firebase CLI installed on your system. If not, you can install it by following the instructions on the Firebase CLI documentation https://www.freecodecamp.org/news/set-up-firebase-cloud-messaging-in-flutter/.
2. Login to Firebase: Use the Firebase CLI to log in to your Firebase account with the following command:

firebase logout
firebase login

3. Configure Firebase in Flutter: Use the flutterfire configure command to set up Firebase services in your Flutter project. This will help you integrate FCM and other Firebase services:

flutterfire configure

4. Replace current package name with new package name from FCM. For example: com.yokogawa.alarm.management

Reference: https://www.freecodecamp.org/news/set-up-firebase-cloud-messaging-in-flutter/
