DECLARE @id int
DECLARE @clientDescription nvarchar(255) = N'YDX Service Account description'
DECLARE @saClientId nvarchar(255) = 'D28710F2-F142-4CBD-A4E9-9D0A09B9ECF5'
Declare @updatedClientId nvarchar(255)
select @Id = Id, @updatedClientId = clientId
                  from clients with(nolock)
                    where [Description] = @clientDescription
update clients 
set clientId = @saClientId
where id = @id;

update ClientSecrets
set [Value] = 'Kiwfhbpp3Aj2djKPuiBIpZQm0+YqJ2Ehzr81MqnQAJc=' -- D34BtxpaKSkfcKYCvxLbgKTdV8FUfRTHGPrKLawL
where clientId = @id

update client_roles
set client_id = @saClientId
where client_id = @updatedClientId

DELETE FROM ClientClaims WHERE ClientId = @Id

INSERT INTO ClientClaims (Type,Value,ClientId)
VALUES
('tenantId','0779433e-f36b-1410-8650-00f91313348c',@Id)
,('subscriptionId','0e79433e-f36b-1410-8650-00f91313348c',@Id)
,('projectId','34e5ee62-429c-4724-b3d0-3891bd0a08c9',@Id)
,('allowHeader','true',@Id)
-- trigger