DECLARE @id varchar(30) = 'broker-listener';
DECLARE @password nvarchar(250) = '6RfhUgJys3G6ct3TDa5SJpArQtp5y2';

update broker_clients set password = @password where id = @id;