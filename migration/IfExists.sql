BEGIN TRAN;
	IF EXISTS (SELECT * FROM sys.triggers WHERE parent_class_desc = 'DATABASE' AND name = N'tr_db')
	DISABLE TRIGGER [tr_db] ON DATABASE;
	IF  EXISTS (SELECT * FROM sys.triggers WHERE parent_class_desc = 'DATABASE' AND name = N'tr_db')DROP TRIGGER [tr_db] ON DATABASE;
	IF EXISTS (SELECT sys.schemas.name FROM sys.schemas where sys.schemas.name = N'dds')
	BEGIN
		DECLARE @table_name sysname;
		DECLARE myCursor CURSOR FOR SELECT sys.tables.name FROM sys.tables, sys.schemas where sys.tables.schema_id = sys.schemas.schema_id and sys.schemas.name = N'dds';
		OPEN myCursor;
		FETCH NEXT FROM myCursor INTO @table_name;
		WHILE @@FETCH_STATUS = 0
		BEGIN
			EXEC('DROP TABLE [dds].[' + @table_name + ']');
			FETCH NEXT FROM myCursor
			INTO @table_name;
		END;
		CLOSE myCursor;
		DEALLOCATE myCursor;
		DROP SCHEMA [dds];
		IF @@ERROR!=0
		BEGIN
			SELECT * FROM sys.all_parameters;
			ROLLBACK TRAN;
		END;
		ELSE
		BEGIN
			COMMIT TRAN;
		END;
	END;
	ELSE
		COMMIT TRAN;