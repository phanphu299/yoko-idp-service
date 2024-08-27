IF NOT EXISTS (
  SELECT 1
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Users]') 
         AND name = 'is_locked'
)
BEGIN
  ALTER TABLE [dbo].[users]
  ADD [is_locked] bit not null
  CONSTRAINT df_users_is_locked DEFAULT 0 WITH VALUES;
END