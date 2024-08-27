IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME = 'fk_entity_tags_clients_entity_id_int')
BEGIN
	ALTER TABLE entity_tags 
	ADD CONSTRAINT fk_entity_tags_clients_entity_id_int
	FOREIGN KEY (entity_id_int) 
	REFERENCES Clients (id)
	ON DELETE CASCADE;
END