create table email_templates(
id uniqueidentifier default newsequentialid() primary key,
name nvarchar(255) not null,
subject nvarchar(255) not null,
html ntext not null,
type_code varchar(25) not null,
created_utc datetime2 not null default getutcdate(),
updated_utc datetime2 not null default getutcdate(),
deleted bit not null default 0,
)