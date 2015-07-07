CREATE PROCEDURE [dbo].[spLog];1 @ApplicationGuid uniqueidentifier,
	@LogEntry dbo.tblLogEntry readonly,
	@Context dbo.tblDictionary readonly,
	@Operation dbo.tblOperation readonly,
	@Argument dbo.tblDictionary readonly
AS
BEGIN
	SET NOCOUNT ON;
	INSERT dbo.Operation (
		[Guid],
		ParentGuid,
		CategoryName,
		Name,
		InstanceHash,
		Method,
		ThreadId,
		ThreadName
	) 
	SELECT	O.[Guid],
		O.ParentGuid,
		O.CategoryName,
		O.Name,
		O.InstanceHash,
		O.Method,
		O.ThreadId,
		O.ThreadName
	FROM	@Operation O
	LEFT OUTER JOIN Operation OT WITH (NOLOCK) ON OT.[Guid] = O.[Guid]
	WHERE OT.[Guid] IS NULL
		
	INSERT dbo.Argument (
		OperationGuid, 
		Name,
		Value
	)
	SELECT	A.Guid,
		A.Name,
		A.Value
	FROM @Argument A
	LEFT OUTER JOIN Operation OT WITH (NOLOCK) ON OT.[Guid] = A.[Guid]
	WHERE OT.[Guid] IS NULL
		
	INSERT dbo.LogEntry (
		[Guid],
		[Group],
		Level,
		Message,
		ThreadId,
		ThreadName,
		Expiry,
		ApplicationGuid,
		OperationGuid,
		ExceptionType,
		StackTrace
	) SELECT	[Guid],
		[Group],
		Level,
		Message,
		ThreadId,
		ThreadName,
		Expiry,
		@ApplicationGuid,
		OperationGuid,
		ExceptionType,
		StackTrace
	FROM @LogEntry
	
	INSERT dbo.Context (
		LogEntryGuid, 
		Name,
		Value
	)
	SELECT	C.Guid,
		C.Name,
		C.Value
	FROM @Context C
END