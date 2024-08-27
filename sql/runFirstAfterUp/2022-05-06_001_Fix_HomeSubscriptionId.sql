update u
set u.tenant_id = s.tenant_id, u.subscription_id = s.subscription_id
from users u
inner join 
(
select u.upn, us.tenant_id, us.subscription_id
from users u
inner join user_subscriptions us on u.upn = us.upn
group by u.upn, us.tenant_id, us.subscription_id
) s on u.upn = s.upn