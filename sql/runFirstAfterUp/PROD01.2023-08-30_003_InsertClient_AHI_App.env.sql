DECLARE @webDomain nvarchar(255) = N'https://app01.ahi.apps.yokogawa.com'
DECLARE @canaryDomain nvarchar(255) = N'https://app02.ahi.apps.yokogawa.com'
DECLARE @clientDescription nvarchar(255) = N'AHI Application - Description'
DECLARE @Id int
select @Id = id from clients where [Description] = @clientDescription
-- delete client information

delete from ClientRedirectUris where clientId = @Id
delete from ClientPostLogoutRedirectUris where clientId = @id

INSERT INTO ClientRedirectUris
      (RedirectUri, ClientId)
VALUES
      (CONCAT(@webDomain,'/.auth/login/ahi/callback'), @Id),
      (CONCAT(@canaryDomain,'/.auth/login/ahi/callback'), @Id),
      (CONCAT(@webDomain,'/signin-oidc'), @Id),
      (CONCAT(@canaryDomain,'/signin-oidc'), @Id)