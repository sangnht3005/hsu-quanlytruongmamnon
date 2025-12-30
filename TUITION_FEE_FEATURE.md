# Tính Năng Quản Lý Học Phí - Tuition Fee Management

## Mô Tả (Description)
Tính năng này cho phép quản lý học phí và tiền ăn theo từng khối lớp (grade level). Khi tạo hóa đơn học phí hàng loạt, hệ thống sẽ tự động lấy mức học phí từ bảng cấu hình này.

## Cấu Trúc Dữ Liệu (Data Structure)

### TuitionFee Entity
```
- Id: Guid (Primary Key)
- GradeId: Guid (Foreign Key -> Grade)
- MonthlyTuitionFee: decimal (Học phí hàng tháng)
- DailyMealFee: decimal (Tiền ăn/ngày)
- SchoolDaysPerMonth: int (Số ngày học/tháng, mặc định 20)
- EffectiveDate: DateTime (Ngày có hiệu lực)
- CreatedAt: DateTime
- UpdatedAt: DateTime
```

## Các Thành Phần (Components)

### 1. Data Access Layer (DAO)
- **File**: `BLO/AllBlo.cs`
- **Class**: `ITuitionFeeDao`, `TuitionFeeDao`
- **Methods**:
  - `GetByGradeIdAsync(Guid gradeId)`: Lấy cấu hình học phí cho một khối lớp
  - `GetAllAsync()`: Lấy tất cả cấu hình học phí
  - `CreateAsync(TuitionFeeDto)`: Tạo cấu hình mới
  - `UpdateAsync(TuitionFeeDto)`: Cập nhật cấu hình
  - `DeleteAsync(Guid id)`: Xóa cấu hình

### 2. Business Logic Layer (BLO)
- **File**: `BLO/AllBlo.cs`
- **Classes**: `ITuitionFeeBlo`, `TuitionFeeBlo`
- **DTO Class**: `TuitionFeeDto`

### 3. User Interface (ViewModel & View)
- **ViewModel**: `ViewModels/TuitionFeeManagementViewModel.cs`
  - Properties: TuitionFees, Grades, SelectedTuitionFee, IsLoading
  - Commands: LoadDataCommand, AddTuitionFeeCommand, SaveTuitionFeeCommand, DeleteTuitionFeeCommand
  
- **UI Template**: MainWindow.xaml
  - Location: "⚙️ Cấu hình học phí và tiền ăn" in Finance section
  - Layout: Two-column (list of grades + edit form)
  - Features:
    - DataGrid showing list of grades with tuition/meal fees
    - Edit form for updating fees
    - Add/Save/Delete buttons

## Sử Dụng (Usage)

### 1. Truy Cập Tính Năng
- Click "💰 TÀI CHÍNH" → "⚙️ Cấu hình học phí" in sidebar

### 2. Thêm Cấu Hình Mới
- Click "➕ Thêm mới"
- Chọn khối lớp từ dropdown
- Nhập:
  - Học phí hàng tháng (VND)
  - Tiền ăn/ngày (VND)
  - Số ngày học/tháng (mặc định: 20)
  - Ngày có hiệu lực
- Click "💾 Lưu"

### 3. Chỉnh Sửa Cấu Hình
- Click vào hàng trong danh sách
- Form sẽ tự động populate
- Chỉnh sửa các trường cần thiết
- Click "💾 Lưu"

### 4. Xóa Cấu Hình
- Chọn cấu hình cần xóa
- Click "🗑️ Xóa"

## Tích Hợp Với Hóa Đơn (Invoice Integration)

### Khi Tạo Hóa Đơn Hàng Loạt (Batch Tuition Invoices)
Hệ thống sẽ:
1. Lấy tất cả lớp học
2. Với mỗi lớp, lấy thông tin khối (Grade)
3. Truy vấn bảng TuitionFee dựa trên GradeId
4. Sử dụng giá từ TuitionFee table:
   - `MonthlyTuitionFee` → Học phí cơ bản
   - `DailyMealFee × SchoolDaysPerMonth` → Tiền ăn hàng tháng
5. Tính toán trừ đi:
   - Tiền hoàn lại từ những ngày nghỉ có phép (refund)
   - Số dư từ tháng trước

### Công Thức Tính (Calculation)
```
TotalAmount = MonthlyTuitionFee + (DailyMealFee × SchoolDaysPerMonth) - CurrentMonthRefund - PreviousMonthRefund
```

## Fallback Logic
Nếu không có cấu hình TuitionFee cho khối lớp, hệ thống sẽ fallback sang:
- `Class.TuitionFee` và `Class.MealFee` (nếu tồn tại)

## Database Migration
- **Migration Name**: `AddTuitionFeeTable`
- **Status**: Applied
- **Changes**: Tạo bảng TuitionFee với tất cả cột cần thiết

## Nhập Liệu Khởi Tạo (Initial Data)
Hiện tại, bảng TuitionFee trống. Quản trị viên cần:
1. Truy cập "⚙️ Cấu hình học phí"
2. Thêm mới cấu hình cho từng khối lớp
3. Cập nhật khi có thay đổi mức học phí

## Ứng Dụng Thực Tiễn (Real-world Usage)
```
Ví dụ:
- Khối A: 5,000,000đ/tháng + 100,000đ/ngày × 20 ngày = 7,000,000đ
- Khối B: 4,500,000đ/tháng + 90,000đ/ngày × 20 ngày = 6,300,000đ
- Khối C: 4,000,000đ/tháng + 80,000đ/ngày × 20 ngày = 5,600,000đ

Khi tạo hóa đơn tháng 3/2024:
- Học sinh lớp A1 (khối A) - 0 ngày nghỉ phép: 7,000,000đ
- Học sinh lớp B2 (khối B) - 2 ngày nghỉ phép: 6,300,000 - 180,000 = 6,120,000đ
```

## Lợi Ích (Benefits)
1. ✅ Quản lý linh hoạt mức học phí theo khối lớp
2. ✅ Tự động áp dụng khi tạo hóa đơn
3. ✅ Dễ dàng cập nhật mức phí mà không ảnh hưởng dữ liệu cũ
4. ✅ Hỗ trợ thay đổi tiền ăn theo ngày học thực tế
5. ✅ Giảm lỗi tính toán thủ công
