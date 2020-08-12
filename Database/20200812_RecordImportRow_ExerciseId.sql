IF NOT EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'RecordImportRow'
                 AND COLUMN_NAME = 'ExerciseId')
 BEGIN
	ALTER TABLE [RecordImportRow]
	ADD [ExerciseId] INTEGER NULL
 END