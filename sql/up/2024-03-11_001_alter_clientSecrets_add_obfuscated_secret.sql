IF NOT EXISTS (
  SELECT 1
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[ClientSecrets]') 
         AND name = 'obfuscated_secret'
)
BEGIN
  ALTER TABLE [ClientSecrets]
  ADD [obfuscated_secret] nvarchar(50);
END