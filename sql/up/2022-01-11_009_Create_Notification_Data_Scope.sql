declare @resource nvarchar(255) = 'notification-data'
declare @resourceSecret nvarchar(255) = 'AYMkPbfygOBVraKt1AjTfIjz/3cf3QQkC5yORGo2QtU=' --W9fSY2V6eeQbWjQd9rA2ZZtHBX6KW33X
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