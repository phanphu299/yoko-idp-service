create table user_subscriptions (
	upn nvarchar(255) not null, -- user identifier
	tenant_id uniqueidentifier not null,
	subscription_id uniqueidentifier not null
)