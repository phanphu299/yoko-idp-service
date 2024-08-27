IF EXISTS (
  SELECT 1
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Users]') 
         AND name = 'user_type'
)
BEGIN
  ALTER TABLE [users] DROP CONSTRAINT df_users_user_type;
  ALTER TABLE [users] DROP COLUMN [user_type];
END

IF NOT EXISTS (
  SELECT 1
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Users]') 
         AND name = 'user_type_code'
)
BEGIN
  ALTER TABLE [dbo].[users]
  ADD [user_type_code] VARCHAR(2) not null
  CONSTRAINT df_users_user_type_code DEFAULT 'la' WITH VALUES;
END