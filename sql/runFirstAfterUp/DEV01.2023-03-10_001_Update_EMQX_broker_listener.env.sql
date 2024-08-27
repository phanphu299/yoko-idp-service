DECLARE @id varchar(30) = 'broker-listener';
DECLARE @password nvarchar(250) = 'Fe5XPfrJdD5qAZY5JTPWJP8Jr8GXMEFx';

update broker_clients set password = @password where id = @id;