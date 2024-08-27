DECLARE @df_name NVARCHAR(255)
-- add CONSTRAINT for created_utc
SELECT @df_name = dc.name
FROM sys.columns cl
INNER JOIN sys.default_constraints dc on dc.parent_object_id = cl.object_id  and cl.column_id = dc.parent_column_id
WHERE dc.parent_object_id = OBJECT_ID('broker_clients')
AND cl.name = 'created_utc'

IF @df_name IS NOT NULL EXEC('ALTER TABLE [broker_clients] DROP CONSTRAINT ' + @df_name)
alter table [broker_clients] ADD CONSTRAINT df_broker_clients_created_utc DEFAULT GETUTCDATE() FOR [created_utc]

-- add CONSTRAINT for updated_utc
SELECT @df_name = dc.name
FROM sys.columns cl
INNER JOIN sys.default_constraints dc on dc.parent_object_id = cl.object_id  and cl.column_id = dc.parent_column_id
WHERE dc.parent_object_id = OBJECT_ID('broker_clients')
AND cl.name = 'updated_utc'
IF @df_name IS NOT NULL EXEC('ALTER TABLE [broker_clients] DROP CONSTRAINT ' + @df_name)
alter table [broker_clients] ADD CONSTRAINT df_broker_clients_updated_utc DEFAULT GETUTCDATE() FOR [updated_utc]

-- add CONSTRAINT for expired_utc
SELECT @df_name = dc.name
FROM sys.columns cl
INNER JOIN sys.default_constraints dc on dc.parent_object_id = cl.object_id  and cl.column_id = dc.parent_column_id
WHERE dc.parent_object_id = OBJECT_ID('broker_clients')
AND cl.name = 'expired_utc'
IF @df_name IS NOT NULL EXEC('ALTER TABLE [broker_clients] DROP CONSTRAINT ' + @df_name)
alter table [broker_clients] ADD CONSTRAINT df_broker_clients_expired_utc DEFAULT DATEADD(day, 365, GETUTCDATE()) FOR [expired_utc]

-- add CONSTRAINT for expired_days
SELECT @df_name = dc.name
FROM sys.columns cl
INNER JOIN sys.default_constraints dc on dc.parent_object_id = cl.object_id  and cl.column_id = dc.parent_column_id
WHERE dc.parent_object_id = OBJECT_ID('broker_clients')
AND cl.name = 'expired_days'
IF @df_name IS NOT NULL EXEC('ALTER TABLE [broker_clients] DROP CONSTRAINT ' + @df_name)
alter table [broker_clients] ADD CONSTRAINT df_broker_clients_expired_days DEFAULT '365' FOR [expired_days]

-- add CONSTRAINT for deleted
SELECT @df_name = dc.name
FROM sys.columns cl
INNER JOIN sys.default_constraints dc on dc.parent_object_id = cl.object_id  and cl.column_id = dc.parent_column_id
WHERE dc.parent_object_id = OBJECT_ID('broker_clients')
AND cl.name = 'deleted'
IF @df_name IS NOT NULL EXEC('ALTER TABLE [broker_clients] DROP CONSTRAINT ' + @df_name)
alter table [broker_clients] ADD CONSTRAINT df_broker_clients_deleted DEFAULT 0 FOR [deleted]