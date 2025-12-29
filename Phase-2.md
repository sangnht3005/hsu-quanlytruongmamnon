Module: Grade, ClassRoom & Student Management

Context:
- Kindergarten domain
- WPF MVVM + EF Core + SQLite
- 3-layer architecture

Responsibilities:
- Grade management (khối lớp)
- ClassRoom management (gắn khối, giáo viên chủ nhiệm)
- Student management
- Student belongs to exactly one ClassRoom
- Student may have allergy ingredient codes
- Assign homeroom teacher to ClassRoom

Rules:
- Use Guid for student identity
- Define proper EF Core relationships
- Validate class existence before assigning students
- Business validation must be in BLO
- Use ObservableCollection for UI binding

Generate clean, maintainable code.
