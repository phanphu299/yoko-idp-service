declare @default_date_time_format varchar(50) = 'yyyy-MM-dd (HH:mm:ss.fff)';
declare @default_display_date_time_format varchar(50) = 'yyyy-MM-DD (HH:mm:ss.SSS)';

update users set date_time_format = @default_date_time_format, display_date_time_format = @default_display_date_time_format where date_time_format = 'd-M-yy (HH:mm:ss)' and display_date_time_format = 'D-M-yy (HH:mm:ss)';
update users set date_time_format = @default_date_time_format, display_date_time_format = @default_display_date_time_format where date_time_format = 'd-M-yy (hh:mm:ss tt)' and display_date_time_format = 'D-M-yy (hh:mm:ss A)';
update users set date_time_format = @default_date_time_format, display_date_time_format = @default_display_date_time_format where date_time_format = 'd/M/yy (HH:mm:ss)' and display_date_time_format = 'D/M/yy (HH:mm:ss)';
update users set date_time_format = @default_date_time_format, display_date_time_format = @default_display_date_time_format where date_time_format = 'd/M/yy (hh:mm:ss tt)' and display_date_time_format = 'D/M/yy (hh:mm:ss A)';