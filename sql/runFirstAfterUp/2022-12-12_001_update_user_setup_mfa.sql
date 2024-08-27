update users 
set setup_mfa = 0
where setup_mfa is null;