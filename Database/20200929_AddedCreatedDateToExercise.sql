IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'CreatedDateTimeUtc'
          AND Object_ID = Object_ID(N'Exercise'))
BEGIN
	ALTER TABLE Exercise
	ADD CreatedDateTimeUtc DATETIME NULL
END
GO

UPDATE Exercise
SET CreatedDateTimeUtc = GETUTCDATE()
WHERE CreatedDateTimeUtc IS NULL
GO

ALTER TABLE Exercise
ALTER COLUMN CreatedDateTimeUtc DATETIME NOT NULL
GO