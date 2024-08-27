DECLARE @clientDescription nvarchar(255) = N'Superset App description'
DECLARE @Id int
select @Id = id from clients where [Description] = @clientDescription
-- delete client information

delete from ClientRedirectUris where clientId = @Id
delete from ClientCorsOrigins where clientId = @Id

-- Redirect uri
INSERT INTO ClientRedirectUris(RedirectUri, clientId)
VALUES
('https://superset.ahi.apps.yokogawa.com/oauth-authorized/ahi', @Id)
