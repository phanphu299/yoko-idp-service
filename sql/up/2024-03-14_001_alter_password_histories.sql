IF OBJECT_ID('[fk_password_histories_upn]', 'F') IS NOT NULL 
BEGIN
    ALTER TABLE password_histories DROP CONSTRAINT fk_password_histories_upn;
END

ALTER TABLE password_histories ADD CONSTRAINT fk_password_histories_upn FOREIGN KEY (upn) REFERENCES users(upn) ON DELETE CASCADE;