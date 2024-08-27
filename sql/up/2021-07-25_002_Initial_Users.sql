Create table users(
upn nvarchar(255) not null primary key,
email nvarchar(255) not null, 
Password nvarchar(2048),
first_name nvarchar(255),
last_name nvarchar(255),
required_change_password bit not null default 1
)
GO
