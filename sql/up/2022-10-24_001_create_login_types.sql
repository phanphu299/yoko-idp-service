GO
CREATE TABLE [login_types] (
[code] VARCHAR(10) not null PRIMARY KEY,
[name] NVARCHAR(255) not null,
[created_utc] DATETIME NOT NULL DEFAULT GETUTCDATE(),
[updated_utc] DATETIME NOT NULL DEFAULT GETUTCDATE(),
[deleted] BIT NOT NULL DEFAULT 0,
CONSTRAINT uc_login_type_name UNIQUE (name)
);

GO
ALTER TABLE users ADD login_type_code VARCHAR(10) NULL;
ALTER TABLE users ADD CONSTRAINT fk_users_login_type_code FOREIGN KEY (login_type_code) REFERENCES login_types(code); 