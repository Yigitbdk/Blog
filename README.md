# 📖 Blog App

**Blog App** is a full-stack blog platform where users can create posts, browse by categories, and securely log in using an authentication system.
This project was built to practice both **.NET backend development** and **modern React frontend** design, implementing the core features of a real-world blog application.

## 🚀 Technologies

### Frontend
- ⚛️ **React** — Single Page Application (SPA)
- 🎨 **Tailwind CSS** — Modern and responsive styling

### Backend
- 🖥️ **ASP.NET Core** — 3-layer architecture (API, Business, Data)
- 🗄️ **Entity Framework Core** — ORM for database operations
- 🔑 **ASP.NET Identity** — User and role management
- 🍪 **Cookie Authentication** — Session management
- ✅ **FluentValidation** — Input and data validation

## ✨ Features
- 👤 User registration and login
- 🔐 Role-based authorization (Admin & User)
- 📝 Create, view, and manage blog posts
- 🏷️ Category-based content filtering
- 📱 Responsive UI (mobile & desktop support)

## 🏗️ Project Architecture
The project follows a **3-layer architecture**:

```
BlogApp/
│
├── BlogApp.Api/               # Presentation Layer (Controllers, API)
├── BlogApp.Business/          # Business Layer (Services, Validation)
├── BlogApp.Data/              # Data Layer (DbContext, Repositories)
```

## ⚙️ Installation

### Backend Setup
1. Update `appsettings.json` with your SQL connection string
2. Run migrations and update the database:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
3. Start the API:
   ```bash
   dotnet run
   ```

### Frontend Setup
```bash
cd BlogApp.Frontend
npm install
npm run dev
```

👉 **Open in browser:** `http://localhost:5173`

## 🔑 Default Account
- **Email:** `admin@blog.com`
- **Password:** `Admin123*`

## 🎯 Purpose

The purpose of this project is to combine **.NET backend skills** with **modern SPA frontend development**, while learning the fundamentals of:

- Layered architecture
- ASP.NET Identity authentication
- React + Tailwind UI design
- Backend–frontend integration

## 🚧 Development Status
⚠️ **Note:** The frontend is not fully completed and is still under active development.

## 📄 License
This project is licensed under the MIT License.
### 📧 Contact
If you have any questions about this project, feel free to reach out!
