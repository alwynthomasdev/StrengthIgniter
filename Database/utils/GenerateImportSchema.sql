

BEGIN TRY 

BEGIN TRANSACTION trn

DECLARE @recordImportSchemaId INTEGER = 0
--
DECLARE @userId INTEGER = 1
DECLARE @schemaName VARCHAR(500) = 'My Schema'

IF NOT EXISTS (SELECT TOP 1 [RecordImportSchemaId] FROM [RecordImportSchema] WHERE [UserId] = @userId AND [Name] = @schemaName)
 BEGIN

	INSERT INTO [RecordImportSchema]
		([Reference]
		,[UserId]
		,[Name]
		,[Delimiter])
	VALUES
		(NEWID()
		,@userId
		,@schemaName
		,',')

	SET @recordImportSchemaId = SCOPE_IDENTITY()

	INSERT INTO [RecordImportSchemaColumnMap]
		([RecordImportSchemaId]
		,[HeaderName]
		,[ColumnTypeCode])
	VALUES
		(@recordImportSchemaId, 'Exercise', 1),
		(@recordImportSchemaId, 'Date', 2),
		(@recordImportSchemaId, 'Reps', 8),
		(@recordImportSchemaId, 'WeightKg', 3),
		(@recordImportSchemaId, 'BodyweightKg', 5),
		(@recordImportSchemaId, 'RPE', 9)

	INSERT INTO [RecordImportSchemaExerciseMap]
		([RecordImportSchemaId]
		,[ExerciseId]
		,[Text])
	VALUES
		(@recordImportSchemaId, 1, 'Squat'),
		(@recordImportSchemaId, 2, 'PauseSquat'),
		(@recordImportSchemaId, 3, 'SSB'),
		(@recordImportSchemaId, 4, 'HbSquat'),
		(@recordImportSchemaId, 5, 'LegPress'),
		(@recordImportSchemaId, 6, 'PauseSSB'),
		(@recordImportSchemaId, 7, 'FrontSquat'),
		(@recordImportSchemaId, 8, 'Bench'),
		(@recordImportSchemaId, 8, 'SpeedBench'),
		(@recordImportSchemaId, 9, 'DbBench'),
		(@recordImportSchemaId, 10, 'CGBP'),
		(@recordImportSchemaId, 11, 'IncDbBench'),
		(@recordImportSchemaId, 12, '3ctPauseBench'),
		(@recordImportSchemaId, 13, 'WGBP'),
		(@recordImportSchemaId, 14, 'Spoto'),
		(@recordImportSchemaId, 15, 'FeetUp'),
		(@recordImportSchemaId, 16, 'InclineBench'),
		(@recordImportSchemaId, 17, 'Deadlift'),
		(@recordImportSchemaId, 18, 'SLDL'),
		(@recordImportSchemaId, 19, 'RDL'),
		(@recordImportSchemaId, 20, 'DefDeadlift'),
		(@recordImportSchemaId, 21, 'PendlayRow'),
		(@recordImportSchemaId, 22, 'Sumo'),
		(@recordImportSchemaId, 23, 'SSBGM'),
		(@recordImportSchemaId, 24, 'Press')
	
 END
 COMMIT TRANSACTION trn

END TRY  
BEGIN CATCH
	PRINT ERROR_MESSAGE()
	ROLLBACK TRANSACTION trn
END CATCH  

