IF NOT EXISTS (SELECT 1 FROM login_types WHERE code = 'sap-cdc')
INSERT INTO login_types (code, name) VALUES ('sap-cdc','SAP CDC')