CREATE DATABASE [StrengthIgniter]
GO

USE [StrengthIgniter]
GO
/****** Object:  UserDefinedTableType [dbo].[uttAuditEventItem]    Script Date: 19/01/2021 22:31:52 ******/
CREATE TYPE [dbo].[uttAuditEventItem] AS TABLE(
	[Key] [varchar](500) NULL,
	[Value] [varchar](1000) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[uttKeyValuePair]    Script Date: 19/01/2021 22:31:52 ******/
CREATE TYPE [dbo].[uttKeyValuePair] AS TABLE(
	[Key] [varchar](500) NULL,
	[Value] [varchar](1000) NULL
)
GO
/****** Object:  Table [dbo].[AuditEvent]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditEvent](
	[AuditEventId] [int] IDENTITY(1,1) NOT NULL,
	[AuditEventDateTimeUtc] [datetime] NOT NULL,
	[EventType] [varchar](100) NULL,
	[Details] [varchar](1000) NULL,
	[RelatedServiceName] [varchar](100) NULL,
	[RelatedUserId] [int] NULL,
	[RelatedAuditEventId] [int] NULL,
 CONSTRAINT [PK_AuditEvent] PRIMARY KEY CLUSTERED 
(
	[AuditEventId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditEventItem]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditEventItem](
	[AuditEventItemId] [int] IDENTITY(1,1) NOT NULL,
	[AuditEventId] [int] NOT NULL,
	[Key] [varchar](500) NOT NULL,
	[Value] [varchar](1000) NOT NULL,
 CONSTRAINT [PK_AuditEventItem] PRIMARY KEY CLUSTERED 
(
	[AuditEventItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Exercise]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Exercise](
	[ExerciseId] [int] IDENTITY(1,1) NOT NULL,
	[Reference] [uniqueidentifier] NOT NULL,
	[Name] [varchar](500) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[DeletedDateTimeUtc] [datetime] NULL,
	[UserId] [int] NULL,
	[CreatedDateTimeUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_Exercise] PRIMARY KEY CLUSTERED 
(
	[ExerciseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Mesocycle]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Mesocycle](
	[MesocycleId] [int] IDENTITY(1,1) NOT NULL,
	[Reference] [uniqueidentifier] NOT NULL,
	[UserId] [int] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[Notes] [varchar](1000) NULL,
	[Description] [varchar](500) NULL,
 CONSTRAINT [PK_Mesocycle] PRIMARY KEY CLUSTERED 
(
	[MesocycleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Microcycle]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Microcycle](
	[MicrocycleId] [int] IDENTITY(1,1) NOT NULL,
	[Reference] [uniqueidentifier] NOT NULL,
	[UserId] [int] NOT NULL,
	[MesocycleId] [int] NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[Notes] [varchar](1000) NULL,
	[Description] [varchar](500) NULL,
 CONSTRAINT [PK_Microcycle] PRIMARY KEY CLUSTERED 
(
	[MicrocycleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Record]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Record](
	[RecordId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ExerciseId] [int] NOT NULL,
	[Date] [date] NOT NULL,
	[Reps] [int] NOT NULL,
	[WeightKg] [decimal](6, 2) NULL,
	[BodyweightKg] [decimal](6, 2) NULL,
	[RPE] [decimal](3, 1) NULL,
	[Notes] [varchar](1000) NULL,
	[CreatedDateTimeUtc] [datetime] NULL,
	[Reference] [uniqueidentifier] NOT NULL,
	[SetReference] [uniqueidentifier] NULL,
	[SetOrdinal] [int] NULL,
	[MicrocycleId] [int] NULL,
	[MesocycleId] [int] NULL,
 CONSTRAINT [PK_Record] PRIMARY KEY CLUSTERED 
(
	[RecordId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SecurityQuestion]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SecurityQuestion](
	[SecurityQuestionId] [int] IDENTITY(1,1) NOT NULL,
	[QuestionText] [varchar](2000) NOT NULL,
 CONSTRAINT [PK_SecurityQuestion] PRIMARY KEY CLUSTERED 
(
	[SecurityQuestionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Reference] [uniqueidentifier] NOT NULL,
	[Name] [varchar](500) NOT NULL,
	[EmailAddress] [varchar](500) NOT NULL,
	[PasswordHash] [nvarchar](2000) NOT NULL,
	[UserTypeCode] [int] NOT NULL,
	[LastLoginDateTimeUtc] [datetime] NULL,
	[LockoutEndDateTimeUtc] [datetime] NULL,
	[FailedLoginAttemptCount] [int] NULL,
	[IsRegistrationValidated] [bit] NOT NULL,
	[RegisteredDateTimeUtc] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[DeletedDateTimeUtc] [datetime] NULL,
	[IsAnonymised] [bit] NULL,
	[AnonymisedDateTimeUtc] [datetime] NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserSecurityQuestion]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserSecurityQuestion](
	[UserSecurityQuestionId] [int] IDENTITY(1,1) NOT NULL,
	[Reference] [uniqueidentifier] NOT NULL,
	[UserId] [int] NOT NULL,
	[QuestionText] [varchar](2000) NOT NULL,
	[AnswerHash] [nvarchar](2000) NOT NULL,
	[FailedAnswerAttemptCount] [int] NULL,
 CONSTRAINT [PK_UserSecurityQuestionAnswer] PRIMARY KEY CLUSTERED 
(
	[UserSecurityQuestionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserToken]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserToken](
	[UserTokenId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Reference] [uniqueidentifier] NOT NULL,
	[PurposeCode] [varchar](100) NOT NULL,
	[IssuedDateTimeUtc] [datetime] NOT NULL,
	[ExpiryDateTimeUtc] [datetime] NOT NULL,
 CONSTRAINT [PK_UserToken] PRIMARY KEY CLUSTERED 
(
	[UserTokenId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Exercise] ADD  CONSTRAINT [DF_Exercise_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_IsRegistrationValidated]  DEFAULT ((0)) FOR [IsRegistrationValidated]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[User] ADD  DEFAULT ((0)) FOR [IsAnonymised]
GO
ALTER TABLE [dbo].[AuditEventItem]  WITH CHECK ADD  CONSTRAINT [FK_AuditEventItem_AuditEvent] FOREIGN KEY([AuditEventId])
REFERENCES [dbo].[AuditEvent] ([AuditEventId])
GO
ALTER TABLE [dbo].[AuditEventItem] CHECK CONSTRAINT [FK_AuditEventItem_AuditEvent]
GO
ALTER TABLE [dbo].[Exercise]  WITH CHECK ADD  CONSTRAINT [FK_Exercise_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Exercise] CHECK CONSTRAINT [FK_Exercise_User]
GO
ALTER TABLE [dbo].[Mesocycle]  WITH CHECK ADD  CONSTRAINT [FK_Mesocycle_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Mesocycle] CHECK CONSTRAINT [FK_Mesocycle_User]
GO
ALTER TABLE [dbo].[Microcycle]  WITH CHECK ADD  CONSTRAINT [FK_Microcycle_Mesocycle] FOREIGN KEY([MesocycleId])
REFERENCES [dbo].[Mesocycle] ([MesocycleId])
GO
ALTER TABLE [dbo].[Microcycle] CHECK CONSTRAINT [FK_Microcycle_Mesocycle]
GO
ALTER TABLE [dbo].[Microcycle]  WITH CHECK ADD  CONSTRAINT [FK_Microcycle_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Microcycle] CHECK CONSTRAINT [FK_Microcycle_User]
GO
ALTER TABLE [dbo].[Record]  WITH CHECK ADD  CONSTRAINT [FK_Record_Exercise] FOREIGN KEY([ExerciseId])
REFERENCES [dbo].[Exercise] ([ExerciseId])
GO
ALTER TABLE [dbo].[Record] CHECK CONSTRAINT [FK_Record_Exercise]
GO
ALTER TABLE [dbo].[Record]  WITH CHECK ADD  CONSTRAINT [FK_Record_Mesocycle] FOREIGN KEY([MesocycleId])
REFERENCES [dbo].[Mesocycle] ([MesocycleId])
GO
ALTER TABLE [dbo].[Record] CHECK CONSTRAINT [FK_Record_Mesocycle]
GO
ALTER TABLE [dbo].[Record]  WITH CHECK ADD  CONSTRAINT [FK_Record_Microcycle] FOREIGN KEY([MicrocycleId])
REFERENCES [dbo].[Microcycle] ([MicrocycleId])
GO
ALTER TABLE [dbo].[Record] CHECK CONSTRAINT [FK_Record_Microcycle]
GO
ALTER TABLE [dbo].[Record]  WITH CHECK ADD  CONSTRAINT [FK_Record_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Record] CHECK CONSTRAINT [FK_Record_User]
GO
ALTER TABLE [dbo].[UserSecurityQuestion]  WITH CHECK ADD  CONSTRAINT [FK_UserSecurityQuestionAnswer_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[UserSecurityQuestion] CHECK CONSTRAINT [FK_UserSecurityQuestionAnswer_User]
GO
ALTER TABLE [dbo].[UserToken]  WITH CHECK ADD  CONSTRAINT [FK_UserToken_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[UserToken] CHECK CONSTRAINT [FK_UserToken_User]
GO
/****** Object:  StoredProcedure [dbo].[spAuditEventInsert]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spAuditEventInsert]
	@EventType VARCHAR(100),
	@Details VARCHAR(1000),
	@RelatedServiceName VARCHAR(100),
	@UserReference UNIQUEIDENTIFIER,
	@RelatedAuditEventId INTEGER,
	@AuditEventItems uttKeyValuePair READONLY
AS
BEGIN

	DECLARE @AuditEventID INTEGER
	DECLARE @UserID INTEGER

	SELECT	@UserID = u.UserId
	FROM	[User] u
	WHERE	u.Reference = @UserReference

	INSERT INTO AuditEvent
		(AuditEventDateTimeUtc
		,EventType
		,Details
		,RelatedServiceName
		,RelatedUserId
		,RelatedAuditEventId)
	VALUES
		(GETUTCDATE()
		,@EventType
		,@Details
		,@RelatedServiceName
		,@UserID
		,@RelatedAuditEventId);

	SELECT @AuditEventID = SCOPE_IDENTITY()

	INSERT INTO AuditEventItem
			(AuditEventId
			,[Key]
			,[Value])
	SELECT	@AuditEventID,
			[Key],
			[Value]
	FROM	@AuditEventItems

	SELECT @AuditEventID

END
GO
/****** Object:  StoredProcedure [dbo].[spExerciseDelete]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spExerciseDelete]
	@ExerciseId		INTEGER,
	@UserReference	UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;

	DELETE		ex
	FROM		Exercise ex
	INNER JOIN	[User] u		ON	ex.UserId = u.UserId
	WHERE		ex.ExerciseId	= @ExerciseId
	AND			u.Reference		= @UserReference
	
END
GO
/****** Object:  StoredProcedure [dbo].[spExerciseFilter]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spExerciseFilter]
	@UserReference	UNIQUEIDENTIFIER,
	@SearchString	VARCHAR(500)	= NULL,
	@Offset			INTEGER			= NULL,
	@Fetch			INTEGER			= NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @SysUserReference UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000000'

	IF @Offset IS NOT NULL AND @Fetch IS NOT NULL
	 BEGIN

		SELECT		ex.ExerciseId,
					ex.Reference,
					u.Reference AS UserReference,
					ex.[Name]
		FROM		Exercise ex
		INNER JOIN	[User] u ON ex.UserId = u.UserId
		WHERE		(u.Reference = @UserReference OR u.Reference = @SysUserReference)
		AND			(ex.[Name] LIKE '%'+@SearchString+'%' OR @SearchString IS NULL)
		AND			ex.IsDeleted = 0
		ORDER BY	ex.[Name]
		OFFSET		@Offset ROWS FETCH NEXT @Fetch ROWS ONLY

	 END
	ELSE IF @Fetch IS NOT NULL AND @Offset IS NULL
	 BEGIN

		SELECT TOP	(@Fetch)	
					ex.ExerciseId,
					ex.Reference,
					u.Reference AS UserReference,
					ex.[Name]
		FROM		Exercise ex
		INNER JOIN	[User] u ON ex.UserId = u.UserId
		WHERE		(u.Reference = @UserReference OR u.Reference = @SysUserReference)
		AND			(ex.[Name] LIKE '%'+@SearchString+'%' OR @SearchString IS NULL)
		AND			ex.IsDeleted = 0
		ORDER BY	ex.[Name]

	 END
	
	SELECT		COUNT(ex.ExerciseId) [Count]
	FROM		Exercise ex
    INNER JOIN	[User] u ON ex.UserId = u.UserId
	WHERE		(u.Reference = @UserReference OR u.Reference = @SysUserReference)
	AND			(ex.[Name] LIKE '%'+@SearchString+'%' OR @SearchString IS NULL)
	AND			ex.IsDeleted = 0
    
END
GO
/****** Object:  StoredProcedure [dbo].[spExerciseInsert]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spExerciseInsert]
	@Reference		UNIQUEIDENTIFIER,
	@UserReference	UNIQUEIDENTIFIER,
	@Name			VARCHAR(500)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @UserId INTEGER

	SELECT	@UserId = UserId 
	FROM	[User] 
	WHERE	Reference = @UserReference

	INSERT INTO [dbo].[Exercise]
		(Reference
		,[Name]
		,UserId
		,CreatedDateTimeUtc)
	VALUES
		(@Reference
		,@Name
		,@UserId
		,GETUTCDATE())

	SELECT SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[spExerciseSelectById]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spExerciseSelectById]
	@ExerciseId		INTEGER,
	@UserReference	UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @SysUserReference UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000000'

	SELECT TOP 1
		e.ExerciseId,
		e.Reference,
		u.Reference AS UserReference,
		e.[Name]

	FROM		Exercise e
    INNER JOIN	[User] u ON e.UserId = u.UserId

	WHERE		e.ExerciseId	= @ExerciseId 
	AND			(u.Reference	= @UserReference OR u.Reference = @SysUserReference) 
	AND			e.IsDeleted		= 0

END
GO
/****** Object:  StoredProcedure [dbo].[spExerciseUpdate]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spExerciseUpdate]
	@ExerciseId		INTEGER,
	@UserReference	UNIQUEIDENTIFIER,
	@Name			VARCHAR(500)
AS
BEGIN
	SET NOCOUNT ON;
    UPDATE		Exercise
	SET			Exercise.[Name] = @Name
	FROM		Exercise e
	INNER JOIN	[User] u ON e.UserId = u.UserId
	WHERE		e.ExerciseId	= @ExerciseId
	AND			u.Reference		= @UserReference
END
GO
/****** Object:  StoredProcedure [dbo].[spMesocycleDelete]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spMesocycleDelete]
	@MesocycleId		INTEGER,
	@UserReference		UNIQUEIDENTIFIER,
	@DeleteMicrocycles	BIT	= 0,
	@DeleteRecords		BIT	= 0
AS
BEGIN
	SET NOCOUNT ON;

	IF @DeleteRecords = 1
	 BEGIN

		-- delete records by mesocycle id
		DELETE		r
		FROM		Record r
		INNER JOIN	[User] u		ON	r.UserId = u.UserId
		WHERE		r.MesocycleId	= @MesocycleId
		AND			u.Reference		= @UserReference 

	 END
	ELSE
	 BEGIN
		
		-- remove references of mesocycle from records
		UPDATE		Record
		SET			MesocycleId		= NULL
		FROM		Record r
		INNER JOIN	[User] u		ON	r.UserId = u.UserId
		WHERE		r.MesocycleId	= @MesocycleId
		AND			u.Reference		= @UserReference 

	 END

	IF @DeleteMicrocycles = 1
	 BEGIN

		-- delete microcycle by mesocycle id
		DELETE		m
		FROM		Microcycle m
		INNER JOIN	[User] u		ON	m.UserId = u.UserId
		WHERE		m.MesocycleId	= @MesocycleId
		AND			u.Reference		= @UserReference 

	 END
	ELSE
	 BEGIN

		-- remove references of mesocycle from microcycles
		UPDATE		Microcycle
		SET			MesocycleId		= NULL
		FROM		Microcycle m
		INNER JOIN	[User] u		ON	m.UserId = u.UserId
		WHERE		m.MesocycleId	= @MesocycleId
		AND			u.Reference		= @UserReference 

	 END

	-- delete the mesocycle
	DELETE		m
	FROM		Mesocycle m
	INNER JOIN	[User] u		ON	m.UserId = u.UserId
	WHERE		m.MesocycleId	= @MesocycleId
	AND			u.Reference		= @UserReference 

END
GO
/****** Object:  StoredProcedure [dbo].[spMesocycleFilter]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spMesocycleFilter]
	@UserReference	UNIQUEIDENTIFIER,
	@SearchString	VARCHAR(500)	= NULL,
	@StartDate		DATETIME		= NULL,
	@EndDate		DATETIME		= NULL,
	@Offset			INTEGER			= NULL,
	@Fetch			INTEGER			= NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF @Offset IS NOT NULL AND @Fetch IS NOT NULL
	 BEGIN
		
		SELECT		m.MesocycleId,
					m.Reference,
					m.StartDate,
					m.EndDate,
					m.[Description],
					m.Notes
		FROM		Mesocycle m
		INNER JOIN	[User] u	ON m.UserId = u.UserId
		WHERE		u.Reference = @UserReference
		AND			(m.[Description] LIKE '%'+@SearchString+'%' OR @SearchString IS NULL)
		AND			(
						-- Date Range Filter...
							m.StartDate BETWEEN @StartDate AND @EndDate
						OR	m.EndDate	BETWEEN @StartDate AND @EndDate
						OR (@StartDate IS NULL OR @EndDate IS NULL)
					)
		ORDER BY	m.StartDate DESC
		OFFSET		@Offset ROWS FETCH NEXT @Fetch ROWS ONLY

	 END
	ELSE
	 BEGIN
		
		SELECT TOP	(@Fetch)
					m.MesocycleId,
					m.Reference,
					m.StartDate,
					m.EndDate,
					m.[Description],
					m.Notes
		FROM		Mesocycle m
		INNER JOIN	[User] u	ON m.UserId = u.UserId
		WHERE		u.Reference = @UserReference
		AND			(m.[Description] LIKE '%'+@SearchString+'%' OR @SearchString IS NULL)
		AND			(
						-- Date Range Filter...
							m.StartDate BETWEEN @StartDate AND @EndDate
						OR	m.EndDate	BETWEEN @StartDate AND @EndDate
						OR (@StartDate IS NULL OR @EndDate IS NULL)
					)
		ORDER BY	m.StartDate DESC

	 END
	
	SELECT		COUNT(m.MesocycleId) [Count]
	FROM		Mesocycle m
	INNER JOIN	[User] u	ON m.UserId = u.UserId
	WHERE		u.Reference = @UserReference
	AND			(m.[Description] LIKE '%'+@SearchString+'%' OR @SearchString IS NULL)
	AND			(
					-- Date Range Filter...
						m.StartDate BETWEEN @StartDate AND @EndDate
					OR	m.EndDate	BETWEEN @StartDate AND @EndDate
					OR (@StartDate IS NULL OR @EndDate IS NULL)
				)

END
GO
/****** Object:  StoredProcedure [dbo].[spMesocycleInsert]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spMesocycleInsert]
	@Reference		UNIQUEIDENTIFIER,
	@UserReference	UNIQUEIDENTIFIER,
	@StartDate		DATETIME,
	@EndDate		DATETIME		= NULL,
	@Description	VARCHAR(500)	= NULL,
	@Notes			VARCHAR(1000)	= NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @UserId INTEGER

	SELECT	@UserId = UserId 
	FROM	[User] 
	WHERE	Reference = @UserReference
	
	INSERT INTO Mesocycle
		(Reference
		,UserId
		,StartDate
		,EndDate
		,[Description]
		,Notes)
	VALUES
		(@Reference
		,@UserId
		,@StartDate
		,@EndDate
		,@Description
		,@Notes)

	SELECT SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[spMesocycleSelectById]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spMesocycleSelectById]
	@MesocycleId	INTEGER,
	@UserReference	UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;

	SELECT TOP	(1)		
				m.MesocycleId,
				m.Reference,
				m.StartDate,
				m.EndDate,
				m.[Description],
				m.Notes
	FROM		Mesocycle m
	INNER JOIN	[User] u	ON m.UserId = u.UserId
	WHERE		u.Reference = @UserReference
	AND			m.MesocycleId = @MesocycleId

END
GO
/****** Object:  StoredProcedure [dbo].[spMesocycleUpdate]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spMesocycleUpdate]
	@MesocycleId	INTEGER,
	@Reference		UNIQUEIDENTIFIER,
	@UserReference	UNIQUEIDENTIFIER,
	@StartDate		DATETIME,
	@EndDate		DATETIME		= NULL,
	@Description	VARCHAR(500)	= NULL,
	@Notes			VARCHAR(1000)	= NULL
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE		Mesocycle
	SET			StartDate		= @StartDate,
				EndDate			= @EndDate,
				[Description]	= @Description,
				Notes			= @Notes
	FROM		Mesocycle m
	INNER JOIN	[User] u		ON m.UserId = u.UserId
	WHERE		m.MesocycleId	= @MesocycleId
	AND			u.Reference		= @UserReference

END
GO
/****** Object:  StoredProcedure [dbo].[spMicrocycleFilter]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spMicrocycleFilter]
	@UserReference	UNIQUEIDENTIFIER,
	@SearchString	VARCHAR(500)	= NULL,
	@StartDate		DATETIME		= NULL,
	@EndDate		DATETIME		= NULL,
	@Offset			INTEGER			= NULL,
	@Fetch			INTEGER			= NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF @Offset IS NOT NULL AND @Fetch IS NOT NULL
	 BEGIN
		
		SELECT		m.MicrocycleId,
					m.Reference,
					m.StartDate,
					m.EndDate,
					m.[Description],
					m.Notes,
					m.MesocycleId
		FROM		Microcycle m
		INNER JOIN	[User] u	ON m.UserId = u.UserId
		WHERE		u.Reference = @UserReference
		AND			(m.[Description] LIKE '%'+@SearchString+'%' OR @SearchString IS NULL)
		AND			(
						-- Date Range Filter...
							m.StartDate BETWEEN @StartDate AND @EndDate
						OR	m.EndDate	BETWEEN @StartDate AND @EndDate
						OR (@StartDate IS NULL OR @EndDate IS NULL)
					)
		ORDER BY	m.StartDate DESC
		OFFSET		@Offset ROWS FETCH NEXT @Fetch ROWS ONLY

	 END
	ELSE
	 BEGIN
		
		SELECT		m.MicrocycleId,
					m.Reference,
					m.StartDate,
					m.EndDate,
					m.[Description],
					m.Notes,
					m.MesocycleId
		FROM		Microcycle m
		INNER JOIN	[User] u	ON m.UserId = u.UserId
		WHERE		u.Reference = @UserReference
		AND			(m.[Description] LIKE '%'+@SearchString+'%' OR @SearchString IS NULL)
		AND			(
						-- Date Range Filter...
							m.StartDate BETWEEN @StartDate AND @EndDate
						OR	m.EndDate	BETWEEN @StartDate AND @EndDate
						OR (@StartDate IS NULL OR @EndDate IS NULL)
					)
		ORDER BY	m.StartDate DESC

	 END
	
	SELECT		COUNT(m.MicrocycleId) [Count]
	FROM		Microcycle m
	INNER JOIN	[User] u	ON m.UserId = u.UserId
	WHERE		u.Reference = @UserReference
	AND			(m.[Description] LIKE '%'+@SearchString+'%' OR @SearchString IS NULL)
	AND			(
					-- Date Range Filter...
						m.StartDate BETWEEN @StartDate AND @EndDate
					OR	m.EndDate	BETWEEN @StartDate AND @EndDate
					OR (@StartDate IS NULL OR @EndDate IS NULL)
				)

END
GO
/****** Object:  StoredProcedure [dbo].[spMicrocycleInsert]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spMicrocycleInsert]
	@Reference		UNIQUEIDENTIFIER,
	@UserReference	UNIQUEIDENTIFIER,
	@StartDate		DATETIME,
	@EndDate		DATETIME		= NULL,
	@Description	VARCHAR(500)	= NULL,
	@Notes			VARCHAR(1000)	= NULL,
	@MesocycleId	INTEGER			= NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @UserId INTEGER

	SELECT	@UserId = UserId 
	FROM	[User] 
	WHERE	Reference = @UserReference
	
	INSERT INTO Microcycle
		(Reference
		,UserId
		,StartDate
		,EndDate
		,[Description]
		,Notes
		,MesocycleId)
	VALUES
		(@Reference
		,@UserId
		,@StartDate
		,@EndDate
		,@Description
		,@Notes
		,@MesocycleId)

	SELECT SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[spMicrocycleSelectById]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spMicrocycleSelectById]
	@MicrocycleId	INTEGER,
	@UserReference	UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;

	SELECT TOP	(1)		
				m.MicrocycleId,
				m.Reference,
				m.StartDate,
				m.EndDate,
				m.[Description],
				m.Notes,
				m.MesocycleId
	FROM		Microcycle m
	INNER JOIN	[User] u	ON m.UserId = u.UserId
	WHERE		u.Reference = @UserReference
	AND			m.MicrocycleId = @MicrocycleId

END
GO
/****** Object:  StoredProcedure [dbo].[spMicrocycleUpdate]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spMicrocycleUpdate]
	@MicrocycleId	INTEGER,
	@Reference		UNIQUEIDENTIFIER,
	@UserReference	UNIQUEIDENTIFIER,
	@StartDate		DATETIME,
	@EndDate		DATETIME		= NULL,
	@Description	VARCHAR(500)	= NULL,
	@Notes			VARCHAR(1000)	= NULL,
	@MesocycleId	INTEGER			= NULL
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE		Microcycle
	SET			StartDate		= @StartDate,
				EndDate			= @EndDate,
				[Description]	= @Description,
				Notes			= @Notes,
				MesocycleId		= @MesocycleId
	FROM		Microcycle m
	INNER JOIN	[User] u		ON m.UserId = u.UserId
	WHERE		m.MicrocycleId	= @MesocycleId
	AND			u.Reference		= @UserReference

END
GO
/****** Object:  StoredProcedure [dbo].[spRecordDelete]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spRecordDelete]
	@RecordId		INTEGER,
	@UserReference	UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;

	DELETE		r
	FROM		Record r
	INNER JOIN	[User] u		ON	r.UserId = u.UserId
	WHERE		r.RecordId		= @RecordId
	AND			u.Reference		= @UserReference
END
GO
/****** Object:  StoredProcedure [dbo].[spRecordFilter]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spRecordFilter]
	@UserReference	UNIQUEIDENTIFIER,
	@ExerciseId		INTEGER		= NULL,
	@StartDate		DATETIME	= NULL,
	@EndDate		DATETIME	= NULL,
	@MesocycleId	INTEGER		= NULL,
	@MicrocycleId	INTEGER		= NULL,
	@Offset			INTEGER		= NULL,
	@Fetch			INTEGER		= NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF @Offset IS NOT NULL AND @Fetch IS NOT NULL
	 BEGIN

		SELECT		r.RecordId,
					r.Reference,
					u.Reference		AS UserReference,
					r.ExerciseId,
					ex.[Name]		AS ExerciseName,
					r.[Date],
					r.Reps,
					r.SetReference,
					r.SetOrdinal,
					r.WeightKg,
					r.BodyweightKg,
					r.RPE,
					r.MicrocycleId,
					r.MesocycleId,
					r.Notes,
					r.CreatedDateTimeUtc
		FROM		Record r
		INNER JOIN	[User] u	ON r.UserId = u.UserId
		INNER JOIN	Exercise ex	ON r.ExerciseId = ex.ExerciseId
		WHERE		u.Reference = @UserReference
		AND			(r.ExerciseId = @ExerciseId OR @ExerciseId IS NULL)
		AND			(r.[Date] BETWEEN @StartDate AND @EndDate OR (@StartDate IS NULL OR @EndDate IS NULL))
		AND			(r.MesocycleId = @MesocycleId OR @MesocycleId IS NULL)
		AND			(r.MicrocycleId = @MicrocycleId OR @MicrocycleId IS NULL)
		ORDER BY	r.[Date] DESC
		OFFSET		@Offset ROWS FETCH NEXT @Fetch ROWS ONLY

	 END
	ELSE IF @Fetch IS NOT NULL AND @Offset IS NULL
	 BEGIN

		SELECT TOP	(@Fetch)		
					r.RecordId,
					r.Reference,
					u.Reference		AS UserReference,
					r.ExerciseId,
					ex.[Name]		AS ExerciseName,
					r.[Date],
					r.Reps,
					r.SetReference,
					r.SetOrdinal,
					r.WeightKg,
					r.BodyweightKg,
					r.RPE,
					r.MicrocycleId,
					r.MesocycleId,
					r.Notes,
					r.CreatedDateTimeUtc
		FROM		Record r
		INNER JOIN	[User] u	ON r.UserId = u.UserId
		INNER JOIN	Exercise ex	ON r.ExerciseId = ex.ExerciseId
		WHERE		u.Reference = @UserReference
		AND			(r.ExerciseId = @ExerciseId OR @ExerciseId IS NULL)
		AND			(r.[Date] BETWEEN @StartDate AND @EndDate OR (@StartDate IS NULL OR @EndDate IS NULL))
		AND			(r.MesocycleId = @MesocycleId OR @MesocycleId IS NULL)
		AND			(r.MicrocycleId = @MicrocycleId OR @MicrocycleId IS NULL)
		ORDER BY	r.[Date] DESC

	 END
	
	SELECT		COUNT(ex.ExerciseId) [Count]
	FROM		Record r
	INNER JOIN	[User] u	ON r.UserId = u.UserId
	INNER JOIN	Exercise ex	ON r.ExerciseId = ex.ExerciseId
	WHERE		u.Reference = @UserReference
	AND			(r.ExerciseId = @ExerciseId OR @ExerciseId IS NULL)
	AND			(r.[Date] BETWEEN @StartDate AND @EndDate OR (@StartDate IS NULL OR @EndDate IS NULL))
    
END
GO
/****** Object:  StoredProcedure [dbo].[spRecordInsert]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spRecordInsert]
	@Reference		UNIQUEIDENTIFIER,
	@UserReference	UNIQUEIDENTIFIER,
	@ExerciseId		INTEGER,
	@Date			DATETIME,
	@Reps			INTEGER,
	@SetReference	UNIQUEIDENTIFIER	= NULL,
	@SetOrdinal		INTEGER				= NULL,
	@WeightKg		DECIMAL(6,2)		= NULL,
	@BodyweightKg	DECIMAL(6,2)		= NULL,
	@RPE			DECIMAL(3,1)		= NULL,
	@Notes			VARCHAR(1000)		= NULL,
	@MesocycleId	INTEGER				= NULL,
	@MicrocycleId	INTEGER				= NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @UserId INTEGER
	SELECT TOP 1 @UserId = UserId
	FROM [User]
	WHERE Reference = @UserReference

	INSERT INTO Record
		(Reference
		,UserId
		,ExerciseId
		,[Date]
		,Reps
		,SetReference
		,SetOrdinal
		,WeightKg
		,BodyweightKg
		,RPE
		,Notes
		,MesocycleId
		,MicrocycleId
		,CreatedDateTimeUtc)
	VALUES
		(@Reference
		,@UserId
		,@ExerciseId
		,@Date
		,@Reps
		,@SetReference
		,@SetOrdinal
		,@WeightKg
		,@BodyweightKg
		,@RPE
		,@Notes
		,@MesocycleId
		,@MicrocycleId
		,GETUTCDATE())

	SELECT SCOPE_IDENTITY()

END
GO
/****** Object:  StoredProcedure [dbo].[spRecordSelectById]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spRecordSelectById]
	@RecordId		INTEGER,
	@UserReference	UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;

	SELECT TOP	(1)		
				r.RecordId,
				r.Reference,
				u.Reference		AS UserReference,
				r.ExerciseId,
				ex.[Name]		AS ExerciseName,
				r.[Date],
				r.Reps,
				r.SetReference,
				r.SetOrdinal,
				r.WeightKg,
				r.BodyweightKg,
				r.RPE,
				r.MicrocycleId,
				r.MesocycleId,
				r.Notes,
				r.CreatedDateTimeUtc
	FROM		Record r
	INNER JOIN	[User] u	ON r.UserId = u.UserId
	INNER JOIN	Exercise ex	ON r.ExerciseId = ex.ExerciseId
	WHERE		r.RecordId = @RecordId 
	AND			u.Reference = @UserReference

END
GO
/****** Object:  StoredProcedure [dbo].[spRecordUpdate]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spRecordUpdate]
	@RecordId		INTEGER,
	@UserReference	UNIQUEIDENTIFIER,
	@ExerciseId		INTEGER,
	@Date			DATETIME,
	@Reps			INTEGER,
	@SetReference	UNIQUEIDENTIFIER	= NULL,
	@SetOrdinal		INTEGER				= NULL,
	@WeightKg		DECIMAL(6,2)		= NULL,
	@BodyweightKg	DECIMAL(6,2)		= NULL,
	@RPE			DECIMAL(3,1)		= NULL,
	@Notes			VARCHAR(1000)		= NULL
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE		Record
	SET			ExerciseId		= @ExerciseId,
				[Date]			= @Date,
				Reps			= @Reps,
				SetReference	= @SetReference,
				SetOrdinal		= @SetOrdinal,
				WeightKg		= @WeightKg,
				BodyweightKg	= @BodyweightKg,
				RPE				= @RPE,
				Notes			= @Notes
	FROM		Record r
	INNER JOIN	[User] u		ON r.UserId = u.UserId
	WHERE		r.RecordId		= @RecordId
	AND			u.Reference		= @UserReference

END
GO
/****** Object:  StoredProcedure [dbo].[spSecurityQuestionSelect]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spSecurityQuestionSelect]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT	SecurityQuestionId,
			QuestionText
	FROM	SecurityQuestion 
	
END
GO
/****** Object:  StoredProcedure [dbo].[spUserInsert]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spUserInsert]
	@Reference		UNIQUEIDENTIFIER,
	@Name			VARCHAR(500),
	@EmailAddress	VARCHAR(500),
	@PasswordHash	NVARCHAR(2000),
	@UserTypeCode	INTEGER
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [User]
		(Reference
		,[Name]
		,EmailAddress
		,PasswordHash
		,UserTypeCode
		,RegisteredDateTimeUtc)
     VALUES
		(@Reference
		,@Name
		,@EmailAddress
		,@PasswordHash
		,@UserTypeCode
		,GETUTCDATE())

	SELECT SCOPE_IDENTITY()
END
GO
/****** Object:  StoredProcedure [dbo].[spUserSecurityQuestionDelete]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spUserSecurityQuestionDelete]
	@UserSecurityQuestionId	INTEGER,
	@UserReference					UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;

	DELETE		usqa
	FROM		UserSecurityQuestion usqa
	INNER JOIN	[User] u							ON	usqa.UserId = u.UserId
	WHERE		usqa.UserSecurityQuestionId			= @UserSecurityQuestionId
	AND			u.Reference							= @UserReference

END
GO
/****** Object:  StoredProcedure [dbo].[spUserSecurityQuestionInsert]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spUserSecurityQuestionInsert]
	@Reference					UNIQUEIDENTIFIER,
	@UserReference				UNIQUEIDENTIFIER,
	@QuestionText				VARCHAR(2000),
	@AnswerHash					VARCHAR(2000),
	@FailedAnswerAttemptCount	INTEGER
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @UserId INTEGER
	SET @UserId		=	(
						SELECT	TOP (1)
								UserId
						FROM	[User]
						WHERE	Reference = @UserReference
						)

	INSERT INTO UserSecurityQuestion
		(Reference
		,UserId
		,QuestionText
		,AnswerHash
		,FailedAnswerAttemptCount)
	VALUES
		(@Reference
		,@UserId
		,@QuestionText
		,@AnswerHash
		,@FailedAnswerAttemptCount)

END
GO
/****** Object:  StoredProcedure [dbo].[spUserSecurityQuestionSelect]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spUserSecurityQuestionSelect]
	@UserReference	UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;

	SELECT		usqa.UserSecurityQuestionId,
				usqa.Reference,
				@UserReference	AS UserReference,
				usqa.QuestionText,
				usqa.AnswerHash,
				usqa.FailedAnswerAttemptCount
	FROM		UserSecurityQuestion usqa
	INNER JOIN	[User] u ON usqa.UserId = u.UserId
	WHERE		u.Reference = @UserReference

END
GO
/****** Object:  StoredProcedure [dbo].[spUserSecurityQuestionUpdate]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spUserSecurityQuestionUpdate]
	@UserSecurityQuestionId			INTEGER,
	@UserReference					UNIQUEIDENTIFIER,
	@QuestionText					VARCHAR(2000),
	@AnswerHash						VARCHAR(2000),
	@FailedAnswerAttemptCount		INTEGER
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE		UserSecurityQuestion
	SET			QuestionText							= @QuestionText,
				AnswerHash								= @AnswerHash,
				FailedAnswerAttemptCount				= @FailedAnswerAttemptCount
	FROM		UserSecurityQuestion usqa
	INNER JOIN	[User] u								ON usqa.UserId = u.UserId
	WHERE		usqa.UserSecurityQuestionId				= @UserSecurityQuestionId
	AND			u.Reference								= @UserReference

END
GO
/****** Object:  StoredProcedure [dbo].[spUserSelectByEmailAddress]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spUserSelectByEmailAddress]
	@EmailAddress	VARCHAR(500)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT	TOP (1) 
			UserId,
			Reference,
			[Name],
			EmailAddress,
			PasswordHash,
			UserTypeCode,
			LastLoginDateTimeUtc,
			LockoutEndDateTimeUtc,
			FailedLoginAttemptCount,
			IsRegistrationValidated,
			RegisteredDateTimeUtc
	FROM	[User]
	WHERE	EmailAddress	= @EmailAddress
	AND		IsDeleted		= 0 

END
GO
/****** Object:  StoredProcedure [dbo].[spUserSelectByReference]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spUserSelectByReference]
	@Reference	UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;

	SELECT	TOP (1) 
			UserId,
			Reference,
			[Name],
			EmailAddress,
			PasswordHash,
			UserTypeCode,
			LastLoginDateTimeUtc,
			LockoutEndDateTimeUtc,
			FailedLoginAttemptCount,
			IsRegistrationValidated,
			RegisteredDateTimeUtc
	FROM	[User]
	WHERE	Reference	= @Reference
	AND		IsDeleted	= 0 

END
GO
/****** Object:  StoredProcedure [dbo].[spUserTokenDelete]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spUserTokenDelete]
	@Reference	UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;

	DELETE		
	FROM		UserToken
	WHERE		Reference	= @Reference

END
GO
/****** Object:  StoredProcedure [dbo].[spUserTokenInsert]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spUserTokenInsert]
	@Reference			UNIQUEIDENTIFIER,
	@UserReference		UNIQUEIDENTIFIER,
	@PurposeCode		VARCHAR(100),
	@ExpiryDateTimeUtc	DATETIME
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @UserId INTEGER
	SET @UserId =	(
					SELECT	UserId
					FROM	[User]
					WHERE	Reference = @UserReference
					)

	INSERT INTO UserToken
		(Reference
		,UserId
		,PurposeCode
		,IssuedDateTimeUtc
		,ExpiryDateTimeUtc)
     VALUES
		(@Reference
		,@UserId
		,@PurposeCode
		,GETUTCDATE()
		,@ExpiryDateTimeUtc)

END
GO
/****** Object:  StoredProcedure [dbo].[spUserTokenSelect]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spUserTokenSelect]
	@UserReference	UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @UserId INTEGER
	SET @UserId		=	(
						SELECT	TOP (1)	UserId
						FROM	[User]
						WHERE	Reference = @UserReference
						)

	SELECT	UserTokenId,
			UserId,
			Reference,
			PurposeCode,
			IssuedDateTimeUtc,
			ExpiryDateTimeUtc
	FROM	UserToken
	WHERE	UserId	= @UserId

END
GO
/****** Object:  StoredProcedure [dbo].[spUserTokenSelectById]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spUserTokenSelectById]
	@UserTokenId	INTEGER,
	@UserReference	UNIQUEIDENTIFIER

AS
BEGIN
	SET NOCOUNT ON;

	SELECT		TOP (1)
				ut.UserTokenId,
				u.Reference		AS UserReference,
				ut.Reference,
				ut.PurposeCode,
				ut.IssuedDateTimeUtc,
				ut.ExpiryDateTimeUtc
	FROM		UserToken ut
	INNER JOIN	[User] u		ON ut.UserId = u.UserId
	WHERE		ut.UserTokenId	= @UserTokenId
	AND			u.Reference		= @UserReference

END
GO
/****** Object:  StoredProcedure [dbo].[spUserTokenSelectByReference]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spUserTokenSelectByReference]
	@Reference	UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;

	SELECT		TOP (1)
				ut.UserTokenId,
				u.Reference		AS UserReference,
				ut.Reference,
				ut.PurposeCode,
				ut.IssuedDateTimeUtc,
				ut.ExpiryDateTimeUtc
	FROM		UserToken ut
	INNER JOIN	[User] u		ON ut.UserId = u.UserId
	WHERE		ut.Reference	= @Reference

END
GO
/****** Object:  StoredProcedure [dbo].[spUserUpdate]    Script Date: 19/01/2021 22:31:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spUserUpdate]
	@Reference					UNIQUEIDENTIFIER,
	@Name						VARCHAR(500),
	@PasswordHash				NVARCHAR(2000),
	@UserTypeCode				INTEGER,
	@LastLoginDateTimeUtc		DATETIME,
	@LockoutEndDateTimeUtc		DATETIME,
	@FailedLoginAttemptCount	INTEGER,
	@IsRegistrationValidated	BIT
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE	[User]
	SET		[Name]						= @Name,
			PasswordHash				= @PasswordHash,
			UserTypeCode				= @UserTypeCode,
			LastLoginDateTimeUtc		= @LastLoginDateTimeUtc,
			LockoutEndDateTimeUtc		= @LockoutEndDateTimeUtc,
			FailedLoginAttemptCount		= @FailedLoginAttemptCount,
			IsRegistrationValidated		= @IsRegistrationValidated
	WHERE	Reference					= @Reference
END
GO
