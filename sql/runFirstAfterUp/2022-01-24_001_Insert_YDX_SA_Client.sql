-- =============================================
DECLARE @id int
DECLARE @clientName nvarchar(255) = N'AHI Service Account'
DECLARE @clientDescription nvarchar(255) = N'YDX Service Account description'
Declare @saClientSecret nvarchar(255) = 'XCo2aZw4+KFTeuHZs7RJPktZg8Dbe3uWHPJ9U7pRFWI=' -- F9aDTtWk89PQzac8fce5WepTDzHETSTV2xLCAxMs
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
      ('config-data', @id),
      ('role-data', @id),
      ('tenant-data', @id),
      ('project-data', @id),
      ('broker-data', @id),
      ('audit-data', @id),
      ('storage-data', @id),
      ('alarm-data', @id),
      ('dataset-data', @id),
      ('model-data', @id),
      ('training-data', @id),
      ('dashboard-data', @id),
      ('master-data', @id),
      ('asset-data', @id),
      ('reporting-data', @id),
      ('scheduler-data', @id),
      ('client-data', @id),
      ('event-data', @id),
      ('localization-data', @id),
      ('data-forwarding-data', @id),
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

set @applicationId  ='2DE4423E-F36B-1410-81D6-007EB43F7292' -- 'Asset Identity
set @adminRoleId = 'A65C1BAF-C30D-449F-98EE-340C49D11025' -- 'Administrator'
insert into client_roles (client_id, application_id, project_id, role_id )
values ( @saClientId,@applicationId , null, @adminRoleId)

set @applicationId  ='F957CB99-0EC3-47FA-9BB7-A48650C17BA0' -- 'Asset Analytic
set @adminRoleId = 'BF8C8B1D-748C-48B2-B598-38E7CBD3C7DC' -- 'Administrator'
insert into client_roles (client_id, application_id, project_id, role_id )
values ( @saClientId,@applicationId , null, @adminRoleId)

set @applicationId  ='EA8F57B2-F183-4ACC-88B0-249ECB59286E' -- 'Asset Dashboard
set @adminRoleId = 'B4B7DEAD-C658-458C-863E-88C91966662A' -- 'Administrator'
insert into client_roles (client_id, application_id, project_id, role_id )
values ( @saClientId,@applicationId , null, @adminRoleId)

set @applicationId  ='34E4423E-F36B-1410-81D6-007EB43F7292' -- 'Asset Tenant'
set @adminRoleId = '9BAFC561-9933-4D8B-B77E-420B7B710F9E' -- 'Tenant Administrator'
insert into client_roles (client_id, application_id, project_id, role_id )
values ( @saClientId,@applicationId , null, @adminRoleId)
