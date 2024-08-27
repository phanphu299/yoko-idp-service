INSERT INTO [Users] (user_id, upn, Email, first_name, last_name, password, required_change_password, mfa,login_type_code) values ('2aac152b-6cd8-42f9-9a86-6c5ca078033f','automationtest.administrator@ahi.com','automationtest.administrator@ahi.com','Automation','Adminstrator',N'AQAAAAEAACcQAAAAEIUM1lt/SY8ndgLBoKoed94BMlktWsDs6EvlH/vZyJkm5YDYcZpoNgXfauCi5p5o/A==',0,0, 'ahi-local');

INSERT INTO [Users] (user_id, upn, Email, first_name, last_name, password, required_change_password, mfa,login_type_code) values ('5eddbf4f-8486-4ae7-b525-f229a14b8d19','automationtest.engineer@ahi.com','automationtest.engineer@ahi.com','Automation','Engineer',N'AQAAAAEAACcQAAAAEIUM1lt/SY8ndgLBoKoed94BMlktWsDs6EvlH/vZyJkm5YDYcZpoNgXfauCi5p5o/A==',0,0, 'ahi-local');

INSERT INTO [Users] (user_id, upn, Email, first_name, last_name, password, required_change_password, mfa,login_type_code) values ('ae7211e1-3236-4efc-90d4-d24520604124','automationtest.operator@ahi.com','automationtest.operator@ahi.com','Automation','Operator',N'AQAAAAEAACcQAAAAEIUM1lt/SY8ndgLBoKoed94BMlktWsDs6EvlH/vZyJkm5YDYcZpoNgXfauCi5p5o/A==',0,0, 'ahi-local');

INSERT INTO [Users] (user_id, upn, Email, first_name, last_name, password, required_change_password, mfa,login_type_code) values ('4455a442-db95-499a-be10-ba6f0ffd0bab','automationtest.viewer@ahi.com','automationtest.viewer@ahi.com','Automation','Viewer',N'AQAAAAEAACcQAAAAEIUM1lt/SY8ndgLBoKoed94BMlktWsDs6EvlH/vZyJkm5YDYcZpoNgXfauCi5p5o/A==',0,0, 'ahi-local');

INSERT INTO [Users] (user_id, upn, Email, first_name, last_name, password, required_change_password, mfa,login_type_code) values ('932fdd18-4642-48be-bb9c-723cb4f10149','automationtest.dataviewer@ahi.com','automationtest.dataviewer@ahi.com','Automation','DataViewer',N'AQAAAAEAACcQAAAAEIUM1lt/SY8ndgLBoKoed94BMlktWsDs6EvlH/vZyJkm5YDYcZpoNgXfauCi5p5o/A==',0,0, 'ahi-local');

-- Pass: Pass123$

-- tenant: automation
-- subscription: automation
update users set tenant_id = '0779433e-f36b-1410-8650-00f91313348c', subscription_id = '0e79433e-f36b-1410-8650-00f91313348c'
