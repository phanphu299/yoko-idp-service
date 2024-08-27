if not exists (select 1 from broker_clients where id = 'broker-listener')

insert into broker_clients (id, password, expired_days, expired_utc, tenant_id, subscription_id, project_id)
values ('broker-listener', 'Fe5XPfrJdD5qAZY5JTPWJP8Jr8GXMEFx', 2147483647, '9999-12-31 23:59:59.997', '', '', '')