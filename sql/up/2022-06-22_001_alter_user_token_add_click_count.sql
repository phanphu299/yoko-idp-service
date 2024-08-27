alter table user_tokens add click_count int not null default 0;
alter table user_tokens add max_click_count int not null default 2;