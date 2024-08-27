DECLARE @clientId  nvarchar(255) = '735E9F04-649F-4C61-8311-EE54AB18CC93'
DECLARE @clientDescription nvarchar(255) = N'Geothermal Frontend description'
DECLARE @Id int
select @Id = id from clients where [Description] = @clientDescription

update clients set clientId = @clientId where id = @Id
select @Id = id from clients where clientId = @clientId
-- delete client information

delete from ClientRedirectUris where clientId = @Id
delete from ClientPostLogoutRedirectUris where clientId = @id

INSERT INTO ClientRedirectUris
      (RedirectUri, ClientId)
VALUES
      ('http://localhost:3000/callback', @Id),
      ('http://localhost:3000/silent', @Id),
      ('http://localhost:3000/signin-oidc', @Id),
      ('http://localhost:3001/callback', @Id),
      ('http://localhost:3001/silent', @Id),
      ('http://localhost:3001/signin-oidc', @Id),
      ('http://localhost:3002/callback', @Id),
      ('http://localhost:3002/silent', @Id),
      ('http://localhost:3002/signin-oidc', @Id),
      ('http://localhost:3003/callback', @Id),
      ('http://localhost:3003/silent', @Id),
      ('http://localhost:3003/signin-oidc', @Id),
      ('http://localhost:3004/callback', @Id),
      ('http://localhost:3004/silent', @Id),
      ('http://localhost:3004/signin-oidc', @Id),
      ('http://localhost:3005/callback', @Id),
      ('http://localhost:3005/silent', @Id),
      ('http://localhost:3005/signin-oidc', @Id),
      ('http://localhost:3006/callback', @Id),
      ('http://localhost:3006/silent', @Id),
      ('http://localhost:3006/signin-oidc', @Id)

INSERT INTO ClientPostLogoutRedirectUris
      (PostLogoutRedirectUri, ClientId)
VALUES
      ('http://localhost:3000', @Id),
      ('http://localhost:3001', @Id),
      ('http://localhost:3002', @Id),
      ('http://localhost:3003', @Id),
      ('http://localhost:3004', @Id),
      ('http://localhost:3005', @Id),
      ('http://localhost:3006', @Id),
      ('http://localhost:3000/signout-callback-oidc', @Id),
      ('http://localhost:3001/signout-callback-oidc', @Id),
      ('http://localhost:3002/signout-callback-oidc', @Id),
      ('http://localhost:3003/signout-callback-oidc', @Id),
      ('http://localhost:3004/signout-callback-oidc', @Id),
      ('http://localhost:3005/signout-callback-oidc', @Id),
      ('http://localhost:3006/signout-callback-oidc', @Id)

DELETE FROM ClientClaims WHERE ClientId = @Id

INSERT INTO ClientClaims (Type,Value,ClientId)
VALUES
('tenantId','0779433e-f36b-1410-8650-00f91313348c',@Id)
,('subscriptionId','0e79433e-f36b-1410-8650-00f91313348c',@Id)
,('projectId','34e5ee62-429c-4724-b3d0-3891bd0a08c9',@Id)
,('allowHeader','true',@Id)