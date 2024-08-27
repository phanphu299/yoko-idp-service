-- =============================================
DECLARE @id int
DECLARE @clientName nvarchar(255) = N'APR-APM Service Account'
DECLARE @clientDescription nvarchar(255) = N'APR-APM Service Account'
Declare @saClientSecret nvarchar(255) = 'J0m6qm0yHL8ttQpsmyYyzKqA/abKFRVt7SSwwlslxw0=' -- #+dx$#LXdF@_<+(%W1)4<(K&3Zl~%T
Declare @saClientId uniqueidentifier 
-------------------------------------------------------------------------------------

if not exists( select 1
from clients with(nolock)
where [Description] = @clientDescription)
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
                  @clientName, --[ClientName]
                  @clientDescription, --[Description]
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
                  3600, --[AccessTokenLifetime]
                  3600, --[AuthorizationCodeLifetime]
                  null, --[ConsentLifetime]
                  3600, --[AbsoluteRefreshTokenLifetime]
                  3600, --[SlidingRefreshTokenLifetime]
                  2592000, --[RefreshTokenUsage]
                  0, --[UpdateAccessTokenClaimsOnRefresh]
                  2592000, --[RefreshTokenExpiration]
                  0, --[AccessTokenType]
                  1, --[EnableLocalLogin]
                  1, --[IncludeJwtId]
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
            from clients with(nolock) where [Description] = @clientDescription
            -- update client information
            update Clients set ClientName = @clientName where id = @Id
            -- delete client information
            delete from ClientGrantTypes where clientId = @Id
            delete from ClientScopes where clientId = @Id
            delete from ClientRedirectUris where clientId = @Id
            delete from ClientPostLogoutRedirectUris where clientId = @Id
            delete from ClientSecrets where clientId = @Id
            delete from ClientClaims where clientId = @Id
      END
END

-- client grant type
INSERT INTO [dbo].[ClientGrantTypes]
      ([GrantType]
      ,[ClientId])
VALUES
      ('client_credentials', @Id )

-- Client scope
insert into ClientScopes(scope, clientId)
values ('user-data', @id),
      ('client-data', @id),
      ('project-data', @id),
      ('asset-data', @id),
      ('localization-data', @id)

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
            @saClientSecret,
            null, --[Expiration]
            'SharedSecret', --[Type]
            getutcdate(), --[Created]
            @Id
)
INSERT INTO ClientClaims ([Type],[Value],[ClientId])
VALUES ('allowHeader','true',@Id)

SELECT @saClientId = clientId from clients where Id = @Id
delete from client_roles where client_id = @saClientId;

declare @applicationId uniqueidentifier ='A0F1C338-1EFF-40FF-997E-64F08E141B06' -- 'Asset Management'
declare @adminRoleId uniqueidentifier = 'B46805C9-2BFF-4541-A415-437F5838E226' -- 'Administrator'
insert into client_roles (client_id, application_id, project_id, role_id )
values ( @saClientId,@applicationId , null, @adminRoleId)
