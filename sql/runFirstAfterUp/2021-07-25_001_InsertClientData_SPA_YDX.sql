-- =============================================
-- Description: Insert client Id and properties for SPA
-- Notes:
--		WebDomain do not contains / at the end
-- =============================================
Declare @override nvarchar(255) = '{{override}}'
if(@override = 'true')
begin
      DECLARE @id int
      DECLARE @clientName nvarchar(255) = N'Frontend website'
      DECLARE @webDomain varchar(500) = '{{feWebsiteDomain}}'; -- no / at the end
      Declare @tenantId nvarchar(255) ='{{tenantId}}'
      Declare @subTenantId nvarchar(255) ='{{subTenantId}}'
      Declare @tenantDomain nvarchar(255) = '{{tenantDomain}}'
      Declare @subTenantDomain nvarchar(255) = '{{subTenantDomain}}'
      Declare @feClientId nvarchar(255) = '{{feClientId}}'
      Declare @subscriptionId nvarchar(255) ='0E79433E-F36B-1410-8650-00F91313348C'
      Declare @projectId nvarchar(255) ='34E5EE62-429C-4724-B3D0-3891BD0A08C9'
      -------------------------------------------------------------------------------------
      SET @clientName =  @clientName + N' - ' + @tenantDomain + N' - ' + N' client';
      -------------------------------------------------------------------------------------
      if not exists( select 1
      from clients with(nolock)
      where clientId = @feClientId)
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
                        @feClientId,--[ClientId]
                        'oidc', --[ProtocolType]
                        0,--[RequireClientSecret]
                        @clientName, --[ClientName]
                        concat(@clientName,' description'), --[Description]
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
            select @Id = Id
            from clients with(nolock)
            where clientId = @feClientId

            -- delete client information
            delete from ClientGrantTypes where clientId = @Id
            delete from ClientScopes where clientId = @Id
            delete from ClientRedirectUris where clientId = @Id
            delete from ClientPostLogoutRedirectUris where clientId = @Id
            delete from ClientSecrets where clientId = @Id
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
      VALUES
            ('openid', @Id),
            ('profile', @Id)

      INSERT INTO ClientRedirectUris
            (RedirectUri, ClientId)
      VALUES
            (CONCAT(@webDomain,'/signin-oidc'), @Id),
            (CONCAT(@webDomain,'/callback'), @Id)

      INSERT INTO ClientPostLogoutRedirectUris
            (PostLogoutRedirectUri, ClientId)
      VALUES
            (@webDomain, @Id),
            (CONCAT(@webDomain,'/signout-callback-oidc'), @Id)

      INSERT INTO ClientClaims ([Type],[Value],[ClientId])
      VALUES 
      ('tenantId',@tenantId,@Id)
      ,('subTenantId',@subTenantId,@Id)
      ,('tenantDomain',@tenantDomain,@Id)
      ,('subTenantDomain',@subTenantDomain,@Id)
      ,('subscriptionId',@subscriptionId,@Id)
      ,('projectId',@projectId,@Id)

      INSERT INTO ClientIdPRestrictions ([Provider],[ClientId])
      VALUES ('aad-ahi',@Id)
end