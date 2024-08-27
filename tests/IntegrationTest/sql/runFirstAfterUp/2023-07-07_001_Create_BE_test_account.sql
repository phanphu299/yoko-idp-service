
-- =============================================
-- Description: Insert client Id and properties for AD
-- Notes:
--		WebDomain do not contains / at the end
-- =============================================
DECLARE @id int
DECLARE @clientId nvarchar(255) = 'B757B725-46E9-4753-9901-D01B100AD63D'
DECLARE @clientSecret nvarchar(255) = 'Kiwfhbpp3Aj2djKPuiBIpZQm0+YqJ2Ehzr81MqnQAJc=' -- D34BtxpaKSkfcKYCvxLbgKTdV8FUfRTHGPrKLawL
DECLARE @clientName nvarchar(255) = N'AHI BE account'
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
                  @clientId,--[ClientId]
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
            @clientSecret,
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
,('upn','automationtest.administrator@ahi.com',@Id)
,('allowHeader','true',@Id)


SELECT @clientId = clientId from clients where Id = @Id
delete from client_roles where client_id = @clientId;



declare @applicationId uniqueidentifier ='A0F1C338-1EFF-40FF-997E-64F08E141B06' -- 'Asset Management'
declare @adminRoleId uniqueidentifier = 'B46805C9-2BFF-4541-A415-437F5838E226' -- 'Administrator'
insert into client_roles (client_id, application_id, project_id, role_id )
values ( @clientId,@applicationId , null, @adminRoleId)

set @applicationId  ='2DE4423E-F36B-1410-81D6-007EB43F7292' -- 'Asset Identity
set @adminRoleId = 'A65C1BAF-C30D-449F-98EE-340C49D11025' -- 'Administrator'
insert into client_roles (client_id, application_id, project_id, role_id )
values ( @clientId,@applicationId , null, @adminRoleId)

set @applicationId  ='F957CB99-0EC3-47FA-9BB7-A48650C17BA0' -- 'Asset Analytic
set @adminRoleId = 'BF8C8B1D-748C-48B2-B598-38E7CBD3C7DC' -- 'Administrator'
insert into client_roles (client_id, application_id, project_id, role_id )
values ( @clientId,@applicationId , null, @adminRoleId)

set @applicationId  ='EA8F57B2-F183-4ACC-88B0-249ECB59286E' -- 'Asset Dashboard
set @adminRoleId = 'B4B7DEAD-C658-458C-863E-88C91966662A' -- 'Administrator'
insert into client_roles (client_id, application_id, project_id, role_id )
values ( @clientId,@applicationId , null, @adminRoleId)

set @applicationId  ='34E4423E-F36B-1410-81D6-007EB43F7292' -- 'Asset Tenant'
set @adminRoleId = '9BAFC561-9933-4D8B-B77E-420B7B710F9E' -- 'Tenant Administrator'
insert into client_roles (client_id, application_id, project_id, role_id )
values ( @clientId,@applicationId , null, @adminRoleId)


