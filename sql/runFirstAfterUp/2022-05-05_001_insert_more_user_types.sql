if not exists (select 1 from user_types where code = 'ga')
insert into user_types (code, name) values ('ga', 'Guest User')