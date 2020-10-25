IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Reference'
          AND Object_ID = Object_ID(N'RecordImportRow'))
BEGIN
	ALTER TABLE RecordImportRow
	ADD Reference UNIQUEIDENTIFIER NULL
END
GO

UPDATE RecordImportRow
SET Reference = NEWID()
WHERE Reference IS NULL
GO

ALTER TABLE RecordImportRow
ALTER COLUMN Reference UNIQUEIDENTIFIER NOT NULL
GO

IF NOT EXISTS (
	SELECT * 
	FROM sys.indexes 
	WHERE [Name]='UX_Reference' AND OBJECT_ID = OBJECT_ID('RecordImportRow'))
 BEGIN
	CREATE UNIQUE INDEX UX_Reference
	ON RecordImportRow (Reference)
 END
GO