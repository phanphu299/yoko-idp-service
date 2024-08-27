declare @resource nvarchar(255) = 'chile-data-source'
DECLARE @resourceSecret nvarchar(255) = 'LeEYBpKsx/orR2d8f0AEvmO+2291/gq1Mub8eLAndLA=' -- 7MHZYcrgrtFE4VenZjTYbajyTtpVLJfXZY
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