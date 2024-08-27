-- =============================================
-- Description: Insert client Id and properties for SPA
-- Notes:
--		WebDomain do not contains / at the end
-- =============================================
Declare @override nvarchar(255) = '{{override}}'
if(@override = 'true')
begin
      DECLARE @id int
      Declare @tenantId nvarchar(255) ='{{tenantId}}'
      Declare @subTenantId nvarchar(255) ='{{subTenantId}}'
      Declare @tenantDomain nvarchar(255) = '{{tenantDomain}}'
      Declare @subTenantDomain nvarchar(255) = '{{subTenantDomain}}'
      Declare @saClientId nvarchar(255) = '{{saClientId}}'
      DECLARE @saClientName nvarchar(255) = N' Service Account'
      Declare @saClientSecret nvarchar(255) = '{{saClientSecret}}'
      Declare @saClientScope nvarchar(255) = '{{saClientScope}}'
      Declare @subscriptionId nvarchar(255) ='0E79433E-F36B-1410-8650-00F91313348C'
      Declare @projectId nvarchar(255) ='34E5EE62-429C-4724-B3D0-3891BD0A08C9'
      -------------------------------------------------------------------------------------
      SET @saClientName =  @saClientName + N' - ' + @tenantDomain + N' - ' + N' client';

      if not exists( select 1
      from clients with(nolock)
      where clientId = @saClientId)
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
                        @saClientId,--[ClientId]
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
                  from clients with(nolock)
                  where clientId = @saClientId

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
            ('client_credentials', @Id ),
            ('switch_tenant', @Id )

      -- Client scope
      insert into ClientScopes(clientId, scope)
      select @Id, value
      from STRING_SPLIT(@saClientScope, ' ')

      INSERT INTO [dbo].[ClientSecrets]
            ([Description]
            ,[Value]
            ,[Expiration]
            ,[Type]
            ,[Created]
            ,[ClientId])

      VALUES
            (
                  concat(@saClientName, ' secrect'), -- [Description]
                  @saClientSecret, --[Value]: CU8cEU3yJ4hCEd6QAB
                  null, --[Expiration]
                  'SharedSecret', --[Type]
                  getutcdate(), --[Created]
                  @Id
      )
      INSERT INTO ClientClaims ([Type],[Value],[ClientId])
      VALUES 
      ('tenantId',@tenantId,@Id)
      ,('subTenantId',@subTenantId,@Id)
      ,('tenantDomain',@tenantDomain,@Id)
      ,('subTenantDomain',@subTenantDomain,@Id)
      ,('subscriptionId',@subscriptionId,@Id)
      ,('projectId',@projectId,@Id)
      ,('sid','EFDEB841-F88B-4F2B-17D7-08D9B4A0B45B',@Id)
      ,('allowHeader','true',@Id)
end