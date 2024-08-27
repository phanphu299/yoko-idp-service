-- =============================================
-- Description: Insert client Id and properties for SPA
-- Notes:
--		WebDomain do not contains / at the end
-- =============================================
DECLARE @id int
DECLARE @clientName nvarchar(255) = N'Post Service Account'
Declare @tenantId nvarchar(255) ='0779433e-f36b-1410-8650-00f91313348c'
Declare @subTenantId nvarchar(255) ='0e79433e-f36b-1410-8650-00f91313348c'
Declare @tenantDomain nvarchar(255) = 'yokogawa'
Declare @subTenantDomain nvarchar(255) = 'ahi'
Declare @subscriptionId nvarchar(255) ='0e79433e-f36b-1410-8650-00f91313348c'
Declare @projectId nvarchar(255) ='34e5ee62-429c-4724-b3d0-3891bd0a08c9'
-------------------------------------------------------------------------------------
SET @clientName =  @clientName + N'_' + @tenantDomain + N'_' + @subTenantDomain + N'_' + N'client';

if not exists( select 1
from clients with(nolock)
where clientName = @clientName)
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
                  'postman-sa',--[ClientId]
                  'oidc', --[ProtocolType]
                  1,--[RequireClientSecret]
                  @clientName, --[ClientName]
                  concat(@clientName,' description'), --[Description]
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
            where clientName = @clientName

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
values ('profile', @id),
       ('user-data', @id),
       ('role-data', @id),
       ('tenant-data', @id),
       ('project-data', @id),
       ('broker-data', @id),
       ('audit-data', @id),
       ('alarm-data', @id),
       ('storage-data', @id),
       ('notification-data', @id),
       ('asset-data', @id),
       ('config-data', @id),
       ('master-data', @id),
       ('dashboard-data', @id),
       ('reporting-data', @id),
       ('scheduler-data', @id),
       ('client-data', @id),
       ('event-data', @id),
       ('localization-data', @id),
       ('data-forwarding-data', @id),
       ('dataset-data', @id),
       ('model-data', @id),
       ('training-data', @id),
       ('entity-data', @id),
       ('bookmark-data', @id),
       ('asset-media-data', @id),
       ('asset3d-data', @id),
       ('asset-table-data', @id),
       ('asset-document', @id),
       ('tag-data', @id)

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
            'gyUaiiz7qRSned525Vml3GLPqz7vXEyTzlD+NSQIx38=', --[Value]: CU8cEU3yJ4hCEd6QAB
            null, --[Expiration]
            'SharedSecret', --[Type]
            getutcdate(), --[Created]
            @Id
)
INSERT INTO ClientClaims ([Type],[Value],[ClientId])
VALUES 
('tenantId',@tenantId,@Id)
,('subTenantId',@subTenantId,@Id)
,('subscriptionId',@subscriptionId,@Id)
,('projectId',@projectId,@Id)
,('upn','thanh.tran@yokogawa.com',@Id)
,('allowHeader','true',@Id)