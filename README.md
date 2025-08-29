
# 📌 Manage Tasks - ASP.NET Core + EF Core

## 🚀 Giới thiệu
Dự án này là một ứng dụng **quản lý công việc và dự án**, được xây dựng bằng **ASP.NET Core 9** và **Entity Framework Core**.  
Khi chạy lần đầu, ứng dụng sẽ **tự động migrate database và seed dữ liệu mặc định** (roles, permissions, task priorities, statuses, ...).

---

## 🛠️ Yêu cầu hệ thống
- [✅ .NET 9 SDK](https://dotnet.microsoft.com/download)
- ✅ SQL Server (có thể cài bản Developer hoặc chạy bằng Docker)
- ✅ Git

---

## 📥 Cách cài đặt & chạy

### 1. Clone repo
```bash
git clone https://github.com/your-username/manage-tasks.git
cd manage-tasks
````

### 2. Khôi phục thư viện NuGet

```bash
dotnet restore
```

### 3. Cấu hình database

Mở file `appsettings.json` và chỉnh lại `ConnectionStrings:DefaultConnection` phù hợp với SQL Server trên máy bạn.
Ví dụ:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=MangeTasksDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
}
```

### 4. Tạo database & áp dụng migrations


```bash
dotnet ef database update
```


### 5. Chạy ứng dụng

```bash
dotnet run
```


---

## 📦 Seed dữ liệu mặc định

Khi ứng dụng chạy, file `DbInitializer` sẽ tự động:

* Tạo **Roles**: `Admin`, `Member`, `Viewer`
* Tạo **Permissions**: (CreateTask, UpdateTask, ViewTask, ...)
* Gán quyền cho từng role
* Tạo **Task Priorities**: High, Medium, Low
* Tạo **Statuses** mặc định cho mỗi Project: Todo, InProgress, Review, Done, Blocked, Cancelled

---

## 👨‍💻 Dev notes

* Code chính nằm trong thư mục `Server/`.
* API endpoints được định nghĩa trong các `Controllers/`.
* Khi thay đổi model, cần chạy:

  ```bash
  dotnet ef migrations add YourMigrationName
  dotnet ef database update
  ```

---

## 📄 License

MIT License. Feel free to use and modify.

