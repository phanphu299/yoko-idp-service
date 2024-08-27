DECLARE @id varchar(30) = 'broker-listener';
DECLARE @password nvarchar(250) = 'NARSLH2cLaP3Qm9fzYZcbW4VS6VGET';

update broker_clients set password = @password where id = @id;