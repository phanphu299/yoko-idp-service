
DECLARE @clientName nvarchar(255) = N'Data Analytic'
DECLARE @clientDescription nvarchar(255) = N'Asset Analytic FE description'
DECLARE @Id int
DECLARE @ApplicationId UNIQUEIDENTIFIER = 'f957cb99-0ec3-47fa-9bb7-a48650c17ba0';
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
                  1, --[RequirePkce]
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
                  1,--[AlwaysSendClientClaims]
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
      select @Id = id from clients where [Description] = @clientDescription
      -- set return client claims in token by default
      update Clients set AlwaysSendClientClaims = 1, ClientName = @clientName where id = @Id

      -- delete client information
      delete from ClientGrantTypes where clientId = @Id
      delete from ClientScopes where clientId = @Id
      delete from ClientIdPRestrictions where clientId = @Id
      delete from ClientClaims where clientId = @Id
      delete from ClientProperties where clientId = @Id
END

INSERT INTO [dbo].[ClientGrantTypes]
      ([GrantType]
      ,[ClientId])
VALUES
      ('authorization_code', @Id ),
      ('user_switch_tenant', @Id )

INSERT INTO [dbo].[ClientScopes]
      ([Scope]
      ,[ClientId])
values ('profile', @id),
       ('openid', @Id),
       ('user-data', @id),
       ('config-data', @id),
       ('role-data', @id),
       ('tenant-data', @id),
       ('project-data', @id),
       ('dataset-data', @id),
       ('model-data', @id),
       ('training-data', @id),
       ('audit-data', @id),
       ('storage-data', @id),
       ('asset-data', @id),
       ('master-data', @id),
       ('event-data', @id),
       ('broker-data', @id),
       ('scheduler-data', @id),
       ('data-forwarding-data', @id),
       ('localization-data', @id),
       ('tag-data', @id)

INSERT INTO ClientIdPRestrictions ([Provider],[ClientId])
VALUES ('aad-ahi',@Id)

INSERT INTO ClientClaims ([Type],[Value],[ClientId])
VALUES ('allowHeader','true',@Id)

INSERT INTO ClientProperties ([Key],[Value],[ClientId])
VALUES ('applicationId',@ApplicationId,@Id)