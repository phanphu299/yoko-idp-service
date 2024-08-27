IF NOT EXISTS(SELECT * FROM sys.indexes WHERE Name = 'idx_entity_tags_tag_id_entity_type_entity_id_varchar')
BEGIN
	CREATE UNIQUE INDEX idx_entity_tags_tag_id_entity_type_entity_id_varchar ON entity_tags(tag_id, entity_type, entity_id_varchar) WHERE entity_id_varchar IS NOT NULL;
END

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE Name = 'idx_entity_tags_tag_id_entity_type_entity_id_int')
BEGIN
	CREATE UNIQUE INDEX idx_entity_tags_tag_id_entity_type_entity_id_int ON entity_tags(tag_id, entity_type, entity_id_int) WHERE entity_id_int IS NOT NULL;
END

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE Name = 'idx_entity_tags_tag_id_entity_type_entity_id_long')
BEGIN
	CREATE UNIQUE INDEX idx_entity_tags_tag_id_entity_type_entity_id_long ON entity_tags(tag_id, entity_type, entity_id_long) WHERE entity_id_long IS NOT NULL;
END

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE Name = 'idx_entity_tags_tag_id_entity_type_entity_id_uuid')
BEGIN
	CREATE UNIQUE INDEX idx_entity_tags_tag_id_entity_type_entity_id_uuid ON entity_tags(tag_id, entity_type, entity_id_uuid) WHERE entity_id_uuid IS NOT NULL;
END