DECLARE @id varchar(30) = 'broker-listener';
DECLARE @password nvarchar(250) = 'LeSSYeezW9RrR9FbHnrZgkFWKqx8Yc';

update broker_clients set password = @password where id = @id;