create table user_tenants (
	upn nvarchar(255) not null, -- user identifier
	org_id uniqueidentifier not null,
	org_name nvarchar(255) not null
)