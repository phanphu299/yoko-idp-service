DECLARE @temp_client_scopes TABLE
(
    Scope nvarchar(200),
    ClientId int
)

INSERT INTO @temp_client_scopes
SELECT 'asset-media-data', client_id from project_clients WITH(NOLOCK)

INSERT INTO @temp_client_scopes
SELECT 'asset-table-data', client_id from project_clients WITH(NOLOCK)

merge ClientScopes as target_table
using (select * from @temp_client_scopes) as source_table on source_table.ClientId = target_table.ClientId and source_table.Scope = target_table.Scope
when not matched by target then
    insert (Scope,ClientId)
    values (source_table.Scope, source_table.ClientId);