DECLARE @name NVARCHAR(255) = 'Force reset password Template';
DECLARE @subject NVARCHAR(255) = 'OpreX Asset Health Insights - Password Reset';
DECLARE @type_code VARCHAR(25) = 'FORCERESETPW';
DECLARE @type_code_name VARCHAR(250) = 'Force reset password email'
DECLARE @html VARCHAR(MAX) = '
<!DOCTYPE HTML PUBLIC "-//W3C//DTD XHTML 1.0 Transitional //EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xmlns:v="urn:schemas-microsoft-com:vml" xmlns:o="urn:schemas-microsoft-com:office:office">
<head>
  <!--[if gte mso 9]>
  <xml>
    <o:OfficeDocumentSettings>
      <o:AllowPNG/>
      <o:PixelsPerInch>96</o:PixelsPerInch>
    </o:OfficeDocumentSettings>
  </xml>
  <![endif]-->
  <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <meta name="x-apple-disable-message-reformatting" />
  <!--[if !mso]><!--><meta http-equiv="X-UA-Compatible" content="IE=edge" /><!--<![endif]-->
  <title></title>
  <style type="text/css">
    @media only screen and (min-width: 920px) {
      .u-row {
        width: 900px !important;
      }
      .u-row .u-col {
        vertical-align: top;
      }

      .u-row .u-col-31p62 {
        width: 284.58px !important;
      }

      .u-row .u-col-68p38 {
        width: 615.42px !important;
      }

      .u-row .u-col-100 {
        width: 900px !important;
      }

    }

    @media (max-width: 920px) {
      .u-row-container {
        max-width: 100% !important;
        padding-left: 0px !important;
        padding-right: 0px !important;
      }
      .u-row .u-col {
        min-width: 320px !important;
        max-width: 100% !important;
        display: block !important;
      }
      .u-row {
        width: 100% !important;
      }
      .u-col {
        width: 100% !important;
      }
      .u-col > div {
        margin: 0 auto;
      }
    }
    body {
      margin: 0;
      padding: 0;
    }

    table,
    tr,
    td {
      vertical-align: top;
      border-collapse: collapse;
    }

    p {
      margin: 0;
    }

    .ie-container table,
    .mso-container table {
      table-layout: fixed;
    }

    * {
      line-height: inherit;
    }

    a[x-apple-data-detectors="true"] {
      color: inherit !important;
      text-decoration: none !important;
    }

    table, td { color: #000000; } #u_body a { color: #0000fd; text-decoration: underline; }
  </style>
  <!--[if !mso]><!--><link href="https://fonts.googleapis.com/css?family=Open+Sans:400,700&display=swap" rel="stylesheet" type="text/css"/><!--<![endif]-->

</head>

<body class="clean-body u_body" style="margin: 0;padding: 0;-webkit-text-size-adjust: 100%;background-color: #e7e7e7;color: #000000">
  <!--[if IE]><div class="ie-container"><![endif]-->
  <!--[if mso]><div class="mso-container"><![endif]-->
  <table id="u_body" style="border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;min-width: 320px;Margin: 0 auto;background-color: #e7e7e7;width:100%" cellpadding="0" cellspacing="0">
  <tbody>
  <tr style="vertical-align: top">
    <td style="word-break: break-word;border-collapse: collapse !important;vertical-align: top">
      <!--[if (mso)|(IE)]><table width="100%" cellpadding="0" cellspacing="0" border="0"><tr><td align="center" style="background-color: #e7e7e7;"><![endif]-->
    
      <div class="u-row-container" style="padding: 0px;background-color: transparent">
        <div class="u-row" style="Margin: 0 auto;min-width: 320px;max-width: 900px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;">
          <div style="border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;">
            <!--[if (mso)|(IE)]><table width="100%" cellpadding="0" cellspacing="0" border="0"><tr><td style="padding: 0px;background-color: transparent;" align="center"><table cellpadding="0" cellspacing="0" border="0" style="width:900px;"><tr style="background-color: transparent;"><![endif]-->      
            <!--[if (mso)|(IE)]><td align="center" width="900" style="width: 900px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;" valign="top"><![endif]-->
            <div class="u-col u-col-100" style="max-width: 320px;min-width: 900px;display: table-cell;vertical-align: top;">
              <div style="height: 100%;width: 100% !important;">
                <!--[if (!mso)&(!IE)]><!--><div style="height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"><!--<![endif]-->
                <table style="font-family:Noto Sans,sans-serif;" role="presentation" cellpadding="0" cellspacing="0" width="100%" border="0">
                  <tbody>
                    <tr>
                      <td style="overflow-wrap:break-word;word-break:break-word;padding:0px;font-family:Noto Sans,sans-serif;" align="left">
                        <table width="100%" cellpadding="0" cellspacing="0" border="0">
                          <tr>
                            <td style="padding-right: 0px;padding-left: 0px;" align="center">
                              <img align="center" border="0" src="cid:opreX_email_header.png" alt="" title="" style="outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 100%;max-width: 900px;" width="900"/>
                            </td>
                          </tr>
                        </table>
                      </td>
                    </tr>
                  </tbody>
                </table>
                <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
              </div>
            </div>
            <!--[if (mso)|(IE)]></td><![endif]-->
            <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
          </div>
        </div>
      </div>

      <div class="u-row-container" style="padding: 0px;background-color: transparent">
        <div class="u-row" style="Margin: 0 auto;min-width: 320px;max-width: 900px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;">
          <div style="border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;">
            <!--[if (mso)|(IE)]><table width="100%" cellpadding="0" cellspacing="0" border="0"><tr><td style="padding: 0px;background-color: transparent;" align="center"><table cellpadding="0" cellspacing="0" border="0" style="width:900px;"><tr style="background-color: transparent;"><![endif]-->
            <!--[if (mso)|(IE)]><td align="center" width="900" style="background-color: #ffffff;width: 900px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;" valign="top"><![endif]-->
            <div class="u-col u-col-100" style="max-width: 320px;min-width: 900px;display: table-cell;vertical-align: top;">
              <div style="background-color: #ffffff;height: 100%;width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;">
                <!--[if (!mso)&(!IE)]><!--><div style="height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;"><!--<![endif]-->
                <table style="font-family:Noto Sans,sans-serif;" role="presentation" cellpadding="0" cellspacing="0" width="100%" border="0">
                  <tbody>
                    <tr>
                      <td style="overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:Noto Sans,sans-serif;" align="left">
                        <div style="line-height: 150%; text-align: left; word-wrap: break-word;">
                          <p style="font-size: 14px; line-height: 150%;">Hi {{Email}},</p>
                          <p style="font-size: 14px; line-height: 150%;"></p>
                          <p style="font-size: 14px; line-height: 150%;">You have recently been asked to reset your password. No changes have been made to your account yet.</p>
                          <p style="font-size: 14px; line-height: 150%;">Please click on this button to reset your password.</p>
                        </div>
                      </td>
                    </tr>
                  </tbody>
                </table>
                <table style="font-family:Noto Sans,sans-serif;" role="presentation" cellpadding="0" cellspacing="0" width="100%" border="0">
                  <tbody>
                    <tr>
                      <td style="overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:Noto Sans,sans-serif;" align="left">  
                        <!--[if mso]><style>.v-button {background: transparent !important;}</style><![endif]-->
                        <div align="center">
                          <!--[if mso]><v:roundrect xmlns:v="urn:schemas-microsoft-com:vml" xmlns:w="urn:schemas-microsoft-com:office:word" href="{{AccessUrl}}" style="height:37px; v-text-anchor:middle; width:177px;" arcsize="11%"  stroke="f" fillcolor="#00316c"><w:anchorlock/><center style="color:#FFFFFF;font-family:Noto Sans,sans-serif;"><![endif]-->  
                            <a href="{{AccessUrl}}" target="_blank" class="v-button" style="box-sizing: border-box;display: inline-block;font-family:Noto Sans,sans-serif;text-decoration: none;-webkit-text-size-adjust: none;text-align: center;color: #FFFFFF; background-color: #00316c; border-radius: 4px;-webkit-border-radius: 4px; -moz-border-radius: 4px; width:auto; max-width:100%; overflow-wrap: break-word; word-break: break-word; word-wrap:break-word; mso-border-alt: none;">
                              <span style="display:block;padding:10px 20px;line-height:120%;"><span style="font-size: 14px; line-height: 16.8px;">SET NEW PASSWORD</span></span>
                            </a>
                          <!--[if mso]></center></v:roundrect><![endif]-->
                        </div>
                      </td>
                    </tr>
                  </tbody>
                </table>
                <table style="font-family:Noto Sans,sans-serif;" role="presentation" cellpadding="0" cellspacing="0" width="100%" border="0">
                  <tbody>
                    <tr>
                      <td style="overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:Noto Sans,sans-serif;" align="left">      
                        <div style="line-height: 150%; text-align: left; word-wrap: break-word;">
                          <p style="font-size: 14px; line-height: 150%;">Or copy and paste below Web URL to access OpreX Asset Health Insight application</p>
                          <p style="font-size: 14px; line-height: 150%;"><a rel="noopener" href="{{AccessUrl}}" target="_blank"><em>{{AccessUrl}}</em></a></p>
                        </div>
                      </td>
                    </tr>
                  </tbody>
                </table>
                <table style="font-family:Noto Sans,sans-serif;" role="presentation" cellpadding="0" cellspacing="0" width="100%" border="0">
                  <tbody>
                    <tr>
                      <td style="overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:Noto Sans,sans-serif;" align="left">      
                        <div style="line-height: 150%; text-align: left; word-wrap: break-word;">
                          <p style="font-size: 14px; line-height: 150%;">If you did not request a password reset, you can safely ignore this email.</p>
                        </div>
                      </td>
                    </tr>
                  </tbody>
                </table>
                <table style="font-family:Noto Sans,sans-serif;" role="presentation" cellpadding="0" cellspacing="0" width="100%" border="0">
                  <tbody>
                    <tr>
                      <td style="overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:Noto Sans,sans-serif;" align="left">
                        <div style="line-height: 150%; text-align: left; word-wrap: break-word;">
                          <p style="font-size: 14px; line-height: 150%;">Best Regards,</p>
                          <p style="font-size: 14px; line-height: 150%;"></p>
                          <p style="font-size: 14px; line-height: 150%;">OpreX Asset Health Insight team</p>
                          <p style="font-size: 14px; line-height: 150%;"><a rel="noopener" href="mailto:support@ahi.apps.yokogawa.com" target="_blank">support@ahi.apps.yokogawa.com</a></p>
                        </div>
                      </td>
                    </tr>
                  </tbody>
                </table>

              <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
              </div>
            </div>
            <!--[if (mso)|(IE)]></td><![endif]-->
            <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
          </div>
        </div>
      </div>

      <div class="u-row-container" style="padding: 0px;background-color: transparent">
        <div class="u-row" style="Margin: 0 auto;min-width: 320px;max-width: 900px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;">
          <div style="border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;">
            <!--[if (mso)|(IE)]><table width="100%" cellpadding="0" cellspacing="0" border="0"><tr><td style="padding: 0px;background-color: transparent;" align="center"><table cellpadding="0" cellspacing="0" border="0" style="width:900px;"><tr style="background-color: transparent;"><![endif]-->
            <!--[if (mso)|(IE)]><td align="center" width="615" style="background-color: #00316c;width: 615px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;" valign="top"><![endif]-->
            <div class="u-col u-col-68p38" style="max-width: 320px;min-width: 615.42px;display: table-cell;vertical-align: top;">
              <div style="background-color: #00316c;height: 100%;width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;">
                <!--[if (!mso)&(!IE)]><!--><div style="height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;"><!--<![endif]-->
                <table style="font-family:Noto Sans,sans-serif;" role="presentation" cellpadding="0" cellspacing="0" width="100%" border="0">
                  <tbody>
                    <tr>
                      <td style="overflow-wrap:break-word;word-break:break-word;padding:15px 5px;font-family:Noto Sans,sans-serif;" align="left">
                        <div style="line-height: 140%; text-align: left; word-wrap: break-word;">
                          <p style="font-size: 14px; line-height: 140%;"><span style="font-size: 14px; line-height: 19.6px;"><strong><span style="color: #ecf0f1; line-height: 19.6px; font-size: 14px;">Yokogawa Electronic International Pte. Ltd.</span></strong></span></p>
                        </div>
                      </td>
                    </tr>
                  </tbody>
                </table>
                <table style="font-family:Noto Sans,sans-serif;" role="presentation" cellpadding="0" cellspacing="0" width="100%" border="0">
                  <tbody>
                    <tr>
                      <td style="overflow-wrap:break-word;word-break:break-word;padding:5px 5px 25px;font-family:Noto Sans,sans-serif;" align="left">      
                        <div style="line-height: 140%; text-align: left; word-wrap: break-word;">
                          <p style="font-size: 14px; line-height: 140%;"><span style="font-size: 14px; line-height: 19.6px; color: #ecf0f1;">Address: 5 Bedok South Road, Singapore 469270</span></p>
                          <p style="font-size: 14px; line-height: 140%;"><span style="font-size: 14px; line-height: 19.6px; color: #ecf0f1;">Tel: (65) 6241 9933</span></p>
                          <p style="font-size: 14px; line-height: 140%;"><span style="color: #ecf0f1; font-size: 14px; line-height: 19.6px;">Email: <span style="font-size: 14px; line-height: 19.6px;"><a rel="noopener" href="mailto:support@ahi.apps.yokogawa.com" target="_blank" style="color: #ecf0f1; text-decoration: underline;">support@ahi.apps.yokogawa.com</a></span> | URL: <a rel="noopener" href="https://www.yokogawa.com" target="_blank" style="color: #ecf0f1;">www.Yokogawa.com</a></span></p>
                          <p style="font-size: 14px; line-height: 140%;"><span style="color: #ecf0f1; font-size: 14px; line-height: 19.6px;">Website: <a rel="noopener" href="https://www.yokogawa.com/sg/solutions/featured-topics/digital-transformation/yokogawa-cloud/asset-health-insights/" target="_blank" style="color: #ecf0f1;">OpreX Asset Health Insights</a></span></p>
                        </div>
                      </td>
                    </tr>
                  </tbody>
                </table>
                <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
              </div>
            </div>
            <!--[if (mso)|(IE)]></td><![endif]-->
            <!--[if (mso)|(IE)]><td align="center" width="284" style="background-color: #00316c;width: 284px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;" valign="top"><![endif]-->
            <div class="u-col u-col-31p62" style="max-width: 320px;min-width: 284.58px;display: table-cell;vertical-align: top;">
              <div style="background-color: #00316c;height: 100%;width: 100% !important;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;">
                <!--[if (!mso)&(!IE)]><!--><div style="height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;border-radius: 0px;-webkit-border-radius: 0px; -moz-border-radius: 0px;"><!--<![endif]-->
                <table style="font-family:Noto Sans,sans-serif;" role="presentation" cellpadding="0" cellspacing="0" width="100%" border="0">
                  <tbody>
                    <tr>
                      <td style="overflow-wrap:break-word;word-break:break-word;padding:5px 25px 0px 0px;font-family:Noto Sans,sans-serif;" align="left">        
                        <table width="100%" cellpadding="0" cellspacing="0" border="0">
                          <tr>
                            <td style="padding-right: 0px;padding-left: 0px;" align="left">
                              <img align="left" border="0" src="cid:opreX_platform_logo.png" alt="" title="" style="outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 75%;max-width: 194.69px;" width="194.69"/>
                            </td>
                          </tr>
                        </table>
                      </td>
                    </tr>
                  </tbody>
                </table>
                <table style="font-family:Noto Sans,sans-serif;" role="presentation" cellpadding="0" cellspacing="0" width="100%" border="0">
                  <tbody>
                    <tr>
                      <td style="overflow-wrap:break-word;word-break:break-word;padding:0px 35px 35px 0px;font-family:Noto Sans,sans-serif;" align="left">    
                        <table width="100%" cellpadding="0" cellspacing="0" border="0">
                          <tr>
                            <td style="padding-right: 0px;padding-left: 0px;" align="left">
                              <img align="left" border="0" src="cid:company_logo.png" alt="" title="" style="outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 75%;max-width: 187.19px;" width="187.19"/>
                            </td>
                          </tr>
                        </table>
                      </td>
                    </tr>
                  </tbody>
                </table>
                <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
              </div>
            </div>
            <!--[if (mso)|(IE)]></td><![endif]-->
            <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
          </div>
        </div>
      </div>

      <!--[if (mso)|(IE)]></td></tr></table><![endif]-->
    </td>
  </tr>
  </tbody>
  </table>
  <!--[if mso]></div><![endif]-->
  <!--[if IE]></div><![endif]-->
</body>

</html>

';

IF NOT EXISTS(SELECT 1 FROM [dbo].[email_templates] WITH (NOLOCK) WHERE [type_code] = @type_code)
BEGIN
    INSERT INTO [dbo].[email_templates] ([name],[subject],[html],[type_code],[deleted])
    VALUES (@name, @subject, @html, @type_code, 0)
END
ELSE
BEGIN
    UPDATE [dbo].[email_templates]
    SET [name] = @name, [subject] = @subject, [html] = @html
    WHERE [type_code] = @type_code
END

DECLARE @template_id UNIQUEIDENTIFIER;
DECLARE @disposition NVARCHAR(25) = 'inline';
DECLARE @attachment_file_path VARCHAR(MAX);

SELECT @template_id = id 
FROM dbo.email_templates 
WHERE deleted = 0 AND type_code = @type_code

SET @attachment_file_path = 'https://cdn.ahi.apps.yokogawa.com/images/email_templates/opreX_email_header.png'
IF NOT EXISTS (SELECT 1 FROM email_attachments WHERE email_template_id = @template_id AND file_path = @attachment_file_path)
BEGIN
    INSERT INTO email_attachments(email_template_id, disposition, file_path)
    VALUES (@template_id, @disposition, @attachment_file_path)
END

SET @attachment_file_path = 'https://cdn.ahi.apps.yokogawa.com/images/email_templates/opreX_platform_logo.png'
IF NOT EXISTS (SELECT 1 FROM email_attachments WHERE email_template_id = @template_id AND file_path = @attachment_file_path)
BEGIN
    INSERT INTO email_attachments(email_template_id, disposition, file_path)
    VALUES (@template_id, @disposition, @attachment_file_path)
END

SET @attachment_file_path = 'https://cdn.ahi.apps.yokogawa.com/images/email_templates/company_logo.png'
IF NOT EXISTS (SELECT 1 FROM email_attachments WHERE email_template_id = @template_id AND file_path = @attachment_file_path)
BEGIN
    INSERT INTO email_attachments(email_template_id, disposition, file_path)
    VALUES (@template_id, @disposition, @attachment_file_path)
END