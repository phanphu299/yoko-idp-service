DECLARE @webDomain nvarchar(255) = N'https://ahs-test03-geothermal-fe-sea-wa.azurewebsites.net'
DECLARE @clientDescription nvarchar(255) = N'Geothermal Frontend description'
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
      (CONCAT(@webDomain,'/silent'), @Id)

INSERT INTO ClientPostLogoutRedirectUris
      (PostLogoutRedirectUri, ClientId)
VALUES
      (@webDomain, @Id),
      (CONCAT(@webDomain,'/signout-callback-oidc'), @Id)