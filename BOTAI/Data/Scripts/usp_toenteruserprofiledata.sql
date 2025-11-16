CREATE PROCEDURE BOTAI.CreateUserProfile
    @UserName NVARCHAR(100),
    @Password NVARCHAR(255),
    @Email NVARCHAR(255),
    @FirstName NVARCHAR(100) = NULL,
    @LastName NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Check for duplicate email
    IF EXISTS (SELECT 1 FROM BOTAI.UserProfile WHERE Email = @Email)
    BEGIN
        RAISERROR('Email already exists.', 16, 1);
        RETURN;
    END

    -- Insert new user
    INSERT INTO BOTAI.UserProfile (UserName, Password, Email, FirstName, LastName)
    VALUES (@UserName, @Password, @Email, @FirstName, @LastName);
END;