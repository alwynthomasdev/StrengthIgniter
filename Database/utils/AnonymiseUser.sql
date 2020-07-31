DECLARE @UserId INTEGER = 7
DECLARE @anon_id VARCHAR(255) = CONVERT(VARCHAR(255), NEWID())

UPDATE [User]
SET
	[Name] = @anon_id,
	[EmailAddress] = @anon_id + '@strengthigniter.com',
	[PasswordHash] = N'MTAwMDAwOitPemFMbzMzeWVSc0hrbEcvenU3dFFXVFVHb3kyeFI1OmM5V3AvQ1VyckYrVVRLK2kxRDh3SnhDWVFJRT0=', --pass1234
	[IsDeleted] = 1,
	[LockoutEndDateTimeUtc] = NULL,
	[FailedLoginAttemptCount] = NULL,
	[IsAnonymised] = 1,
	[DeletedDateTimeUtc] = GETUTCDATE(),
	[AnonymisedDateTimeUtc] = GETUTCDATE()
WHERE [UserId] = @UserId

UPDATE [UserSecurityQuestionAnswer]
SET
	[QuestionText] = 'Anon',
	[AnswerHash] = N'MTAwMDAwOitPemFMbzMzeWVSc0hrbEcvenU3dFFXVFVHb3kyeFI1OmM5V3AvQ1VyckYrVVRLK2kxRDh3SnhDWVFJRT0=', --pass1234
	[FailedAnswerAttemptCount] = 0
WHERE [UserId] = @UserId