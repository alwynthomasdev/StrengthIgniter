

BEGIN TRY 

BEGIN TRANSACTION trn

DECLARE @recordImportSchemaId INTEGER = 0
--
DECLARE @userId INTEGER = 10
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
		(@recordImportSchemaId, 73, 'Squat'),
		(@recordImportSchemaId, 74, 'PauseSquat'),
		(@recordImportSchemaId, 75, 'SSB'),
		(@recordImportSchemaId, 76, 'HbSquat'),
		(@recordImportSchemaId, 77, 'LegPress'),
		(@recordImportSchemaId, 78, 'PauseSSB'),
		(@recordImportSchemaId, 79, 'FrontSquat'),
		(@recordImportSchemaId, 80, 'Bench'),
		(@recordImportSchemaId, 80, 'SpeedBench'),
		(@recordImportSchemaId, 81, 'DbBench'),
		(@recordImportSchemaId, 82, 'CGBP'),
		(@recordImportSchemaId, 83, 'IncDbBench'),
		(@recordImportSchemaId, 84, '3ctPauseBench'),
		(@recordImportSchemaId, 85, 'WGBP'),
		(@recordImportSchemaId, 86, 'Spoto'),
		(@recordImportSchemaId, 87, 'FeetUp'),
		(@recordImportSchemaId, 88, 'InclineBench'),
		(@recordImportSchemaId, 89, 'Deadlift'),
		(@recordImportSchemaId, 90, 'SLDL'),
		(@recordImportSchemaId, 91, 'RDL'),
		(@recordImportSchemaId, 92, 'DefDeadlift'),
		(@recordImportSchemaId, 93, 'PendlayRow'),
		(@recordImportSchemaId, 94, 'Sumo'),
		(@recordImportSchemaId, 95, 'SSBGM'),
		(@recordImportSchemaId, 96, 'Press')
	
 END
 COMMIT TRANSACTION trn

END TRY  
BEGIN CATCH
	PRINT ERROR_MESSAGE()
	ROLLBACK TRANSACTION trn
END CATCH  

