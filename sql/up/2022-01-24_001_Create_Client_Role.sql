create table client_roles (
	id uniqueidentifier not null default newsequentialid() primary key,
	client_id uniqueidentifier not null,
	tenant_id uniqueidentifier not null,
	subscription_id uniqueidentifier not null,
	application_id uniqueidentifier not null,
	project_id uniqueidentifier not null,
	role_id uniqueidentifier not null
)