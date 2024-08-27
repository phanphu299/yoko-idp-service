
IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE name='project_clients' and xtype='U')
    CREATE TABLE project_clients (
        id uniqueidentifier NOT NULL PRIMARY KEY,
	    client_id int NOT NULL,
        tenant_id uniqueidentifier not null,
        subscription_id uniqueidentifier not null,
        project_id uniqueidentifier not null,
        FOREIGN KEY (client_id) REFERENCES Clients(Id)
    )
GO