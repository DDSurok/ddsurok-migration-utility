BEGIN TRAN
	USE proba
	GO
	IF EXISTS (SELECT sys.schemas.name FROM sys.schemas where sys.schemas.name = N'sc_ddsurok')
	BEGIN
		DECLARE @table_name sysname
		DECLARE myCursor CURSOR FOR SELECT sys.tables.name FROM sys.tables, sys.schemas where sys.tables.schema_id = sys.schemas.schema_id and sys.schemas.name = N'sc_ddsurok'
		OPEN myCursor
		FETCH NEXT FROM myCursor
		INTO @table_name
		WHILE @@FETCH_STATUS = 0
		BEGIN
			EXEC('DROP TABLE [sc_ddsurok].[' + @table_name + ']')
			FETCH NEXT FROM myCursor
			INTO @table_name
		END
		CLOSE myCursor
		DEALLOCATE myCursor
		DROP SCHEMA [sc_ddsurok]
		IF @@ERROR!=0
		BEGIN
			SELECT * FROM sys.all_parameters
			ROLLBACK TRAN
		END
		ELSE
		BEGIN
			COMMIT TRAN	
		END
	END
	ELSE
		COMMIT TRAN