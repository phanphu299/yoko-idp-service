IF COL_LENGTH('email_templates', 'attachment_disposition') IS NOT NULL
BEGIN
    ALTER TABLE dbo.email_templates DROP COLUMN attachment_disposition
END
IF COL_LENGTH('email_templates', 'attachment_files') IS NOT NULL
BEGIN
    ALTER TABLE dbo.email_templates DROP COLUMN attachment_files
END