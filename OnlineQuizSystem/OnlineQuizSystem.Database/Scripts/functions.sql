USE OnlineQuizDB;
GO

-- Function: convert raw score into a percentage safely.
CREATE FUNCTION dbo.fn_GetSessionPercent(
    @Score INT,
    @TotalQuestions INT
)
RETURNS DECIMAL(5,2)
AS
BEGIN
    DECLARE @Percent DECIMAL(5,2);
    IF @TotalQuestions = 0
        SET @Percent = 0.00;
    ELSE
        SET @Percent = ROUND((CAST(@Score AS DECIMAL(10,2)) / @TotalQuestions) * 100.0, 2);

    RETURN @Percent;
END;
GO
-- Function: answer correctness helper.
CREATE FUNCTION dbo.fn_IsAnswerCorrect(
    @QuestionID INT,
    @SelectedAnswer CHAR(1)
)
RETURNS BIT
AS
BEGIN
    DECLARE @CorrectOption CHAR(1);

    SELECT @CorrectOption = CorrectOption
    FROM Questions
    WHERE QuestionID = @QuestionID;

    IF @CorrectOption IS NULL
        RETURN 0; -- unknown question means not correct

    RETURN CASE WHEN @SelectedAnswer = @CorrectOption THEN 1 ELSE 0 END;
END;
GO

-- Inline TVF: return question details by QuestionID.
CREATE FUNCTION dbo.ufn_GetQuestionDetails(
    @QuestionID INT
)
RETURNS TABLE
AS
RETURN
(
    SELECT
        q.QuestionID,
        q.QuestionText,
        q.OptionA,
        q.OptionB,
        q.OptionC,
        q.OptionD,
        q.CorrectOption,
        q.DifficultyLevel,
        q.TeacherID
    FROM Questions q
    WHERE q.QuestionID = @QuestionID
);
GO
