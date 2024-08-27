declare @resource nvarchar(255) = 'reporting-data'
declare @resourceSecret nvarchar(255) = 'BAh+RI44vVGBbbKwyC0y8irAUx6kaPoQsohX9aDR4HM=' -- UxQ5SHGkshgm9ALKWu4JMJ9TsELdBAKqT9
declare @resourceId int = (select Id from ApiResources with(nolock) where name = @resource)

-- there is an existing reporting-data secret without pre-hashed password, need to update
if exists (select 1 from ApiSecrets with(nolock) where ApiResourceId = @resourceId and Value <> @resourceSecret)
begin
	update ApiSecrets set Value = @resourceSecret where ApiResourceId = @resourceId
end