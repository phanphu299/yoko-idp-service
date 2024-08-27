INSERT INTO [Users] (user_id, upn, Email, first_name, last_name, password, required_change_password, mfa) values ('ebfccd72-fd78-4da0-a7e8-c6c70e1a6f71','thanh.tran@yokogawa.com','thanh.tran@yokogawa.com','Thanh','Tran',N'AQAAAAEAACcQAAAAEIUM1lt/SY8ndgLBoKoed94BMlktWsDs6EvlH/vZyJkm5YDYcZpoNgXfauCi5p5o/A==',0,0)

-- insert to user subscription
-- insert into user_subscriptions (upn, tenant_id, subscription_id)
-- select upn, '0779433E-F36B-1410-8650-00F91313348C','0E79433E-F36B-1410-8650-00F91313348C'
-- from users;
update users set tenant_id = '0779433e-f36b-1410-8650-00f91313348c', subscription_id = '0e79433e-f36b-1410-8650-00f91313348c'
