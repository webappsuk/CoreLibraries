CREATE PROCEDURE [dbo].[spRegisterApplication];1 @ApplicationGuid uniqueidentifier,
	@ApplicationName nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT EXISTS(SELECT * FROM dbo.[Application] WHERE Guid=@ApplicationGuid)
	BEGIN
		INSERT dbo.[Application] ([Guid], Name) 
		VALUES (@ApplicationGuid, @ApplicationName) 
	END ELSE BEGIN
		UPDATE dbo.[Application]
		SET Name = @ApplicationName
		WHERE [Guid] = @ApplicationGuid
	END
END