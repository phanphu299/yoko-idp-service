DECLARE @webDomain nvarchar(255) = N'https://ahs-dev01-dashboard-mgt-sea-wa.azurewebsites.net'
DECLARE @canaryDomain nvarchar(255) = N'https://ahs-dev01-dashboard-canary-sea-wa.azurewebsites.net'
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
      (CONCAT('http://localhost:3000','/callback'), @Id),
      (CONCAT('http://localhost:3000','/silent'), @Id),
       (CONCAT('http://localhost:3001','/callback'), @Id),
      (CONCAT('http://localhost:3001','/silent'), @Id),
       (CONCAT('http://localhost:3002','/callback'), @Id),
      (CONCAT('http://localhost:3002','/silent'), @Id),
       (CONCAT('http://localhost:3003','/callback'), @Id),
      (CONCAT('http://localhost:3003','/silent'), @Id),
       (CONCAT('http://localhost:3004','/callback'), @Id),
      (CONCAT('http://localhost:3004','/silent'), @Id),
       (CONCAT('http://localhost:3005','/callback'), @Id),
      (CONCAT('http://localhost:3005','/silent'), @Id)

INSERT INTO ClientPostLogoutRedirectUris
      (PostLogoutRedirectUri, ClientId)
VALUES
      (@webDomain, @Id),
      (@canaryDomain, @Id),
      ('http://localhost:3000', @Id),
      ('http://localhost:3001', @Id),
      ('http://localhost:3002', @Id),
      ('http://localhost:3003', @Id),
      ('http://localhost:3004', @Id),
      ('http://localhost:3005', @Id),
      (CONCAT(@webDomain,'/signout-callback-oidc'), @Id),
      (CONCAT(@canaryDomain,'/signout-callback-oidc'), @Id)



