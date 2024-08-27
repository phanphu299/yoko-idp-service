IF NOT EXISTS (
  SELECT 1
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Users]') 
         AND name = 'deleted'
)
BEGIN
  ALTER TABLE [dbo].[users] ADD [deleted] bit not null default 0
END