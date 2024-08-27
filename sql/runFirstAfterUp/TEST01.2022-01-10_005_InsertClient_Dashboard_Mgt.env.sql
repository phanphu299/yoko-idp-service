DECLARE @webDomain nvarchar(255) = N'https://ahs-test01-dashboard-mgt-sea-wa.azurewebsites.net'
DECLARE @canaryDomain nvarchar(255) = N'https://ahs-test01-dashboard-canary-sea-wa.azurewebsites.net'
DECLARE @canaryDomain2 nvarchar(255) = N'https://ahs-test01-dashboard-canary02-sea-wa.azurewebsites.net'
DECLARE @canaryDomain3 nvarchar(255) = N'https://ahs-test01-dashboard-canary03-sea-wa.azurewebsites.net'
DECLARE @canaryDomain4 nvarchar(255) = N'https://ahs-test01-dashboard-canary04-sea-wa.azurewebsites.net'
DECLARE @clientDescription nvarchar(255) = N'Dashboard Management Frontend description'
DECLARE @Id int
select @Id = id from clients where  [Description] = @clientDescription
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
      (CONCAT(@canaryDomain,'/silent'), @Id),
      (CONCAT(@canaryDomain2,'/signin-oidc'), @Id),
      (CONCAT(@canaryDomain2,'/callback'), @Id),
      (CONCAT(@canaryDomain2,'/silent'), @Id),
      (CONCAT(@canaryDomain3,'/signin-oidc'), @Id),
      (CONCAT(@canaryDomain3,'/callback'), @Id),
      (CONCAT(@canaryDomain3,'/silent'), @Id),
      (CONCAT(@canaryDomain4,'/signin-oidc'), @Id),
      (CONCAT(@canaryDomain4,'/callback'), @Id),
      (CONCAT(@canaryDomain4,'/silent'), @Id)

INSERT INTO ClientPostLogoutRedirectUris
      (PostLogoutRedirectUri, ClientId)
VALUES
      (@webDomain, @Id),
      (@canaryDomain, @Id),
      (CONCAT(@webDomain,'/signout-callback-oidc'), @Id),
      (CONCAT(@canaryDomain,'/signout-callback-oidc'), @Id),
      (CONCAT(@canaryDomain2,'/signout-callback-oidc'), @Id),
      (CONCAT(@canaryDomain3,'/signout-callback-oidc'), @Id),
      (CONCAT(@canaryDomain4,'/signout-callback-oidc'), @Id)
