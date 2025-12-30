# 🎯 PRACTICAL GUIDE: How to Use & Optimize the Application

## 📖 Table of Contents
1. [Getting Started](#getting-started)
2. [Navigation Guide](#navigation-guide)
3. [Common Tasks](#common-tasks)
4. [Tips & Tricks](#tips--tricks)
5. [Troubleshooting](#troubleshooting)

---

## 🚀 Getting Started

### **First Time Login**
1. Launch the application
2. Enter username and password
3. Click "Đăng nhập" (Login)
4. You'll see the Dashboard with quick stats

### **Interface Overview**
```
┌─────────────────────────────────┐
│  Left Sidebar (Menu)  │ Main Content Area │
│                       │                   │
│  📊 Tổng quan        │  Screen Content   │
│  👶 Học sinh         │  (Changes based   │
│  🏫 Lớp học          │   on menu click)  │
│  💊 Sức khỏe         │                   │
│  🍽️ Thực đơn         │ 💾 Buttons        │
│  👨‍💼 Nhân sự         │ (Bottom/Sticky)   │
│  💰 Tài chính        │                   │
│  📈 Báo cáo          │                   │
│  🔐 Hệ thống         │                   │
└─────────────────────────────────┘
```

---

## 🗺️ Navigation Guide

### **Sidebar Menu Structure**

#### **📊 Dashboard (Tổng quan)**
- Overview of key statistics
- Recent activities
- Quick access to important sections

#### **👶 HỌC SINH (Students & Parents)**
- **Danh sách học sinh** - Manage student information
- **Phụ huynh** - Manage parent information

#### **🏫 LỚP HỌC (Classes & Attendance)**
- **Khối lớp** - Configure grade levels (K1, K2, K3)
- **Quản lý lớp** - Manage individual classes
- **Điểm danh** - Take daily attendance

#### **💊 SỨC KHỎE (Health)**
- **Hồ sơ sức khỏe** - Health records
- **Vaccine** - Vaccine management
- **Tiêm chủng** - Vaccination records
- **Hồ sơ tháng** - Monthly health reports

#### **🍽️ THỰC ĐƠN (Nutrition & Supplies)**
- **Thực đơn** - Dish management
- **Menu hằng ngày** - Daily menu planning
- **Phiếu ăn** - Meal tickets
- **Nhà cung cấp** - Supplier management
- **Nguyên liệu** - Ingredient inventory
- **Món ăn** - Food dishes

#### **👨‍💼 NHÂN SỰ (Staff)**
- **Nhân viên** - Staff information
- **Đơn nghỉ** - Leave requests

#### **💰 TÀI CHÍNH (Finance)**
- **Hóa đơn** - Invoices
- **Cấu hình học phí** - Tuition fee configuration

#### **📈 BÁO CÁO (Reports)**
- Generate and view reports

#### **🔐 HỆ THỐNG (System)**
- **Tài khoản** - Account management

---

## 📝 Common Tasks

### **Task 1: Add a New Student**
```
Step 1: Click "👶 Danh sách học sinh" in menu
Step 2: Click "➕ Tạo mới" button
Step 3: Fill in form fields:
        • Họ và tên (Full name)
        • Ngày sinh (Date of birth)
        • Giới tính (Gender)
        • Địa chỉ (Address) - optional
        • Ghi chú y tế (Medical notes)
        • Mã dị ứng (Allergy codes)
        • Phụ huynh (Parent)
        • Lớp (Class)
Step 4: Click "💾 Lưu" to save
Step 5: Confirmation message appears
```

**Pro Tip:** Hover over field labels for help text (*)

---

### **Task 2: Take Daily Attendance**
```
Step 1: Click "🏫 Điểm danh"
Step 2: Select class and date
Step 3: Check/uncheck attendance for each student
Step 4: Click "💾 Lưu" to save
Step 5: System generates meal tickets automatically
```

**Pro Tip:** Use the red/green indicators for quick visual feedback

---

### **Task 3: Create Daily Menu**
```
Step 1: Click "🍽️ Menu hằng ngày"
Step 2: Click "➕ Thêm mới"
Step 3: Fill in:
        • Ngày trong tuần (Day of week)
        • Loại bữa (Meal type: Breakfast, Lunch, etc.)
        • Tên thực đơn (Menu name)
Step 4: Add dishes from dropdown
Step 5: Click "💾 Lưu"
Step 6: Use "🎫 Tạo phiếu" to auto-generate meal tickets
```

---

### **Task 4: Manage Student Grades**
```
Step 1: Click "🏫 Khối lớp"
Step 2: View/edit grade levels (K1, K2, K3)
Step 3: Click "➕ Tạo mới" to add new grade
Step 4: Enter:
        • Tên khối (Grade name)
        • Độ tuổi tối thiểu (Min age)
        • Độ tuổi tối đa (Max age)
        • Mô tả (Description)
Step 5: Click "💾 Lưu"
```

---

### **Task 5: Configure Tuition Fees**
```
Step 1: Click "💰 Cấu hình học phí"
Step 2: View/edit fees by grade
Step 3: For each grade:
        • Enter tuition amount
        • Enter mileage allowance
        • Enter other fees
Step 4: Click "💾 Lưu"
```

**Pro Tip:** Fees are linked to grades - change here updates all invoices

---

## 💡 Tips & Tricks

### **General**
1. **Placeholder Text** - Gray text in fields shows you what to enter
2. **Sticky Buttons** - Save/Delete buttons stay visible while scrolling
3. **Hover Help** - Hover over field labels to see tooltips
4. **Color Coding:**
   - 🟦 Blue button = Add/Create new item
   - 🟩 Green button = Save changes
   - 🟥 Red button = Delete item
   - ⚪ Gray button = Secondary action (Reload, Filter, etc.)

### **DataGrid (Tables)**
1. **Click column header to sort** - Sort ascending/descending
2. **Row highlighting:**
   - Hover over row → Light blue background
   - Select row → Darker blue background
3. **Alternating row colors** - Easier to read
4. **Search in first column** - Filter by typing

### **Forms**
1. **Fields with * are required** - Must fill before saving
2. **Red border = validation error** - Check field value
3. **Tab key** - Move between fields quickly
4. **Ctrl+S** - Save (future: keyboard shortcuts coming)

### **Menu**
1. **Emoji help identify sections** - 👶=Students, 🏫=Classes, etc.
2. **Recent items** - (Coming soon) Quick access to last used screens
3. **Search menu** - (Coming soon) Type to find menu item

---

## 🔧 Troubleshooting

### **"Form shows placeholder text but no input"**
- **Solution:** Click on the field and start typing. Placeholder disappears when you type.

### **"I can't find my data in the list"**
- **Solution:** 
  1. Check the DataGrid is not filtered
  2. Use search/filter if available
  3. Click column header to check sort order
  4. Reload data using "🔄 Tải lại" button

### **"Save button is out of view"**
- **Solution:** The button bar is sticky (stays at bottom). Try scrolling, or look for it at screen bottom. It won't disappear.

### **"Deleted data by accident"**
- **Note:** In newer versions, we'll ask for confirmation before delete. For now, contact admin to restore.

### **"Form validation fails but I don't know why"**
- **Solution:** 
  1. Look for red-bordered fields (invalid)
  2. Check that all * (required) fields are filled
  3. Read error message at top of screen
  4. Hover over field label for help

### **"Numbers look wrong (showing 123456 instead of 123,456)"**
- **Solution:** This is normal. Number format depends on system locale.

### **"Dates show wrong format"**
- **Solution:** Dates are shown as dd/MM/yyyy (European format). Example: 25/12/2023 = December 25, 2023

### **"Menu items are too small to read"**
- **Solution:** Font size has been increased. If still small:
  1. Check system display zoom (Windows zoom setting)
  2. Or contact admin about increasing app font size

### **"Performance is slow when loading large lists"**
- **Tip:**
  1. Use filters to reduce data shown
  2. Close other applications
  3. Try sorting by date (newest first) to see recent data first
  4. Contact admin if issue persists

---

## 📞 Getting Help

### **In-App Help**
- **Hover over field labels** - See tooltips explaining what each field is for
- **Red * (asterisk)** - Indicates required field
- **Gray text in fields** - Shows example of what to enter

### **Common Issues**
- **Can't save:** Check all required fields (*) are filled
- **Data not showing:** Use "🔄 Tải lại" to refresh
- **Can't find button:** Scroll down or check screen edges

### **Contact Admin/Support**
- Report bugs with screenshot
- Include exact steps to reproduce issue
- Note which screen/menu item has problem

---

## 🎓 Best Practices

### **Data Entry**
1. ✅ Fill required fields first (marked with *)
2. ✅ Use suggested formats (examples show in placeholder text)
3. ✅ Double-check data before saving
4. ✅ Save frequently (don't lose work)

### **Navigation**
1. ✅ Use menu to go to different screens
2. ✅ Use breadcrumb (when available) to see where you are
3. ✅ Use recent items (when available) for quick access
4. ✅ Click "🔄 Tải lại" if data doesn't match expected

### **Reports**
1. ✅ Generate reports for analysis
2. ✅ Filter by date range for specific period
3. ✅ Export to file for archiving
4. ✅ Review reports monthly for compliance

---

## 🚀 Upcoming Features

- ✅ Placeholder text examples (DONE)
- ✅ Button color standardization (DONE)
- ⏳ Collapsible menu sections (coming soon)
- ⏳ Keyboard shortcuts (coming soon)
- ⏳ Recent items menu (coming soon)
- ⏳ Data export to Excel (coming soon)
- ⏳ Mobile app (future)

---

## 📊 Quick Reference: Menu Map

```
QUICK ACCESS:
Shift + 1: Dashboard
Shift + 2: Students
Shift + 3: Classes
Shift + 4: Health
Shift + 5: Menu
Shift + 6: Staff
Shift + 7: Finance
Shift + 8: Reports
Shift + 9: System

COMMON SHORTCUTS (when implemented):
Ctrl+S: Save current form
Ctrl+N: Add new record
Ctrl+D: Delete record
Ctrl+F: Filter/Search
```

---

**Last Updated:** December 30, 2025  
**Version:** 1.0  
**Status:** Current UX Documentation  
**Feedback Welcome!** 📧
