IF NOT EXISTS (SELECT 1 FROM login_types WHERE code = 'ahi-local')
INSERT INTO login_types (code, name) VALUES ('ahi-local', 'AHI Local')

IF NOT EXISTS (SELECT 1 FROM login_types WHERE code = 'ahi-ad')
INSERT INTO login_types (code, name) VALUES ('ahi-ad', 'AHI Azure AD')

GO
UPDATE users
SET login_type_code = 'ahi-ad'
WHERE user_type_code = 'ad'

UPDATE users
SET login_type_code = 'ahi-local'
WHERE user_type_code != 'ad'

UPDATE users
SET user_type_code = 'la', first_name = 'Local', last_name = 'User'
WHERE user_type_code != 'la'
