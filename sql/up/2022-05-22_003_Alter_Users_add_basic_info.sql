ALTER TABLE users ADD avatar NVARCHAR(2048) null;
ALTER TABLE users ADD display_date_time_format NVARCHAR(50) not null CONSTRAINT DF_users_display_date_time_format DEFAULT N'YYYY-MM-DD HH:mm:ss.SSS' WITH VALUES;
ALTER TABLE users ADD date_time_format NVARCHAR(50) not null CONSTRAINT DF_users_date_time_format DEFAULT N'yyyy-MM-dd HH:mm:ss.fff' WITH VALUES;
ALTER TABLE users ADD timezone_id int NOT NULL CONSTRAINT DF_users_timezone_id DEFAULT N'205' WITH VALUES;
ALTER TABLE [users] ADD CONSTRAINT fk_user_timezone_id FOREIGN KEY (timezone_id) REFERENCES timezones(id);
