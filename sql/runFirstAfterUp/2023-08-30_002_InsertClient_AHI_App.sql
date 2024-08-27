DECLARE @clientName nvarchar(255) = N'AHI Application'
DECLARE @clientSecret nvarchar(255) = N'GjC5xt8FXTrvZzXh5ammrN9VizpuNdrX9VIp2hOTRIw='; -- byctvWvkH8HYX4bhbFJnzgCYM9Y7sekt
DECLARE @clientDescription nvarchar(255) = N'AHI Application - Description'
DECLARE @Id int
if not exists( select 1
from clients with(nolock)
where [clientName] = @clientName)
BEGIN
      INSERT INTO [dbo].[Clients]
            ([Enabled]
            ,[ClientId]
            ,[ProtocolType]
            ,[RequireClientSecret]
            ,[ClientName]
            ,[Description]
            ,[ClientUri]
            ,[LogoUri]
            ,[RequireConsent]
            ,[AllowRememberConsent]
            ,[AlwaysIncludeUserClaimsInIdToken]
            ,[RequirePkce]
            ,[AllowPlainTextPkce]
            ,[AllowAccessTokensViaBrowser]
            ,[FrontChannelLogoutUri]
            ,[FrontChannelLogoutSessionRequired]
            ,[BackChannelLogoutUri]
            ,[BackChannelLogoutSessionRequired]
            ,[AllowOfflineAccess]
            ,[IdentityTokenLifetime]
            ,[AccessTokenLifetime]
            ,[AuthorizationCodeLifetime]
            ,[ConsentLifetime]
            ,[AbsoluteRefreshTokenLifetime]
            ,[SlidingRefreshTokenLifetime]
            ,[RefreshTokenUsage]
            ,[UpdateAccessTokenClaimsOnRefresh]
            ,[RefreshTokenExpiration]
            ,[AccessTokenType]
            ,[EnableLocalLogin]
            ,[IncludeJwtId]
            ,[AlwaysSendClientClaims]
            ,[ClientClaimsPrefix]
            ,[PairWiseSubjectSalt]
            ,[Created]
            ,[Updated]
            ,[LastAccessed]
            ,[UserSsoLifetime]
            ,[UserCodeType]
            ,[DeviceCodeLifetime]
            ,[NonEditable])
      VALUES
            (
                  1, --[Enabled]
                  newid(),--[ClientId]
                  'oidc', --[ProtocolType]
                  0,--[RequireClientSecret]
                  @clientName, --[ClientName]
                  @clientDescription, --[Description]
                  null, --[ClientUri]
                  null, --[LogoUri]
                  0, --[RequireConsent]
                  1, --[AllowRememberConsent]
                  1, --[AlwaysIncludeUserClaimsInIdToken]
                  0, --[RequirePkce]
                  0,--[AllowPlainTextPkce]
                  1, --[AllowAccessTokensViaBrowser]
                  null, --[FrontChannelLogoutUri]
                  0, --[FrontChannelLogoutSessionRequired]
                  null, --[BackChannelLogoutUri]
                  0, --[BackChannelLogoutSessionRequired]
                  1, --[AllowOfflineAccess]
                  3600, --[IdentityTokenLifetime]
                  3600, --[AccessTokenLifetime]
                  3600, --[AuthorizationCodeLifetime]
                  null, --[ConsentLifetime]
                  3600, --[AbsoluteRefreshTokenLifetime]
                  3600, --[SlidingRefreshTokenLifetime]
                  2592000, --[RefreshTokenUsage]
                  0, --[UpdateAccessTokenClaimsOnRefresh]
                  2592000, --[RefreshTokenExpiration]
                  1, --[AccessTokenType]
                  1, --[EnableLocalLogin]
                  1, --[IncludeJwtId]
                  0,--[AlwaysSendClientClaims]
                  null, --[ClientClaimsPrefix]
                  null, --[PairWiseSubjectSalt]
                  getutcdate(),--[Created]
                  getutcdate(),--[Updated]
                  null, --[LastAccessed]
                  null, --[UserSsoLifetime]
                  null, --[UserCodeType]
                  3600, --[DeviceCodeLifetime]
                  1 --[NonEditable]
)
      SELECT @Id = Scope_Identity()
END
ELSE
BEGIN
	select @Id = id from clients where [clientName] = @clientName
	-- delete client information
      delete from ClientGrantTypes where clientId = @Id
      delete from ClientScopes where clientId = @Id
      delete from ClientIdPRestrictions where clientId = @Id
      delete from ClientClaims where clientId = @Id
      delete from ClientRedirectUris where clientId = @Id
      delete from ClientPostLogoutRedirectUris where clientId = @Id      
      delete from ClientSecrets where clientId = @Id
END

INSERT INTO [dbo].[ClientGrantTypes]
      ([GrantType]
      ,[ClientId])
VALUES
      ('authorization_code', @Id )

INSERT INTO [dbo].[ClientScopes]
      ([Scope]
      ,[ClientId])
values ('profile', @id),
       ('openid', @Id),
       ('email', @Id)

INSERT INTO ClientIdPRestrictions ([Provider],[ClientId])
VALUES ('aad-ahi',@Id)


INSERT INTO [dbo].[ClientSecrets]
            ([Description]
            ,[Value]
            ,[Expiration]
            ,[Type]
            ,[Created]
            ,[ClientId])

      VALUES
            (
                  concat(@clientName, ' secrect'), -- [Description]
                  @clientSecret,
                  null, --[Expiration]
                  'SharedSecret', --[Type]
                  getutcdate(), --[Created]
                  @id
      )
