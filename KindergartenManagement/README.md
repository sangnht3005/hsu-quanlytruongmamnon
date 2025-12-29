# Hệ thống quản lý mầm non (Kindergarten Management System)

## Giới thiệu
Ứng dụng WPF quản lý mầm non với giao diện thân thiện, được xây dựng theo kiến trúc 3-tier/3-layer với MVVM pattern.

## Công nghệ sử dụng
- **.NET 8.0** - Framework chính
- **WPF** - Giao diện desktop
- **MVVM Pattern** - Kiến trúc presentation
- **Entity Framework Core** - ORM
- **SQLite** - Database
- **BCrypt.Net** - Mã hóa mật khẩu
- **Dependency Injection** - Microsoft.Extensions.DependencyInjection

## Kiến trúc

### 3-Tier Architecture
1. **Presentation Tier** (Views + ViewModels)
   - Giao diện người dùng XAML
   - ViewModels xử lý UI logic
   - Commands và data binding

2. **Business Tier** (BLO - Business Logic Objects)
   - Xử lý business rules
   - Validation
   - Permission checks
   - Tính toán nghiệp vụ

3. **Data Tier** (DAO - Data Access Objects)
   - Truy cập database qua Entity Framework Core
   - Repository pattern
   - CRUD operations

### Các lớp dữ liệu (DTOs)
- Account, Role, Permission
- User (Staff), Parent, Student
- Class, Grade
- Attendance, HealthRecord
- Food, Menu, MealTicket
- Invoice

## Chức năng chính

### 1. Quản lý tài khoản và phân quyền
- Đăng nhập/Đăng xuất
- Quản lý tài khoản người dùng
- Phân quyền dựa trên Role-Permission

### 2. Quản lý học sinh
- Thêm/Sửa/Xóa thông tin học sinh
- Gán học sinh vào lớp
- Quản lý thông tin phụ huynh

### 3. Quản lý lớp học
- Tạo và quản lý lớp học
- Gán giáo viên cho lớp
- Theo dõi sĩ số lớp

### 4. Điểm danh
- Điểm danh hàng ngày theo lớp
- Theo dõi lịch sử điểm danh
- Thống kê tỷ lệ đi học

### 5. Quản lý sức khỏe
- Theo dõi chiều cao, cân nặng
- Lịch sử tiêm chủng
- Ghi chú tình trạng sức khỏe

### 6. Quản lý thực đơn
- Tạo thực đơn theo ngày
- Quản lý món ăn và nguyên liệu
- Tạo phiếu ăn tự động theo điểm danh

### 7. Quản lý hóa đơn
- Hóa đơn học phí
- Hóa đơn lương
- Hóa đơn chi phí hoạt động

## Cách chạy ứng dụng

### Yêu cầu
- Visual Studio 2022 hoặc mới hơn
- .NET 8.0 SDK

### Các bước thực hiện

1. **Mở project trong Visual Studio**
   ```
   Mở file KindergartenManagement.csproj
   ```

2. **Restore NuGet packages**
   ```
   Visual Studio sẽ tự động restore packages khi mở project
   Hoặc chạy: dotnet restore
   ```

3. **Build project**
   ```
   Build > Build Solution (Ctrl+Shift+B)
   Hoặc: dotnet build
   ```

4. **Chạy ứng dụng**
   ```
   Debug > Start Debugging (F5)
   Hoặc: dotnet run
   ```

## Thông tin đăng nhập mặc định

Sau khi chạy lần đầu, database sẽ được tạo tự động với dữ liệu mẫu:

**Tài khoản Admin:**
- Username: `admin`
- Password: `admin123`

## Cấu trúc thư mục

```
KindergartenManagement/
├── DTO/                    # Data Transfer Objects (Entities)
├── DAO/                    # Data Access Objects
├── BLO/                    # Business Logic Objects
├── ViewModels/             # MVVM ViewModels
├── Views/                  # XAML Views
├── Styles/                 # XAML Resource Dictionaries
├── Converters/             # Value Converters
├── Utilities/              # Helper classes
├── App.xaml               # Application definition
└── App.xaml.cs            # Application startup & DI configuration
```

## Database

Database được lưu tại: `kindergarten.db` (SQLite) trong thư mục ứng dụng.

Để reset database, xóa file `kindergarten.db` và khởi động lại ứng dụng.

## Quy tắc coding

- PascalCase cho classes, methods, properties
- camelCase cho private fields, local variables
- Prefix 'I' cho interfaces
- Async methods kết thúc bằng 'Async'
- Sử dụng async/await cho database operations
- LINQ cho query operations
- ViewModels KHÔNG truy cập DAO trực tiếp
- Mọi business logic phải qua BLO layer

## Tính năng bảo mật

- Mật khẩu được hash bằng BCrypt
- Role-based access control
- Validation ở BLO layer
- Kiểm tra permissions trước khi thực hiện actions

## Mở rộng

Để thêm chức năng mới:

1. Tạo DTO nếu cần entity mới
2. Tạo DAO interface và implementation
3. Tạo BLO interface và implementation
4. Tạo ViewModel
5. Tạo View (XAML)
6. Đăng ký services trong App.xaml.cs
7. Thêm navigation trong MainViewModel

## Liên hệ và hỗ trợ

Nếu có bất kỳ câu hỏi hoặc vấn đề nào, vui lòng liên hệ với nhóm phát triển.

---

**Lưu ý:** Đây là ứng dụng demo cho mục đích học tập. Trong môi trường production, cần thêm các tính năng bảo mật, logging, error handling và testing đầy đủ hơn.
