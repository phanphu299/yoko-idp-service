DECLARE @clientDescription nvarchar(255) = N'Asuene App description'
DECLARE @Id int
select @Id = id from clients where [Description] = @clientDescription
-- delete client information

delete from ClientRedirectUris where clientId = @Id
delete from ClientCorsOrigins where clientId = @Id

-- Redirect uri
INSERT INTO ClientRedirectUris(RedirectUri, clientId)
VALUES
('https://earthene.auth.ap-northeast-1.amazoncognito.com/saml2/idpresponse', @Id),
('https://earthene.auth.ap-northeast-1.amazoncognito.com/oauth2/idpresponse', @Id)
