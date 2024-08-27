DECLARE @webDomain nvarchar(255) = N'https://ahs-dev01-asset-viewer-sea-wa.azurewebsites.net'
DECLARE @clientDescription nvarchar(255) = N'Asset Viewer description'
DECLARE @Id int
select @Id = id from clients where  [Description] = @clientDescription
-- delete client information

delete from ClientRedirectUris where clientId = @Id
delete from ClientPostLogoutRedirectUris where clientId = @id

INSERT INTO ClientRedirectUris
      (RedirectUri, ClientId)
VALUES
      (CONCAT(@webDomain,'/SsoReturnPage.aspx'), @Id),
      (CONCAT('http://localhost:8081','/SsoReturnPage.aspx'), @Id),
      (CONCAT('http://localhost:5000','/SsoReturnPage.aspx'), @Id)

INSERT INTO ClientPostLogoutRedirectUris
      (PostLogoutRedirectUri, ClientId)
VALUES
      (@webDomain, @Id),
      (CONCAT('http://localhost:5000','/SsoLandingPage.aspx'), @Id),
      (CONCAT('http://localhost:8081','/SsoLandingPage.aspx'), @Id),
      (CONCAT(@webDomain,'/SsoLandingPage.aspx'), @Id)