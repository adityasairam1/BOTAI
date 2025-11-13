-- 1. Create schema if not exists
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'BOTAI')
BEGIN
    EXEC('CREATE SCHEMA BOTAI');
END;

-- 2. Create sequence if not exists
IF NOT EXISTS (
    SELECT * FROM sys.sequences 
    WHERE name = 'UserProfileSeq' 
      AND schema_id = SCHEMA_ID('BOTAI')
)
BEGIN
    EXEC('CREATE SEQUENCE BOTAI.UserProfileSeq START WITH 1 INCREMENT BY 1');
END;

-- 3. Create table if not exists
IF OBJECT_ID('BOTAI.UserProfile', 'U') IS NULL
BEGIN
    CREATE TABLE BOTAI.UserProfile (
        UserID INT PRIMARY KEY DEFAULT NEXT VALUE FOR BOTAI.UserProfileSeq,
        UserName NVARCHAR(100) NOT NULL,
        Password NVARCHAR(255) NOT NULL,
        Email NVARCHAR(255) NOT NULL UNIQUE,
        FirstName NVARCHAR(100),
        LastName NVARCHAR(100),
        CreatedAt DATETIME DEFAULT GETDATE(),
        UpdatedAt DATETIME DEFAULT GETDATE()
    );
END;
