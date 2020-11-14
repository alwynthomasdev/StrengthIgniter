IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'UserId'
          AND Object_ID = Object_ID(N'RecordImportSchema'))
BEGIN
	ALTER TABLE RecordImportSchema
	ADD UserId INTEGER NULL
END
GO

DECLARE @SysUserId INTEGER

SELECT @SysUserId = UserId 
FROM [User]
WHERE Reference = '00000000-0000-0000-0000-000000000000'

UPDATE RecordImportSchema
SET UserId = @SysUserId
WHERE UserId IS NULL
GO

ALTER TABLE RecordImportSchema
ALTER COLUMN UserId INTEGER NULL

IF NOT EXISTS(
	SELECT * 
    FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS 
    WHERE CONSTRAINT_NAME ='FK_RecordImportSchema_User')
 BEGIN
	ALTER TABLE RecordImportSchema
	ADD CONSTRAINT FK_RecordImportSchema_User FOREIGN KEY (UserId) references [User] (UserId)
 END
 GO