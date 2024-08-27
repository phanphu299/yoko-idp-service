-- =============================================
DECLARE @Id int
DECLARE @saClientName nvarchar(255) = N'EMQX Service Account'
Declare @saClientSecret nvarchar(255) = 'GIzihZPLk9pIKJhZ0uaj4+Lc1/sonvLx8RI/V/2s6VQ=' -- xsdLservCCfurN0uz65k8GL9Dd6tC2Wzn4EDRpmK
Declare @saClientId uniqueidentifier 
-------------------------------------------------------------------------------------

if not exists( select 1
from [Clients] with(nolock)
where ClientName = @saClientName)
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
                  newId(),--[ClientId]
                  'oidc', --[ProtocolType]
                  1,--[RequireClientSecret]
                  @saClientName, --[ClientName]
                  concat(@saClientName,' description'), --[Description]
                  null, --[ClientUri]
                  null, --[LogoUri]
                  0, --[RequireConsent]
                  1, --[AllowRememberConsent]
                  0, --[AlwaysIncludeUserClaimsInIdToken]
                  0, --[RequirePkce]
                  0,--[AllowPlainTextPkce]
                  0, --[AllowAccessTokensViaBrowser]
                  null, --[FrontChannelLogoutUri]
                  0, --[FrontChannelLogoutSessionRequired]
                  null, --[BackChannelLogoutUri]
                  0, --[BackChannelLogoutSessionRequired]
                  0, --[AllowOfflineAccess]
                  3600, --[IdentityTokenLifetime]
                  2147483647, --[AccessTokenLifetime] ~= 68 years
                  3600, --[AuthorizationCodeLifetime]
                  null, --[ConsentLifetime]
                  3600, --[AbsoluteRefreshTokenLifetime]
                  3600, --[SlidingRefreshTokenLifetime]
                  2592000, --[RefreshTokenUsage]
                  0, --[UpdateAccessTokenClaimsOnRefresh]
                  2592000, --[RefreshTokenExpiration]
                  0, --[AccessTokenType]
                  1, --[EnableLocalLogin]
                  0, --[IncludeJwtId]
                  0,--[AlwaysSendClientClaims]
                  '', --[ClientClaimsPrefix]
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
      BEGIN
            select @Id = Id
            from clients with(nolock)
                  where ClientName = @saClientName

            -- delete client information
            delete from ClientGrantTypes where clientId = @Id
            delete from ClientScopes where clientId = @Id
            delete from ClientRedirectUris where clientId = @Id
            delete from ClientPostLogoutRedirectUris where clientId = @Id
            delete from ClientSecrets where clientId = @Id
            delete from ClientClaims where clientId = @Id

            SELECT @saClientId = clientId from clients where Id = @Id
            delete from client_roles where client_id = @saClientId;
      END
END

-- client grant type
INSERT INTO [dbo].[ClientGrantTypes]
      ([GrantType]
      ,[ClientId])
VALUES
      ('client_credentials', @Id )

INSERT INTO [dbo].[ClientSecrets]
      ([Description]
      ,[Value]
      ,[Expiration]
      ,[Type]
      ,[Created]
      ,[ClientId])
VALUES
      (
            concat(@saClientName, ' secret'), -- [Description]
            @saClientSecret,
            null, --[Expiration]
            'SharedSecret', --[Type]
            getutcdate(), --[Created]
            @Id
)

-- Client scope
insert into ClientScopes(scope, clientId)
values ('openid', @Id)

