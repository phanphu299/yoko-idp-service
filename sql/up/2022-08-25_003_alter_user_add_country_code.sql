ALTER TABLE [users] 
ADD country_code VARCHAR(2) NOT NULL 
CONSTRAINT [df_users_country_code]  DEFAULT N'SG'