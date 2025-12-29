using Microsoft.EntityFrameworkCore;
using KindergartenManagement.DTO;

namespace KindergartenManagement.DAO;

public class KindergartenDbContext : DbContext
{
    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Permission> Permissions { get; set; } = null!;
    public DbSet<RolePermission> RolePermissions { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Parent> Parents { get; set; } = null!;
    public DbSet<Student> Students { get; set; } = null!;
    public DbSet<Class> Classes { get; set; } = null!;
    public DbSet<Grade> Grades { get; set; } = null!;
    public DbSet<Attendance> Attendances { get; set; } = null!;
    public DbSet<HealthRecord> HealthRecords { get; set; } = null!;
    public DbSet<Food> Foods { get; set; } = null!;
    public DbSet<Menu> Menus { get; set; } = null!;
    public DbSet<MenuFood> MenuFoods { get; set; } = null!;
    public DbSet<MealTicket> MealTickets { get; set; } = null!;
    public DbSet<Invoice> Invoices { get; set; } = null!;
    public DbSet<StaffLeave> StaffLeaves { get; set; } = null!;
    public DbSet<Vaccine> Vaccines { get; set; } = null!;
    public DbSet<VaccinationRecord> VaccinationRecords { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=kindergarten.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // RolePermission composite key
        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId);

        // MenuFood composite key
        modelBuilder.Entity<MenuFood>()
            .HasKey(mf => new { mf.MenuId, mf.FoodId });

        modelBuilder.Entity<MenuFood>()
            .HasOne(mf => mf.Menu)
            .WithMany(m => m.MenuFoods)
            .HasForeignKey(mf => mf.MenuId);

        modelBuilder.Entity<MenuFood>()
            .HasOne(mf => mf.Food)
            .WithMany(f => f.MenuFoods)
            .HasForeignKey(mf => mf.FoodId);

        // Account-Role relationship
        modelBuilder.Entity<Account>()
            .HasOne(a => a.Role)
            .WithMany(r => r.Accounts)
            .HasForeignKey(a => a.RoleId);

        // Account-User relationship
        modelBuilder.Entity<Account>()
            .HasOne(a => a.User)
            .WithOne(u => u.Account)
            .HasForeignKey<Account>(a => a.UserId);

        // Student-Parent relationship
        modelBuilder.Entity<Student>()
            .HasOne(s => s.Parent)
            .WithMany(p => p.Students)
            .HasForeignKey(s => s.ParentId);

        // Student-Class relationship
        modelBuilder.Entity<Student>()
            .HasOne(s => s.Class)
            .WithMany(c => c.Students)
            .HasForeignKey(s => s.ClassId);

        // Class-Grade relationship
        modelBuilder.Entity<Class>()
            .HasOne(c => c.Grade)
            .WithMany(g => g.Classes)
            .HasForeignKey(c => c.GradeId);

        // StaffLeave-User relationship
        modelBuilder.Entity<StaffLeave>()
            .HasOne(sl => sl.User)
            .WithMany(u => u.StaffLeaves)
            .HasForeignKey(sl => sl.UserId);

        // VaccinationRecord-Student relationship
        modelBuilder.Entity<VaccinationRecord>()
            .HasOne(vr => vr.Student)
            .WithMany()
            .HasForeignKey(vr => vr.StudentId);

        // VaccinationRecord-Vaccine relationship
        modelBuilder.Entity<VaccinationRecord>()
            .HasOne(vr => vr.Vaccine)
            .WithMany(v => v.VaccinationRecords)
            .HasForeignKey(vr => vr.VaccineId);

        // HealthRecord unique constraint (Student + Month + Year)
        modelBuilder.Entity<HealthRecord>()
            .HasIndex(hr => new { hr.StudentId, hr.Month, hr.Year })
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}
