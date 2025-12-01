CREATE PROCEDURE BOTAI.LoginUser
    @Email NVARCHAR(256),
    @Password NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 
        UserID,
        UserName,
        Email,
        FirstName,
        LastName,
        CreatedAt,
        UpdatedAt
    FROM BOTAI.UserProfile
    WHERE Email = @Email
      AND Password = @Password;
END;
GO
