IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Reference'
          AND Object_ID = Object_ID(N'Record'))
BEGIN
	ALTER TABLE Record
	ADD Reference UNIQUEIDENTIFIER NULL
END
GO

UPDATE Record
SET Reference = NEWID()
WHERE Reference IS NULL
GO

ALTER TABLE Record
ALTER COLUMN Reference UNIQUEIDENTIFIER NOT NULL
GO

IF NOT EXISTS (
	SELECT * 
	FROM sys.indexes 
	WHERE [Name]='UX_Reference' AND OBJECT_ID = OBJECT_ID('Record'))
 BEGIN
	CREATE UNIQUE INDEX UX_Reference
	ON Record (Reference)
 END
GO