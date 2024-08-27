DECLARE @id varchar(30) = 'broker-listener';
DECLARE @password nvarchar(250) = '3jj4PA7AbJURtnrRVkMbgLEaeE82HV';

update broker_clients set password = @password where id = @id;