DECLARE @webDomain nvarchar(255) = N'http://afec0dbe4f8a94262b2860419be9a295-428122111.ap-southeast-1.elb.amazonaws.com'
DECLARE @clientDescription nvarchar(255) = N'Tenant Management Frontend description'
DECLARE @Id int
select @Id = id from clients where [Description] = @clientDescription

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
