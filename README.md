# OnlineQuizSystem

A modern, full-stack online quiz application built with ASP.NET. This project provides a polished experience for administrators, teachers, and students to manage quizzes, sections, questions, and results in a clean, easy-to-use interface.

---

## ⭐ Key Features

- **Role-based access**
  - Admin dashboard for managing users, sections, and teachers
  - Teacher view for creating and supervising quizzes
  - Student quiz taking and progress reporting

- **Quiz management**
  - Create, edit, and organize quizzes by section
  - Support for multiple question types and scoring
  - Automatic result calculation after quiz submission
  - Built-in anti-cheating controls for fair quiz delivery

- **User and profile handling**
  - Secure login, logout, and signup flows
  - User profile management
  - Admin account management

- **Results tracking**
  - View quiz history and result summaries
  - Retrieve student performance across quizzes
  - Export-ready result data for reporting

- **Responsive UI**
  - Clean and intuitive Razor Pages frontend
  - Mobile-friendly layout for easy use across devices

---

## 🧱 Project Structure

- `OnlineQuizSystem/` - Main application project
- `OnlineQuizSystem.BLL/` - Business logic layer and services
- `OnlineQuizSystem.DAL/` - Data access layer and repositories
- `OnlineQuizSystem.Database/` - SQL database scripts and schema
- `OnlineQuizSystem.UI/` - Razor Pages UI, views, and frontend assets

---

## 🚀 Getting Started

1. Clone the repository:

```bash
git clone https://github.com/abubakkkar/OnlineQuizSystem.git
cd OnlineQuizSystem
```

2. Open the solution in Visual Studio or your preferred IDE.

3. Restore packages and build the solution.

4. Update your database connection string in `OnlineQuizSystem/appsettings.json` if needed.

5. Run the application:

```bash
dotnet run --project OnlineQuizSystem/OnlineQuizSystem.csproj
```

---

## 🛠️ Tech Stack

- **ASP.NET** with Razor Pages
- **C#** for backend services and business logic
- **SQL** database script support
- **HTML/CSS** for modern responsive UI

---

## 💡 Notes

- The app is designed for educational environments to help manage quizzes and track results.
- The solution is modular, making it easy to extend with new question types, authentication, or reporting features.

---

## 📄 License

This project is licensed under the terms in the `LICENSE` file.
