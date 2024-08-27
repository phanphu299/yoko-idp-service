CREATE TABLE dbo.email_attachments
(
    [id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
    [email_template_id] UNIQUEIDENTIFIER NOT NULL,
    [disposition] NVARCHAR(25) NOT NULL,
    [file_path] VARCHAR(MAX) NOT NULL, 
    CONSTRAINT fk_email_attachments_email_template_id FOREIGN KEY(email_template_id) REFERENCES email_templates(id)
)