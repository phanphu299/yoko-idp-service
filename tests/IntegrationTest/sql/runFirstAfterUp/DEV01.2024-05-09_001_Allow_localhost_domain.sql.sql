;INSERT INTO [identity].dbo.ClientRedirectUris(RedirectUri, ClientId)
select REPLACE(c.RedirectUri,'http://localhost', 'http://ui.local'), ClientId
from ClientRedirectUris c
where RedirectUri like '%localhost%';


;INSERT INTO [identity].dbo.ClientPostLogoutRedirectUris(PostLogoutRedirectUri, ClientId)
select REPLACE(c.PostLogoutRedirectUri,'http://localhost', 'http://ui.local'), ClientId 
from ClientPostLogoutRedirectUris c
where PostLogoutRedirectUri like '%localhost%';
