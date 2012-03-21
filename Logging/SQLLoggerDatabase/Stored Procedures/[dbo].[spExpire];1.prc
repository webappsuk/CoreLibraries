CREATE PROCEDURE [dbo].[spExpire];1 (@Date DateTime) AS

DELETE
FROM	dbo.LogEntry WITH (READPAST)
WHERE	Expiry < @Date

DELETE	C
FROM	dbo.Context AS C WITH (READPAST)
	LEFT OUTER JOIN dbo.LogEntry AS L WITH (NOLOCK) ON C.LogEntryGuid = L.[Guid]
WHERE	L.[Guid] IS NULL

DELETE	O
FROM	dbo.Operation AS O WITH (READPAST)
	LEFT OUTER JOIN dbo.LogEntry AS L WITH (NOLOCK) ON L.OperationGuid = O.[Guid]
WHERE	L.[Guid] IS NULL

DELETE	A
FROM	dbo.Argument AS A WITH (READPAST)
	LEFT OUTER JOIN dbo.Operation AS O WITH (NOLOCK) ON A.OperationGuid = O.[Guid]
WHERE	O.[Guid] IS NULL