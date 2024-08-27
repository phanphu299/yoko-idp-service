INSERT INTO password_histories (upn, [password])
(SELECT upn, password FROM users where [password] IS NOT NULL)