Module: Student Attendance Management

Context:
- Attendance is taken daily per class
- Teachers mark attendance by listing students in a class

Responsibilities:
- Daily attendance records per student
- One attendance record per student per day
- Attendance includes status: Present / Absent / Excused
- Attendance affects meal ticket generation

Rules:
- Enforce uniqueness (StudentId + Date)
- Attendance logic must be in BLO
- Use DateOnly or DateTime normalized to date
- No duplicate attendance records allowed

Optimize EF Core queries to avoid N+1 issues.
