declare @resource nvarchar(255) = 'asset-document'
declare @resourceSecret nvarchar(255) = 'bX081KPxytnKYyiTR6QWX4vpxa+o7ZEP8MAqlakx49U=' --kUNhO4kPxEeCJZmU68rfnOQlLJ4fLP
declare @resourceId int
if not exists (select 1 from ApiResources api with(nolock) where name = @resource) 
	begin
	 insert into ApiResources(Enabled, Name, DisplayName, NonEditable, Created, Updated) values (1,@resource, concat('Can get ' ,@resource), 1, GETUTCDATE(), GETUTCDATE())
	 select @resourceId = SCOPE_IDENTITY()
	 insert into ApiSecrets(Description, Value, Expiration, Type, Created, ApiResourceId)
	 values (CONCAT(@resource, ' secret'), @resourceSecret, null, 'SharedSecret' , GETUTCDATE(), @resourceId)
	 insert into ApiScopes( Name, DisplayName, Description, Required, Emphasize,ShowInDiscoveryDocument, ApiResourceId) 
	 values (@resource,@resource, concat('Can get ' ,@resource), 0,  1, 1, @resourceId)
	end