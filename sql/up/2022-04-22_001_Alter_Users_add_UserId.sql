IF NOT EXISTS (
  SELECT 1
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Users]') 
         AND name = 'user_id'
)
BEGIN
  ALTER TABLE [dbo].[users]
  ADD [user_id] uniqueidentifier NOT NULL
  CONSTRAINT df_users_user_id DEFAULT NEWID()
END