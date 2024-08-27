
DECLARE @webDomain nvarchar(255) = N'https://asset-analytic-uat.ahi.apps.yokogawa.com.cn'
DECLARE @clientDescription nvarchar(255) = N'Asset Analytic FE description'
DECLARE @Id int
select @Id = id from clients where  [Description] = @clientDescription
-- delete client information

delete from ClientRedirectUris where clientId = @Id
delete from ClientPostLogoutRedirectUris where clientId = @Id
delete from ClientIdpRestrictions where clientId = @Id

INSERT INTO ClientIdpRestrictions (Provider,ClientId)
VALUES ('noprovider',@Id)

INSERT INTO ClientRedirectUris
      (RedirectUri, ClientId)
VALUES
      (CONCAT(@webDomain,'/signin-oidc'), @Id),
      (CONCAT(@webDomain,'/callback'), @Id),
      (CONCAT(@webDomain,'/silent'), @Id)

INSERT INTO ClientPostLogoutRedirectUris
      (PostLogoutRedirectUri, ClientId)
VALUES
      (@webDomain, @Id),
      (CONCAT(@webDomain,'/signout-callback-oidc'), @Id)

