CREATE TABLE user_tokens
(
    id INT IDENTITY(1,1) primary key,
    user_name NVARCHAR(255) NOT NULL,
    token_key NVARCHAR(255) NOT NULL,
    token_type VARCHAR(255) NOT NULL,
    created_date DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    expired_date DATETIME2 NOT NULL DEFAULT DATEADD(MINUTE,60,GETUTCDATE()),
    redirect_url NVARCHAR(2000),
    deleted BIT NOT NULL DEFAULT 0
)
