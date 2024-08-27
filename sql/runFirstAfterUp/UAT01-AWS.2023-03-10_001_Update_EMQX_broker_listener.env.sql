DECLARE @id varchar(30) = 'broker-listener';
DECLARE @password nvarchar(250) = 'K6TA7D7qqVp8C5gFftUQQLQ4wWzja8';

update broker_clients set password = @password where id = @id;