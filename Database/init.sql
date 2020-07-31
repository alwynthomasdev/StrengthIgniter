CREATE DATABASE [StrengthIgniter]
GO

USE [StrengthIgniter]
GO
/****** Object:  Table [dbo].[AuditEvent]    Script Date: 18/05/2020 21:49:54 ******/
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
/****** Object:  Table [dbo].[AuditEventItem]    Script Date: 18/05/2020 21:49:54 ******/
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
/****** Object:  Table [dbo].[Exercise]    Script Date: 18/05/2020 21:49:54 ******/
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
 CONSTRAINT [PK_Exercise] PRIMARY KEY CLUSTERED 
(
	[ExerciseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Record]    Script Date: 18/05/2020 21:49:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Record](
	[RecordId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ExerciseId] [int] NOT NULL,
	[Date] [date] NOT NULL,
	[Sets] [int] NULL,
	[Reps] [int] NOT NULL,
	[WeightKg] [decimal](6, 2) NULL,
	[BodyweightKg] [decimal](6, 2) NULL,
	[RPE] [decimal](3, 1) NULL,
	[Notes] [varchar](4000) NULL,
	[CreatedDateTimeUtc] [datetime] NULL,
 CONSTRAINT [PK_Record] PRIMARY KEY CLUSTERED 
(
	[RecordId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RecordImport]    Script Date: 18/05/2020 21:49:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RecordImport](
	[RecordImportId] [int] IDENTITY(1,1) NOT NULL,
	[Reference] [uniqueidentifier] NOT NULL,
	[UserId] [int] NOT NULL,
	[RecordImportSchemaId] [int] NOT NULL,
	[Name] [varchar](500) NOT NULL,
	[ImportDateTimeUtc] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[DeletedDateTimeUtc] [datetime] NULL,
 CONSTRAINT [PK_RecordImport] PRIMARY KEY CLUSTERED 
(
	[RecordImportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RecordImportRow]    Script Date: 18/05/2020 21:49:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RecordImportRow](
	[RecordImportRowId] [int] IDENTITY(1,1) NOT NULL,
	[RecordImportId] [int] NOT NULL,
	[StatusCode] [int] NOT NULL,
	[ExerciseText] [varchar](500) NULL,
	[DateText] [varchar](100) NULL,
	[WeightKgText] [varchar](100) NULL,
	[WeightLbText] [varchar](100) NULL,
	[BodyweightKgText] [varchar](100) NULL,
	[BodyweightLbText] [varchar](100) NULL,
	[SetText] [varchar](25) NULL,
	[RepText] [varchar](25) NULL,
	[RpeText] [varchar](25) NULL,
	[Notes] [varchar](2000) NULL,
 CONSTRAINT [PK_RecordImportRow] PRIMARY KEY CLUSTERED 
(
	[RecordImportRowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RecordImportRowError]    Script Date: 18/05/2020 21:49:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RecordImportRowError](
	[RecordImportRowErrorId] [int] IDENTITY(1,1) NOT NULL,
	[RecordImportRowId] [int] NOT NULL,
	[ErrorCode] [int] NOT NULL,
 CONSTRAINT [PK_RecordImportRowError] PRIMARY KEY CLUSTERED 
(
	[RecordImportRowErrorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RecordImportSchema]    Script Date: 18/05/2020 21:49:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RecordImportSchema](
	[RecordImportSchemaId] [int] IDENTITY(1,1) NOT NULL,
	[Reference] [uniqueidentifier] NOT NULL,
	[UserId] [int] NOT NULL,
	[Name] [varchar](500) NOT NULL,
	[Delimiter] [char](1) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[DeletedDateTimeUtc] [datetime] NULL,
 CONSTRAINT [PK_RecordImportSchema] PRIMARY KEY CLUSTERED 
(
	[RecordImportSchemaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RecordImportSchemaColumnMap]    Script Date: 18/05/2020 21:49:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RecordImportSchemaColumnMap](
	[RecordImportSchemaColumnMapId] [int] IDENTITY(1,1) NOT NULL,
	[RecordImportSchemaId] [int] NOT NULL,
	[HeaderName] [varchar](500) NOT NULL,
	[ColumnTypeCode] [int] NOT NULL,
 CONSTRAINT [PK_RecordImportSchemaColumnMap] PRIMARY KEY CLUSTERED 
(
	[RecordImportSchemaColumnMapId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RecordImportSchemaExerciseMap]    Script Date: 18/05/2020 21:49:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RecordImportSchemaExerciseMap](
	[RecordImportSchemaExerciseMapId] [int] IDENTITY(1,1) NOT NULL,
	[RecordImportSchemaId] [int] NOT NULL,
	[ExerciseId] [int] NOT NULL,
	[Text] [varchar](500) NOT NULL,
 CONSTRAINT [PK_RecordImportSchemaExerciseMap] PRIMARY KEY CLUSTERED 
(
	[RecordImportSchemaExerciseMapId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SecurityQuestion]    Script Date: 18/05/2020 21:49:54 ******/
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
/****** Object:  Table [dbo].[User]    Script Date: 18/05/2020 21:49:54 ******/
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
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserSecurityQuestionAnswer]    Script Date: 18/05/2020 21:49:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserSecurityQuestionAnswer](
	[UserSecurityQuestionAnswerId] [int] IDENTITY(1,1) NOT NULL,
	[Reference] [uniqueidentifier] NOT NULL,
	[UserId] [int] NOT NULL,
	[QuestionText] [varchar](2000) NOT NULL,
	[AnswerHash] [nvarchar](2000) NOT NULL,
	[FailedAnswerAttemptCount] [int] NULL,
 CONSTRAINT [PK_UserSecurityQuestionAnswer] PRIMARY KEY CLUSTERED 
(
	[UserSecurityQuestionAnswerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserToken]    Script Date: 18/05/2020 21:49:54 ******/
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
/****** Object:  Index [UX_Reference]    Script Date: 18/05/2020 21:49:54 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_Reference] ON [dbo].[Exercise]
(
	[Reference] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [UX_Reference]    Script Date: 18/05/2020 21:49:54 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_Reference] ON [dbo].[RecordImport]
(
	[Reference] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [UX_Reference]    Script Date: 18/05/2020 21:49:54 ******/
CREATE NONCLUSTERED INDEX [UX_Reference] ON [dbo].[RecordImportSchema]
(
	[Reference] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UX_EmailAddress]    Script Date: 18/05/2020 21:49:54 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_EmailAddress] ON [dbo].[User]
(
	[EmailAddress] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [UX_Reference]    Script Date: 18/05/2020 21:49:54 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_Reference] ON [dbo].[User]
(
	[Reference] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [UX_Reference]    Script Date: 18/05/2020 21:49:54 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_Reference] ON [dbo].[UserSecurityQuestionAnswer]
(
	[Reference] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [UX_Reference]    Script Date: 18/05/2020 21:49:54 ******/
CREATE NONCLUSTERED INDEX [UX_Reference] ON [dbo].[UserToken]
(
	[Reference] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Exercise] ADD  CONSTRAINT [DF_Exercise_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[RecordImport] ADD  CONSTRAINT [DF_RecordImport_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[RecordImportSchema] ADD  CONSTRAINT [DF_RecordImportSchema_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_IsRegistrationValidated]  DEFAULT ((0)) FOR [IsRegistrationValidated]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[AuditEventItem]  WITH CHECK ADD  CONSTRAINT [FK_AuditEventItem_AuditEvent] FOREIGN KEY([AuditEventId])
REFERENCES [dbo].[AuditEvent] ([AuditEventId])
GO
ALTER TABLE [dbo].[AuditEventItem] CHECK CONSTRAINT [FK_AuditEventItem_AuditEvent]
GO
ALTER TABLE [dbo].[Record]  WITH CHECK ADD  CONSTRAINT [FK_Record_Exercise] FOREIGN KEY([ExerciseId])
REFERENCES [dbo].[Exercise] ([ExerciseId])
GO
ALTER TABLE [dbo].[Record] CHECK CONSTRAINT [FK_Record_Exercise]
GO
ALTER TABLE [dbo].[Record]  WITH CHECK ADD  CONSTRAINT [FK_Record_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[Record] CHECK CONSTRAINT [FK_Record_User]
GO
ALTER TABLE [dbo].[RecordImport]  WITH CHECK ADD  CONSTRAINT [FK_RecordImport_RecordImportSchema] FOREIGN KEY([RecordImportSchemaId])
REFERENCES [dbo].[RecordImportSchema] ([RecordImportSchemaId])
GO
ALTER TABLE [dbo].[RecordImport] CHECK CONSTRAINT [FK_RecordImport_RecordImportSchema]
GO
ALTER TABLE [dbo].[RecordImport]  WITH CHECK ADD  CONSTRAINT [FK_RecordImport_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[RecordImport] CHECK CONSTRAINT [FK_RecordImport_User]
GO
ALTER TABLE [dbo].[RecordImportRow]  WITH CHECK ADD  CONSTRAINT [FK_RecordImportRow_RecordImport] FOREIGN KEY([RecordImportId])
REFERENCES [dbo].[RecordImport] ([RecordImportId])
GO
ALTER TABLE [dbo].[RecordImportRow] CHECK CONSTRAINT [FK_RecordImportRow_RecordImport]
GO
ALTER TABLE [dbo].[RecordImportRowError]  WITH CHECK ADD  CONSTRAINT [FK_RecordImportRowError_RecordImportRow] FOREIGN KEY([RecordImportRowId])
REFERENCES [dbo].[RecordImportRow] ([RecordImportRowId])
GO
ALTER TABLE [dbo].[RecordImportRowError] CHECK CONSTRAINT [FK_RecordImportRowError_RecordImportRow]
GO
ALTER TABLE [dbo].[RecordImportSchema]  WITH CHECK ADD  CONSTRAINT [FK_RecordImportSchema_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[RecordImportSchema] CHECK CONSTRAINT [FK_RecordImportSchema_User]
GO
ALTER TABLE [dbo].[RecordImportSchemaColumnMap]  WITH CHECK ADD  CONSTRAINT [FK_RecordImportSchemaColumnMap_RecordImportSchema] FOREIGN KEY([RecordImportSchemaId])
REFERENCES [dbo].[RecordImportSchema] ([RecordImportSchemaId])
GO
ALTER TABLE [dbo].[RecordImportSchemaColumnMap] CHECK CONSTRAINT [FK_RecordImportSchemaColumnMap_RecordImportSchema]
GO
ALTER TABLE [dbo].[RecordImportSchemaExerciseMap]  WITH CHECK ADD  CONSTRAINT [FK_RecordImportSchemaExerciseMap_Exercise] FOREIGN KEY([ExerciseId])
REFERENCES [dbo].[Exercise] ([ExerciseId])
GO
ALTER TABLE [dbo].[RecordImportSchemaExerciseMap] CHECK CONSTRAINT [FK_RecordImportSchemaExerciseMap_Exercise]
GO
ALTER TABLE [dbo].[RecordImportSchemaExerciseMap]  WITH CHECK ADD  CONSTRAINT [FK_RecordImportSchemaExerciseMap_RecordImportSchema] FOREIGN KEY([RecordImportSchemaId])
REFERENCES [dbo].[RecordImportSchema] ([RecordImportSchemaId])
GO
ALTER TABLE [dbo].[RecordImportSchemaExerciseMap] CHECK CONSTRAINT [FK_RecordImportSchemaExerciseMap_RecordImportSchema]
GO
ALTER TABLE [dbo].[UserSecurityQuestionAnswer]  WITH CHECK ADD  CONSTRAINT [FK_UserSecurityQuestionAnswer_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[UserSecurityQuestionAnswer] CHECK CONSTRAINT [FK_UserSecurityQuestionAnswer_User]
GO
ALTER TABLE [dbo].[UserToken]  WITH CHECK ADD  CONSTRAINT [FK_UserToken_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[UserToken] CHECK CONSTRAINT [FK_UserToken_User]
GO
