IF NOT EXISTS (
  SELECT 1
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Users]') 
         AND name = 'tenant_id'
)
BEGIN
  ALTER TABLE [dbo].[users]
  ADD [tenant_id] uniqueidentifier NULL
END

IF NOT EXISTS (
  SELECT 1
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Users]') 
         AND name = 'subscription_id'
)
BEGIN
  ALTER TABLE [dbo].[users]
  ADD [subscription_id] uniqueidentifier NULL
END
