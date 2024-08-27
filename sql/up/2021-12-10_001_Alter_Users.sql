IF NOT EXISTS (
  SELECT 1
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Users]') 
         AND name = 'mfa'
)
BEGIN
  ALTER TABLE [dbo].[users]
  ADD [mfa] bit not null
  CONSTRAINT df_users_mfa DEFAULT 0 WITH VALUES;
END