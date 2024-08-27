create table broker_clients (
	[id] varchar(30) not null primary key,
	[password] nvarchar(2048),
	[created_utc] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [updated_utc] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
	[expired_utc] DATETIME2 NOT NULL DEFAULT DATEADD(day, 365, GETUTCDATE()),
	[created_by] nvarchar(255),
	[expired_days] INT NOT NULL DEFAULT '365',
	[tenant_id] nvarchar(50) NOT NULL,
	[subscription_id] nvarchar(50) NOT NULL,
	[project_id] nvarchar(50) NOT NULL,
    [deleted] BIT NOT NULL DEFAULT 0
)