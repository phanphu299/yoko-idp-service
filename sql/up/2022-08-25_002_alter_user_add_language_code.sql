ALTER TABLE [users] 
ADD language_code VARCHAR(5) NOT NULL 
CONSTRAINT [df_users_language_code]  DEFAULT N'en-US'