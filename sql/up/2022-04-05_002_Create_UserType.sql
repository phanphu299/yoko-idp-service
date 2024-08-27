IF (NOT EXISTS (SELECT 1 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'user_types'))
BEGIN
  CREATE TABLE [user_types] (
    [code] VARCHAR(2) not null PRIMARY KEY,
    [name] NVARCHAR(255) not null,
    [created_utc] DATETIME NOT NULL DEFAULT GETUTCDATE(),
    [updated_utc] DATETIME NOT NULL DEFAULT GETUTCDATE(),
    [deleted] BIT NOT NULL DEFAULT 0,
    CONSTRAINT uc_user_type_name UNIQUE (name)
  );

  insert into user_types(code, name) values ('ad', 'Azure AD')
  insert into user_types(code, name) values ('la', 'Local User')

  ALTER TABLE [users] ADD CONSTRAINT fk_users_user_type_code FOREIGN KEY (user_type_code) REFERENCES user_types(code); 
END


