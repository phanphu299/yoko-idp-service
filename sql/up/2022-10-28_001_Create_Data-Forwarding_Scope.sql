declare @resource nvarchar(255) = 'data-forwarding-data'
DECLARE @resourceSecret nvarchar(255) = 'xik96bbkzrSjdWs8317HsKXlfABT8gYhUBqPWprGg68=' -- WLkPSyX2pyD7cafFLovxu3y4ILZ5bdgcFf3
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