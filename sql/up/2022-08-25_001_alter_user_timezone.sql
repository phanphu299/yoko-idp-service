ALTER TABLE [users] DROP CONSTRAINT [fk_user_timezone_id];
ALTER TABLE [users] DROP CONSTRAINT [DF_users_timezone_id];
ALTER TABLE [users] ADD  CONSTRAINT [df_users_timezone_id]  DEFAULT 108 FOR [timezone_id] -- // 'Singapore Standard Time','(UTC+08:00) Kuala Lumpur, Singapore','+08:00'