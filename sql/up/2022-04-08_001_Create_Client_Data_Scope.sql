declare @resource nvarchar(255) = 'client-data'
declare @resourceSecret nvarchar(255) = 'e65z9O+O3T3xtcvb9yenNekts2KoFlMk0XfjQFstIZc=' --zOqEuKHnB8Cz6eFVxGzCnMac0ZsQ3w
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