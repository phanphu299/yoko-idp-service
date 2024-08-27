DECLARE @clientDescription nvarchar(255) = N'Gadget Mobile Management description'
DECLARE @Id int
select @Id = id from clients where [Description] = @clientDescription
-- delete client information

delete from ClientRedirectUris where clientId = @Id
delete from ClientCorsOrigins where clientId = @Id

INSERT INTO ClientRedirectUris
      (RedirectUri, ClientId)
VALUES
      ('http://localhost:4000/', @Id)

INSERT INTO ClientCorsOrigins
      (Origin, ClientId)
VALUES
      ('http://localhost:4000', @Id)
