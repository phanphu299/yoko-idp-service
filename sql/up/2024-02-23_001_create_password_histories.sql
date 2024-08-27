IF  NOT EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'password_histories') AND type in (N'U'))
BEGIN
    CREATE TABLE [password_histories] (
    [id] uniqueidentifier not null PRIMARY KEY DEFAULT (NEWID()),
    [upn] NVARCHAR(255) not null,
    [password] nvarchar(2048) not null,
    [created_utc] DATETIME NOT NULL DEFAULT GETUTCDATE()
    );
    ALTER TABLE password_histories ADD CONSTRAINT fk_password_histories_upn FOREIGN KEY (upn) REFERENCES users(upn); 
END