DECLARE @name NVARCHAR(255) = 'Generic Template';
DECLARE @subject NVARCHAR(255) = 'Your account has been locked';
DECLARE @type_code VARCHAR(25) = 'ACCOUNTLOCKED';
DECLARE @type_code_name varchar(250) = 'Account locked notification email'
DECLARE @html NVARCHAR(MAX) = '<!DOCTYPE html>
                        <html>
                        <head>

                        <meta charset="utf-8">
                        <meta http-equiv="x-ua-compatible" content="ie=edge">
                        <title>Your account has been locked</title>
                        <meta name="viewport" content="width=device-width, initial-scale=1">
                        <style type="text/css">

                        @@media screen {
                            @@font-face {
                            font-family: "Source Sans Pro";
                            font-style: normal;
                            font-weight: 400;
                            src: local("Source Sans Pro Regular"), local("SourceSansPro-Regular"), url(https://fonts.gstatic.com/s/sourcesanspro/v10/ODelI1aHBYDBqgeIAH2zlBM0YzuT7MdOe03otPbuUS0.woff) format("woff");
                            }

                            @@font-face {
                            font-family: "Source Sans Pro";
                            font-style: normal;
                            font-weight: 700;
                            src: local("Source Sans Pro Bold"), local("SourceSansPro-Bold"), url(https://fonts.gstatic.com/s/sourcesanspro/v10/toadOcfmlt9b38dHJxOBGFkQc6VGVFSmCnC_l7QZG60.woff) format("woff");
                            }
                        }

                        body,
                        table,
                        td,
                        a {
                            -ms-text-size-adjust: 100%; 
                            -webkit-text-size-adjust: 100%; 
                        }

                        table,
                        td {
                            mso-table-rspace: 0pt;
                            mso-table-lspace: 0pt;
                        }

                        img {
                            -ms-interpolation-mode: bicubic;
                        }

                        a[x-apple-data-detectors] {
                            font-family: inherit !important;
                            font-size: inherit !important;
                            font-weight: inherit !important;
                            line-height: inherit !important;
                            color: inherit !important;
                            text-decoration: none !important;
                        }

                        div[style*="margin: 16px 0;"] {
                            margin: 0 !important;
                        }

                        body {
                            width: 100% !important;
                            height: 100% !important;
                            padding: 0 !important;
                            margin: 0 !important;
                        }

                        table {
                            border-collapse: collapse !important;
                        }

                        a {
                            color: black;
                        }

                        img {
                            height: auto;
                            line-height: 100%;
                            text-decoration: none;
                            border: 0;
                            outline: none;
                        }
                        </style>

                        </head>
                        <body style="background-color: #e9ecef;">

                        <div class="preheader" style="display: none; max-width: 0; max-height: 0; overflow: hidden; font-size: 1px; line-height: 1px; color: #fff; opacity: 0;">
                            A new tenant has been created, please use the following link to set new password
                        </div>

                        <table border="0" cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                            <td align="center" bgcolor="#e9ecef">
                                <table border="0" cellpadding="0" cellspacing="0" width="100%" style="max-width: 600px;">
                                <tr>
                                    <td bgcolor="#ffffff" align="left">
                                    
                                    </td>
                                </tr>
                                </table>
                            </td>
                            </tr>
                            <tr>
                            <td align="center" bgcolor="#e9ecef">
                                <table border="0" cellpadding="0" cellspacing="0" width="100%" style="max-width: 600px;">
                                <tr>
                                    <td bgcolor="#ffffff" align="left" style="padding: 24px; font-family: Source Sans Pro, Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;">
                                    <h1 style="margin: 0 0 12px; font-size: 32px; font-weight: 400; line-height: 48px;">Account locked!</h1>
                                    <p style="margin: 0;">Hello, {{UserName}}.</p>
									<br/>
                                    <p style="margin: 0;">Your account has been locked due to too many invalid login attempts.</p>
									<p style="margin: 0;">If you forgot your password, please use the forgot password function.</p>
									<br/>
									<p style="margin: 0;">In case you want to unlock the account, please click this button.</p>
                                    
                                    </td>
                                </tr>
								<!-- start button -->
                                    <tr>
                                      <td align="left" bgcolor="#ffffff">
                                        <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                          <tr>
                                            <td align="center" bgcolor="#ffffff" style="padding: 12px;">
                                              <table border="0" cellpadding="0" cellspacing="0">
                                                <tr>
                                                  <td align="center" bgcolor="#1a82e2" style="border-radius: 6px;">
                                                    <a href="{{UnlockLink}}" target="_blank" style="display: inline-block; padding: 16px 36px; font-family: Source Sans Pro, Helvetica, Arial, sans-serif; font-size: 16px; color: #ffffff; text-decoration: none; border-radius: 6px;">UNLOCK ACCOUNT</a>
                                                  </td>
                                                </tr>
                                              </table>
                                            </td>
                                          </tr>
                                        </table>
                                      </td>
                                    </tr>
                                    <!-- end button -->
                                <tr>
                                    <td align="left" bgcolor="#ffffff" style="padding: 24px; font-family: Source Sans Pro, Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-bottom: 3px solid #d4dadf">
                                    <p style="margin: 0;">Thank you,<br> Asset Health Team</p>
                                    </td>
                                </tr>
                                </table>
                            </td>
                            </tr>
                            
                            <tr>
                            <td align="center" bgcolor="#e9ecef" style="padding: 24px;">
                            
                            </td>
                            </tr>

                        </table>
                        </body>
                        </html>';

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