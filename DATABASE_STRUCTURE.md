# Database Structure

## Overview
The database for this project is `OnlineQuizDB`. It supports user roles, sections, quizzes, questions, and results tracking.

This schema is designed for an online quiz system with the following responsibilities:
- store teachers, students, and admins
- organize questions by section and teacher
- enroll students in sections
- manage quiz sessions and answer results
- summarize performance with views and business logic functions

## Main tables

### Teachers
- `TeacherID` INT IDENTITY PRIMARY KEY
- `Name`, `Email`, `Password`
- `Role` defaults to `Teacher`
- `IsActive` indicates whether the teacher account is active

Relationships:
- one teacher can own many `Sections`
- one teacher can own many `Questions`
- one teacher can own many `QuizSessions`
- optional one-to-many connection to `Users` via `Users.TeacherID`

### Users
- `UserID` INT IDENTITY PRIMARY KEY
- `Name`, `Email`, `Password`
- `Role` distinguishes student or other account types
- `TeacherID` optionally links a student to a teacher
- `IsActive` indicates whether the user account is active

Relationships:
- many-to-one to `Teachers`
- many-to-many with `Sections` through `UserSections`
- one-to-many with `QuizSessions`
- one-to-many with `Results`

### Admins
- `AdminID` INT IDENTITY PRIMARY KEY
- `Name`, `Email`, `Password`
- `Role` defaults to `Admin`

This table stores administrator accounts separately from teachers and users.

### Sections
- `SectionID` INT IDENTITY PRIMARY KEY
- `SectionName`
- `TeacherID` references `Teachers(TeacherID)`
- `Description`
- `CreatedDate` defaults to the current date/time

Relationships:
- belongs to one `Teacher`
- has many enrolled students via `UserSections`
- can group `Questions` if `Questions.SectionID` is set

### UserSections
- `UserSectionID` INT IDENTITY PRIMARY KEY
- `UserID` references `Users(UserID)`
- `SectionID` references `Sections(SectionID)`
- `EnrolledDate` records when enrollment happened
- unique constraint on `(UserID, SectionID)` ensures a student cannot be enrolled twice in the same section

This table models section enrollment and supports student progress by section.

### QuizSessions
- `SessionID` INT IDENTITY PRIMARY KEY
- `UserID` references `Users(UserID)`
- `TeacherID` references `Teachers(TeacherID)`
- `Score` stores earned points
- `StartTime` and `EndTime` record the quiz interval
- `TotalQuestions` stores the number of answered questions
- `IsSubmitted` marks whether the quiz was completed

Relationships:
- links a single user to a quiz attempt
- links the attempt to a teacher for reporting

### UserTeachers
- `UserTeacherID` INT IDENTITY PRIMARY KEY
- `UserID` references `Users(UserID)`
- `TeacherID` references `Teachers(TeacherID)`
- `EnrolledDate` records the association time
- unique constraint on `(UserID, TeacherID)` prevents duplicates

This table captures explicit assignments between students and teachers.

### Questions
- `QuestionID` INT IDENTITY PRIMARY KEY
- `QuestionText`
- `OptionA`, `OptionB`, `OptionC`, `OptionD`
- `CorrectOption` stores the correct answer letter
- `DifficultyLevel` stores question difficulty
- `TeacherID` references `Teachers(TeacherID)`
- `SectionID` optionally references `Sections(SectionID)`

Relationships:
- owned by a teacher
- optionally grouped into a section
- answered through the `Results` table

### Results
- `ResultID` INT IDENTITY PRIMARY KEY
- `UserID` references `Users(UserID)`
- `QuestionID` references `Questions(QuestionID)`
- `SelectedAnswer` stores the chosen option
- `IsCorrect` stores whether the answer was correct
- `AnsweredAt` defaults to the current date/time
- `SessionID` references `QuizSessions(SessionID)`

This table records every quiz answer and feeds score calculations.

## Important objects

### Views
- `dbo.vw_UserSessionSummary`
  - summarizes session scores, totals, and percentage per user
- `dbo.vw_SectionEnrollmentDetails`
  - reports section enrollment counts by teacher

### Functions
- `dbo.fn_GetSessionPercent(@Score, @TotalQuestions)`
  - returns a percentage safely and avoids division by zero
- `dbo.fn_IsAnswerCorrect(@QuestionID, @SelectedAnswer)`
  - checks whether the selected answer matches the correct option

### Stored procedures
- `dbo.sp_RecordQuizAnswer`
  - inserts a result row and uses the correctness function to set `IsCorrect`
- `dbo.sp_SeedSampleData`
  - seeds demo data only when target tables are empty

### Trigger
- `dbo.trg_UpdateSessionTotalsOnResultInsert`
  - updates `QuizSessions.Score` and `TotalQuestions` automatically after answers are inserted into `Results`

## Key workflows

1. A teacher creates a `Section`.
2. A student is assigned a teacher and optionally enrolled in that section via `UserSections`.
3. The student starts a quiz session in `QuizSessions`.
4. Each answered question is recorded in `Results` using `sp_RecordQuizAnswer`.
5. The trigger updates session totals and score automatically.
6. Summary views provide reporting and performance insights.

## Relationships diagram (simplified)

Teachers
  ├─< Users
  ├─< Sections
  ├─< Questions
  └─< QuizSessions

Users
  ├─< UserSections >─ Sections
  ├─< QuizSessions
  ├─< Results
  └─< UserTeachers

Sections
  ├─< Questions
  └─< UserSections

Questions
  └─< Results

QuizSessions
  └─< Results
