# 🗨️ NodeChat

A real-time chat application built using **ASP.NET Core** with **Razor Pages**, designed for educational purposes. This project includes user interaction via a chat interface and covers core web development concepts such as routing, model binding, and basic testing.

## 📁 Project Structure

- `Assignment4ChatApplication/` – Main web application (front-end + back-end logic)  
- `Assignment4ChatTests/` – Unit tests for validating core functionality  
- `Pages/` – Razor pages used for rendering views  
- `Models/` – Application data models  
- `Migrations/` – Database migration files (if using EF Core)  
- `wwwroot/` – Static files (CSS, JS, etc.)  
- `Program.cs` – Entry point for the app  
- `ChatArea.cs` – Core logic for the chat feature  

## ⚙️ Technologies Used

- C#  
- ASP.NET Core (Razor Pages)  
- Entity Framework Core (if applicable)  
- xUnit (for unit testing)  
- Visual Studio  

## ✅ Features

- Real-time chat functionality  
- Message display area with basic styling  
- Server-side processing with Razor Pages  
- Unit testing support via `Assignment4ChatTests`  

## 🚀 Getting Started

1. **Clone the repo**
   ```bash
   git clone https://github.com/SamiNachwati/nodechat.git
   cd nodechat
   ```

2. **Open the solution in Visual Studio**  
   Open `Assignment4ChatApplication.sln`

3. **Run the application**  
   Hit `F5` or click the `Run` button in Visual Studio

4. **(Optional) Run unit tests**  
   Navigate to the `Test` tab in Visual Studio and run the tests under `Assignment4ChatTests`

## 🧪 Sample Test Case

`UnitTest1.cs` includes sample tests such as:
- Verifying message parsing  
- Ensuring proper message formatting  
- Simulating user input  

## 📌 Notes

- This project was developed as part of a course assignment and is intended for educational/demonstration use.  
- It does not currently include authentication or persistent storage.  

## 📄 License

This project is open source and available under the [MIT License](LICENSE).
