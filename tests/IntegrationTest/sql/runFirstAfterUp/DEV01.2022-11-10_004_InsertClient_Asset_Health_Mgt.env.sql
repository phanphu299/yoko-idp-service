DECLARE @clientId  nvarchar(255) = 'd112273d-1625-4e87-a67e-bf97cb0b4969'
DECLARE @clientDescription nvarchar(255) = N'Tenant Health Frontend description'
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
      ('http://localhost:3005/signin-oidc', @Id)

INSERT INTO ClientPostLogoutRedirectUris
      (PostLogoutRedirectUri, ClientId)
VALUES
      ('http://localhost:3000', @Id),
      ('http://localhost:3001', @Id),
      ('http://localhost:3002', @Id),
      ('http://localhost:3003', @Id),
      ('http://localhost:3004', @Id),
      ('http://localhost:3005', @Id),
      ('http://localhost:3000/signout-callback-oidc', @Id),
      ('http://localhost:3001/signout-callback-oidc', @Id),
      ('http://localhost:3002/signout-callback-oidc', @Id),
      ('http://localhost:3003/signout-callback-oidc', @Id),
      ('http://localhost:3004/signout-callback-oidc', @Id),
      ('http://localhost:3005/signout-callback-oidc', @Id)