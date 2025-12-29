Module: Account, Role & Permission Management

Context:
- WPF MVVM application
- 3-layer architecture (DTO, DAO, BLO)
- SQLite + EF Core

Responsibilities:
- Permission management (id, code, name, description)
- Role management with list of Permission IDs
- Account management (username, passwordHash, email, roleId)
- Staff (User) management with salary, allowance, specialization
- Parent management
- Staff leave tracking

Rules:
- Passwords must be hashed
- Role-based access control (RBAC)
- Permission checks must be implemented in BLO
- No UI logic or DbContext access in ViewModel
- Use async EF Core operations

Generate code appropriate to the current layer only.
