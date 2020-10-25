IF NOT EXISTS(SELECT TOP 1 UserId FROM [User] WHERE Reference = '00000000-0000-0000-0000-000000000000')
 BEGIN
	INSERT INTO [User]
		(Reference
		,[Name]
		,EmailAddress
		,PasswordHash
		,UserTypeCode
		,IsRegistrationValidated
		,RegisteredDateTimeUtc)
	VALUES
		('00000000-0000-0000-0000-000000000000'
		,'System'
		,'sys@strenghtigniter.com'
		,N'MTAwMDAwOitPemFMbzMzeWVSc0hrbEcvenU3dFFXVFVHb3kyeFI1OmM5V3AvQ1VyckYrVVRLK2kxRDh3SnhDWVFJRT0=' --pass1234
		,0
		,1
		,GETUTCDATE())
 END