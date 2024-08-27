DECLARE @ClientUpdate_tmp TABLE (id int, client_id varchar(50));
DECLARE @ClaimType VARCHAR(250) = 'apiClient';
DECLARE @ClaimValue VARCHAR(250) = 'true';

INSERT INTO @ClientUpdate_tmp (client_id, id)
(SELECT DISTINCT client_roles.client_id, clients.id FROM client_roles WITH (NOLOCK)
INNER JOIN clients WITH (NOLOCK) on CAST(client_roles.client_id as varchar(50)) = clients.ClientId
where tenant_id is not NULL AND clients.Id NOT IN
(
    SELECT ClientId FROM ClientClaims WITH (NOLOCK)
    WHERE 
        ClientClaims.[Type] = @ClaimType
))

INSERT INTO ClientClaims ([Type],[Value],[ClientId])
(SELECT @ClaimType, @ClaimValue, id FROM @ClientUpdate_tmp)