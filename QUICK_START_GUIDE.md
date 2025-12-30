# 📘 Hướng Dẫn Nhanh - Tuition Fee Management System

## 🎯 Mục Đích
Quản lý mức học phí và tiền ăn theo khối lớp, tự động áp dụng khi tạo hóa đơn.

## 🚀 Bắt Đầu

### Bước 1: Truy Cập Tính Năng
```
Sidebar → 💰 TÀI CHÍNH → ⚙️ Cấu hình học phí
```

### Bước 2: Thêm Cấu Hình
```
1. Click "➕ Thêm mới"
2. Chọn khối lớp (Grade)
3. Nhập mức học phí hàng tháng
4. Nhập tiền ăn/ngày
5. Đặt số ngày học/tháng (mặc định 20)
6. Chọn ngày có hiệu lực
7. Click "💾 Lưu"
```

## 💡 Ví Dụ Thực Tế

### Cấu Hình
```
Khối A:
- Học phí: 5,000,000đ/tháng
- Tiền ăn: 100,000đ/ngày
- Số ngày: 20 ngày/tháng
- Total: 5,000,000 + (100,000 × 20) = 7,000,000đ/tháng
```

### Khi Tạo Hóa Đơn
```
Học sinh X, Khối A:
- Mức cơ bản: 7,000,000đ
- Trừ: 2 ngày nghỉ phép = -200,000đ
- Trừ: Dư từ tháng trước = -100,000đ
- Hóa đơn cuối: 6,700,000đ
```

## 📊 Giao Diện

```
┌─────────────────────────────────────────────────────┐
│  ⚙️ Cấu hình học phí và tiền ăn                      │
├──────────────────────┬──────────────────────────────┤
│ Danh sách khối lớp   │ Thông tin học phí            │
│ ➕ Thêm mới          │ ────────────────────────────  │
│                      │ Chọn khối lớp: [Dropdown]    │
│ ┌─────────────────┐  │                              │
│ │Khối │ HP   │ TA │  │ Học phí/tháng: [_______]    │
│ ├─────┼──────┼───┤  │                              │
│ │ A   │5M    │100│  │ Tiền ăn/ngày: [_______]    │
│ │ B   │4.5M  │90 │  │                              │
│ │ C   │4M    │80 │  │ Số ngày học: [__]            │
│ └─────┴──────┴───┘  │                              │
│                      │ Ngày hiệu lực: [____________]│
│                      │                              │
│                      │ [💾 Lưu]  [🗑️ Xóa]          │
└──────────────────────┴──────────────────────────────┘
```

## ✨ Tính Năng Chính

| Tính Năng | Mô Tả | Cách Dùng |
|-----------|-------|----------|
| **Thêm** | Tạo cấu hình mới | Click "➕ Thêm mới" |
| **Sửa** | Chỉnh sửa cấu hình | Click hàng → Sửa → Lưu |
| **Xóa** | Xóa cấu hình cũ | Chọn → Click "🗑️ Xóa" |
| **Tự động áp dụng** | Áp vào hóa đơn | Tạo hóa đơn → Tự động lấy |

## 🔗 Liên Kết Với Hóa Đơn

### Tạo Hóa Đơn Tự Động
```
1. Truy cập: 💰 TÀI CHÍNH → Hóa đơn
2. Chọn tháng
3. Click "🔄 Tạo hóa đơn cho tất cả lớp"
4. Hệ thống:
   - Đọc cấu hình TuitionFee
   - Tính: Học phí + (Tiền ăn × Ngày)
   - Trừ: Ngày nghỉ có phép + Dư tháng trước
   - Tạo hóa đơn
```

## 📋 Kiểm Tra Nhanh

- [ ] Cấu hình TuitionFee đã được thêm
- [ ] Số tiền hóa đơn tính đúng
- [ ] Hoàn lại tiền từ ngày nghỉ
- [ ] Có thể chỉnh sửa, xóa cấu hình
- [ ] Fallback sang Class data nếu cần

## ❓ FAQ

**Q: Nếu không có cấu hình TuitionFee sẽ thế nào?**  
A: Hệ thống sẽ sử dụng mức phí từ Class (TuitionFee / MealFee)

**Q: Có thể cấu hình khác nhau theo tháng không?**  
A: Có! Sử dụng "Ngày có hiệu lực" để đặt thời gian áp dụng

**Q: Cách tính tiền ăn?**  
A: `DailyMealFee × SchoolDaysPerMonth` (ví dụ: 100,000 × 20 = 2,000,000đ)

**Q: Nếu thay đổi cấu hình sẽ ảnh hưởng hóa đơn cũ?**  
A: Không, chỉ ảnh hưởng hóa đơn tạo mới sau khi thay đổi

## 🛠️ Maintenance

### Cập nhật Mức Phí
```
1. Mở "⚙️ Cấu hình học phí"
2. Click hàng cần sửa
3. Thay đổi giá trị
4. Đặt "Ngày có hiệu lực" mới
5. Click "💾 Lưu"
6. Hóa đơn mới sẽ dùng giá mới
```

### Xóa Cấu Hình Cũ
```
1. Chọn cấu hình
2. Click "🗑️ Xóa"
3. Xác nhận xóa
```

## 💾 Dữ Liệu Lưu Trữ

```
Bảng: TuitionFee
┌────┬─────────┬──────────────────┬────────────┬──────────────┐
│ Id │ GradeId │ MonthlyTuitionFee│ DailyMealFee│ SchoolDaysPerMonth│
├────┼─────────┼──────────────────┼────────────┼──────────────┤
│ ...│  UUID   │   5,000,000      │  100,000   │      20      │
└────┴─────────┴──────────────────┴────────────┴──────────────┘
```

---

**Phiên bản:** 1.0  
**Cập nhật:** 2024-12-30  
**Trạng thái:** ✅ Production Ready
