-- View: session summary by user. Shows the quiz score and easy-to-read percentage.
CREATE VIEW dbo.vw_UserSessionSummary
AS
SELECT
    qs.SessionID,
    u.UserID,
    u.Name AS UserName,
    qs.Score,
    qs.TotalQuestions,
    dbo.fn_GetSessionPercent(qs.Score, qs.TotalQuestions) AS ScorePercent,
    qs.StartTime,
    qs.EndTime,
    qs.IsSubmitted
FROM QuizSessions qs
INNER JOIN Users u
    ON qs.UserID = u.UserID;
GO
select * from dbo.vw_UserSessionSummary
-- View: section enrollment details. Shows how many students are enrolled in each section.
CREATE VIEW dbo.vw_SectionEnrollmentDetails
AS
SELECT
    s.SectionID,
    s.SectionName,
    t.Name AS TeacherName,
    COUNT(us.UserSectionID) AS EnrolledUserCount
FROM Sections s
INNER JOIN Teachers t
    ON s.TeacherID = t.TeacherID
LEFT JOIN UserSections us
    ON s.SectionID = us.SectionID
GROUP BY
    s.SectionID,
    s.SectionName,
    t.Name;
GO
