IF NOT EXISTS (
  SELECT 1
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Users]') 
         AND name = 'user_type'
)
BEGIN
  ALTER TABLE [dbo].[users]
  ADD [user_type] nvarchar(25) not null
  CONSTRAINT df_users_user_type DEFAULT 'Local' WITH VALUES;
END