IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'User'
                 AND COLUMN_NAME = 'IsAnonymised')
 BEGIN
	ALTER TABLE [User]
	ADD [IsAnonymised] BIT DEFAULT 0

	ALTER TABLE [User] 
	ADD [AnonymisedDateTimeUtc] DATETIME

 END