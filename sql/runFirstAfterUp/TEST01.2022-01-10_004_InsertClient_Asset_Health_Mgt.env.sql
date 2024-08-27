
DECLARE @webDomain nvarchar(255) = N'https://ahs-test01-asset-health-fe-sea-wa.azurewebsites.net'
DECLARE @canaryDomain nvarchar(255) = N'https://ahs-test01-asset-health-canary-sea-wa.azurewebsites.net'
DECLARE @clientDescription nvarchar(255) = N'Tenant Health Frontend description'
DECLARE @Id int
select @Id = id from clients where [Description] = @clientDescription
-- delete client information

delete from ClientRedirectUris where clientId = @Id
delete from ClientPostLogoutRedirectUris where clientId = @id

INSERT INTO ClientRedirectUris
      (RedirectUri, ClientId)
VALUES
      (CONCAT(@webDomain,'/signin-oidc'), @Id),
      (CONCAT(@webDomain,'/callback'), @Id),
      (CONCAT(@webDomain,'/silent'), @Id),
      (CONCAT(@canaryDomain,'/signin-oidc'), @Id),
      (CONCAT(@canaryDomain,'/callback'), @Id),
      (CONCAT(@canaryDomain,'/silent'), @Id)

INSERT INTO ClientPostLogoutRedirectUris
      (PostLogoutRedirectUri, ClientId)
VALUES
      (@webDomain, @Id),
      (@canaryDomain, @Id),
      (CONCAT(@webDomain,'/signout-callback-oidc'), @Id),
      (CONCAT(@canaryDomain,'/signout-callback-oidc'), @Id)
