IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'UserId'
          AND Object_ID = Object_ID(N'RecordImportSchema'))
BEGIN
	ALTER TABLE RecordImportSchema
	ADD UserId INTEGER
END

