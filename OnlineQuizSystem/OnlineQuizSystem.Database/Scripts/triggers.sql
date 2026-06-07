-- Trigger: automatically update session score and question count after results are inserted.
CREATE TRIGGER dbo.trg_UpdateSessionTotalsOnResultInsert
ON dbo.Results
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE qs
    SET
        qs.TotalQuestions = qs.TotalQuestions + i.InsertedCount,
        qs.Score = qs.Score + i.CorrectCount
    FROM QuizSessions qs
    INNER JOIN (
        SELECT
            SessionID,
            COUNT(*) AS InsertedCount,
            SUM(CAST(IsCorrect AS INT)) AS CorrectCount
        FROM inserted
        GROUP BY SessionID
    ) AS i
        ON qs.SessionID = i.SessionID;
END;
GO

-- Trigger: validate CorrectOption on insert or update to Questions
CREATE TRIGGER dbo.trg_ValidateCorrectOption
ON dbo.Questions
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 FROM inserted WHERE CorrectOption NOT IN ('A','B','C','D') OR CorrectOption IS NULL
    )
    BEGIN
        RAISERROR('CorrectOption must be one of A, B, C, or D.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO
