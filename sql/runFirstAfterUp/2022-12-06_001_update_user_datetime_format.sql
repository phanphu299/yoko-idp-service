UPDATE users SET date_time_format = REPLACE(date_time_format COLLATE SQL_Latin1_General_CP1_CS_AS,' HH:mm:ss.fff', ' (HH:mm:ss.fff)')
WHERE date_time_format COLLATE SQL_Latin1_General_CP1_CI_AS like '% HH:mm:ss.fff'
UPDATE users SET date_time_format = REPLACE(date_time_format COLLATE SQL_Latin1_General_CP1_CS_AS,' HH:mm:ss.fff tt', ' (HH:mm:ss.fff tt)')
WHERE date_time_format COLLATE SQL_Latin1_General_CP1_CI_AS like '% HH:mm:ss.fff tt'

UPDATE users SET display_date_time_format = REPLACE(display_date_time_format COLLATE SQL_Latin1_General_CP1_CS_AS,' HH:mm:ss.SSS', ' (HH:mm:ss.SSS)')
WHERE display_date_time_format COLLATE SQL_Latin1_General_CP1_CI_AS like '% HH:mm:ss.SSS'
UPDATE users SET display_date_time_format = REPLACE(display_date_time_format COLLATE SQL_Latin1_General_CP1_CS_AS,' HH:mm:ss.SSS A', ' (HH:mm:ss.SSS A)')
WHERE display_date_time_format COLLATE SQL_Latin1_General_CP1_CI_AS like '% HH:mm:ss.SSS A'