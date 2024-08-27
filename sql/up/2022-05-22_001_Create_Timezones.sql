CREATE TABLE timezones (
    id int not null PRIMARY key,
    name NVARCHAR(255) not null,
    offset NVARCHAR(10) not null,
    Description NVARCHAR(255) null
)