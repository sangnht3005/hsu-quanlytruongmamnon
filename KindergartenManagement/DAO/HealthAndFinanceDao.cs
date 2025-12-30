using Microsoft.EntityFrameworkCore;
using KindergartenManagement.DTO;

namespace KindergartenManagement.DAO;

public interface IHealthRecordDao
{
    Task<HealthRecord?> GetByIdAsync(Guid id);
    Task<IEnumerable<HealthRecord>> GetAllAsync();
    Task<IEnumerable<HealthRecord>> GetByStudentIdAsync(Guid studentId);
    Task<HealthRecord?> GetByStudentAndMonthAsync(Guid studentId, int month, int year);
    Task<HealthRecord> CreateAsync(HealthRecord healthRecord);
    Task<HealthRecord> UpdateAsync(HealthRecord healthRecord);
    Task DeleteAsync(Guid id);
}

public class HealthRecordDao : IHealthRecordDao
{
    private readonly KindergartenDbContext _context;

    public HealthRecordDao(KindergartenDbContext context)
    {
        _context = context;
    }

    public async Task<HealthRecord?> GetByIdAsync(Guid id)
    {
        return await _context.HealthRecords
            .Include(h => h.Student)
            .FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task<IEnumerable<HealthRecord>> GetAllAsync()
    {
        return await _context.HealthRecords
            .Include(h => h.Student)
            .OrderByDescending(h => h.Year)
            .ThenByDescending(h => h.Month)
            .ToListAsync();
    }

    public async Task<IEnumerable<HealthRecord>> GetByStudentIdAsync(Guid studentId)
    {
        return await _context.HealthRecords
            .Include(h => h.Student)
            .Where(h => h.StudentId == studentId)
            .OrderByDescending(h => h.Year)
            .ThenByDescending(h => h.Month)
            .ToListAsync();
    }

    public async Task<HealthRecord?> GetByStudentAndMonthAsync(Guid studentId, int month, int year)
    {
        return await _context.HealthRecords
            .Include(h => h.Student)
            .FirstOrDefaultAsync(h => h.StudentId == studentId && h.Month == month && h.Year == year);
    }

    public async Task<HealthRecord> CreateAsync(HealthRecord healthRecord)
    {
        _context.HealthRecords.Add(healthRecord);
        await _context.SaveChangesAsync();
        return healthRecord;
    }

    public async Task<HealthRecord> UpdateAsync(HealthRecord healthRecord)
    {
        healthRecord.UpdatedAt = DateTime.Now;
        _context.HealthRecords.Update(healthRecord);
        await _context.SaveChangesAsync();
        return healthRecord;
    }

    public async Task DeleteAsync(Guid id)
    {
        var healthRecord = await _context.HealthRecords.FindAsync(id);
        if (healthRecord != null)
        {
            _context.HealthRecords.Remove(healthRecord);
            await _context.SaveChangesAsync();
        }
    }
}

public interface IFoodDao
{
    Task<Food?> GetByIdAsync(Guid id);
    Task<IEnumerable<Food>> GetAllAsync();
    Task<Food> CreateAsync(Food food);
    Task<Food> UpdateAsync(Food food);
    Task DeleteAsync(Guid id);
}

public class FoodDao : IFoodDao
{
    private readonly KindergartenDbContext _context;

    public FoodDao(KindergartenDbContext context)
    {
        _context = context;
    }

    public async Task<Food?> GetByIdAsync(Guid id)
    {
        return await _context.Foods.FindAsync(id);
    }

    public async Task<IEnumerable<Food>> GetAllAsync()
    {
        return await _context.Foods.ToListAsync();
    }

    public async Task<Food> CreateAsync(Food food)
    {
        _context.Foods.Add(food);
        await _context.SaveChangesAsync();
        return food;
    }

    public async Task<Food> UpdateAsync(Food food)
    {
        food.UpdatedAt = DateTime.Now;
        _context.Foods.Update(food);
        await _context.SaveChangesAsync();
        return food;
    }

    public async Task DeleteAsync(Guid id)
    {
        var food = await _context.Foods.FindAsync(id);
        if (food != null)
        {
            _context.Foods.Remove(food);
            await _context.SaveChangesAsync();
        }
    }
}

public interface IMenuDao
{
    Task<Menu?> GetByIdAsync(Guid id);
    Task<IEnumerable<Menu>> GetAllAsync();
    Task<IEnumerable<Menu>> GetByDayOfWeekAsync(int dayOfWeek);
    Task<Menu> CreateAsync(Menu menu);
    Task<Menu> UpdateAsync(Menu menu);
    Task DeleteAsync(Guid id);
}

public class MenuDao : IMenuDao
{
    private readonly KindergartenDbContext _context;

    public MenuDao(KindergartenDbContext context)
    {
        _context = context;
    }

    public async Task<Menu?> GetByIdAsync(Guid id)
    {
        return await _context.Menus
            .Include(m => m.MenuDishes)
            .ThenInclude(md => md.Dish)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<Menu>> GetAllAsync()
    {
        return await _context.Menus
            .Include(m => m.MenuDishes)
            .ThenInclude(md => md.Dish)
            .OrderBy(m => m.DayOfWeek)
            .ThenBy(m => m.MealType)
            .ToListAsync();
    }

    public async Task<IEnumerable<Menu>> GetByDayOfWeekAsync(int dayOfWeek)
    {
        return await _context.Menus
            .Include(m => m.MenuDishes)
            .ThenInclude(md => md.Dish)
            .Where(m => m.DayOfWeek == dayOfWeek)
            .ToListAsync();
    }

    public async Task<Menu> CreateAsync(Menu menu)
    {
        _context.Menus.Add(menu);
        await _context.SaveChangesAsync();
        return menu;
    }

    public async Task<Menu> UpdateAsync(Menu menu)
    {
        menu.UpdatedAt = DateTime.Now;
        _context.Menus.Update(menu);
        await _context.SaveChangesAsync();
        return menu;
    }

    public async Task DeleteAsync(Guid id)
    {
        var menu = await _context.Menus.FindAsync(id);
        if (menu != null)
        {
            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();
        }
    }
}

public interface IMealTicketDao
{
    Task<MealTicket?> GetByIdAsync(Guid id);
    Task<IEnumerable<MealTicket>> GetByStudentIdAsync(Guid studentId);
    Task<IEnumerable<MealTicket>> GetByDateAsync(DateTime date);
    Task<MealTicket> CreateAsync(MealTicket mealTicket);
    Task<MealTicket> UpdateAsync(MealTicket mealTicket);
    Task DeleteAsync(Guid id);
}

public class MealTicketDao : IMealTicketDao
{
    private readonly KindergartenDbContext _context;

    public MealTicketDao(KindergartenDbContext context)
    {
        _context = context;
    }

    public async Task<MealTicket?> GetByIdAsync(Guid id)
    {
        return await _context.MealTickets
            .Include(mt => mt.Student)
            .Include(mt => mt.Menu)
            .FirstOrDefaultAsync(mt => mt.Id == id);
    }

    public async Task<IEnumerable<MealTicket>> GetByStudentIdAsync(Guid studentId)
    {
        return await _context.MealTickets
            .Include(mt => mt.Student)
            .Include(mt => mt.Menu)
            .Where(mt => mt.StudentId == studentId)
            .OrderByDescending(mt => mt.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<MealTicket>> GetByDateAsync(DateTime date)
    {
        return await _context.MealTickets
            .Include(mt => mt.Student)
            .Include(mt => mt.Menu)
            .Where(mt => mt.Date.Date == date.Date)
            .ToListAsync();
    }

    public async Task<MealTicket> CreateAsync(MealTicket mealTicket)
    {
        _context.MealTickets.Add(mealTicket);
        await _context.SaveChangesAsync();
        return mealTicket;
    }

    public async Task<MealTicket> UpdateAsync(MealTicket mealTicket)
    {
        mealTicket.UpdatedAt = DateTime.Now;
        _context.MealTickets.Update(mealTicket);
        await _context.SaveChangesAsync();
        return mealTicket;
    }

    public async Task DeleteAsync(Guid id)
    {
        var mealTicket = await _context.MealTickets.FindAsync(id);
        if (mealTicket != null)
        {
            _context.MealTickets.Remove(mealTicket);
            await _context.SaveChangesAsync();
        }
    }
}

public interface IInvoiceDao
{
    Task<Invoice?> GetByIdAsync(Guid id);
    Task<IEnumerable<Invoice>> GetAllAsync();
    Task<IEnumerable<Invoice>> GetByStudentIdAsync(Guid studentId);
    Task<IEnumerable<Invoice>> GetByUserIdAsync(Guid userId);
    Task<Invoice> CreateAsync(Invoice invoice);
    Task<Invoice> UpdateAsync(Invoice invoice);
    Task DeleteAsync(Guid id);
}

public class InvoiceDao : IInvoiceDao
{
    private readonly KindergartenDbContext _context;

    public InvoiceDao(KindergartenDbContext context)
    {
        _context = context;
    }

    public async Task<Invoice?> GetByIdAsync(Guid id)
    {
        return await _context.Invoices
            .Include(i => i.Student)
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync()
    {
        try
        {
            var result = await _context.Invoices
                .Include(i => i.Student)
                .Include(i => i.User)
                .OrderByDescending(i => i.IssueDate)
                .ToListAsync();
            System.Diagnostics.Debug.WriteLine($"InvoiceDao.GetAllAsync: Loaded {result?.Count ?? 0} invoices");
            return result ?? new List<Invoice>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"InvoiceDao.GetAllAsync error: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<Invoice>> GetByStudentIdAsync(Guid studentId)
    {
        return await _context.Invoices
            .Include(i => i.Student)
            .Where(i => i.StudentId == studentId)
            .OrderByDescending(i => i.IssueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Invoices
            .Include(i => i.User)
            .Where(i => i.UserId == userId)
            .OrderByDescending(i => i.IssueDate)
            .ToListAsync();
    }

    public async Task<Invoice> CreateAsync(Invoice invoice)
    {
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();
        return invoice;
    }

    public async Task<Invoice> UpdateAsync(Invoice invoice)
    {
        invoice.UpdatedAt = DateTime.Now;
        _context.Invoices.Update(invoice);
        await _context.SaveChangesAsync();
        return invoice;
    }

    public async Task DeleteAsync(Guid id)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice != null)
        {
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
        }
    }
}

// Vaccine DAO
public interface IVaccineDao
{
    Task<Vaccine?> GetByIdAsync(Guid id);
    Task<IEnumerable<Vaccine>> GetAllAsync();
    Task<IEnumerable<Vaccine>> GetMandatoryVaccinesAsync();
    Task<Vaccine> CreateAsync(Vaccine vaccine);
    Task<Vaccine> UpdateAsync(Vaccine vaccine);
    Task DeleteAsync(Guid id);
}

public class VaccineDao : IVaccineDao
{
    private readonly KindergartenDbContext _context;

    public VaccineDao(KindergartenDbContext context)
    {
        _context = context;
    }

    public async Task<Vaccine?> GetByIdAsync(Guid id)
    {
        return await _context.Vaccines.FindAsync(id);
    }

    public async Task<IEnumerable<Vaccine>> GetAllAsync()
    {
        return await _context.Vaccines
            .OrderBy(v => v.RequiredAgeMonths)
            .ToListAsync();
    }

    public async Task<IEnumerable<Vaccine>> GetMandatoryVaccinesAsync()
    {
        return await _context.Vaccines
            .Where(v => v.IsMandatory)
            .OrderBy(v => v.RequiredAgeMonths)
            .ToListAsync();
    }

    public async Task<Vaccine> CreateAsync(Vaccine vaccine)
    {
        _context.Vaccines.Add(vaccine);
        await _context.SaveChangesAsync();
        return vaccine;
    }

    public async Task<Vaccine> UpdateAsync(Vaccine vaccine)
    {
        vaccine.UpdatedAt = DateTime.Now;
        _context.Vaccines.Update(vaccine);
        await _context.SaveChangesAsync();
        return vaccine;
    }

    public async Task DeleteAsync(Guid id)
    {
        var vaccine = await _context.Vaccines.FindAsync(id);
        if (vaccine != null)
        {
            _context.Vaccines.Remove(vaccine);
            await _context.SaveChangesAsync();
        }
    }
}

// VaccinationRecord DAO
public interface IVaccinationRecordDao
{
    Task<VaccinationRecord?> GetByIdAsync(Guid id);
    Task<IEnumerable<VaccinationRecord>> GetAllAsync();
    Task<IEnumerable<VaccinationRecord>> GetByStudentIdAsync(Guid studentId);
    Task<IEnumerable<VaccinationRecord>> GetByVaccineIdAsync(Guid vaccineId);
    Task<VaccinationRecord?> GetByStudentAndVaccineAsync(Guid studentId, Guid vaccineId);
    Task<VaccinationRecord> CreateAsync(VaccinationRecord record);
    Task<VaccinationRecord> UpdateAsync(VaccinationRecord record);
    Task DeleteAsync(Guid id);
}

public class VaccinationRecordDao : IVaccinationRecordDao
{
    private readonly KindergartenDbContext _context;

    public VaccinationRecordDao(KindergartenDbContext context)
    {
        _context = context;
    }

    public async Task<VaccinationRecord?> GetByIdAsync(Guid id)
    {
        return await _context.VaccinationRecords
            .Include(vr => vr.Student)
            .Include(vr => vr.Vaccine)
            .FirstOrDefaultAsync(vr => vr.Id == id);
    }

    public async Task<IEnumerable<VaccinationRecord>> GetAllAsync()
    {
        return await _context.VaccinationRecords
            .Include(vr => vr.Student)
            .Include(vr => vr.Vaccine)
            .OrderBy(vr => vr.Student!.FullName)
            .ThenBy(vr => vr.Vaccine!.RequiredAgeMonths)
            .ToListAsync();
    }

    public async Task<IEnumerable<VaccinationRecord>> GetByStudentIdAsync(Guid studentId)
    {
        return await _context.VaccinationRecords
            .Include(vr => vr.Student)
            .Include(vr => vr.Vaccine)
            .Where(vr => vr.StudentId == studentId)
            .OrderBy(vr => vr.Vaccine!.RequiredAgeMonths)
            .ToListAsync();
    }

    public async Task<IEnumerable<VaccinationRecord>> GetByVaccineIdAsync(Guid vaccineId)
    {
        return await _context.VaccinationRecords
            .Include(vr => vr.Student)
            .Include(vr => vr.Vaccine)
            .Where(vr => vr.VaccineId == vaccineId)
            .OrderBy(vr => vr.Student!.FullName)
            .ToListAsync();
    }

    public async Task<VaccinationRecord?> GetByStudentAndVaccineAsync(Guid studentId, Guid vaccineId)
    {
        return await _context.VaccinationRecords
            .Include(vr => vr.Student)
            .Include(vr => vr.Vaccine)
            .FirstOrDefaultAsync(vr => vr.StudentId == studentId && vr.VaccineId == vaccineId);
    }

    public async Task<VaccinationRecord> CreateAsync(VaccinationRecord record)
    {
        _context.VaccinationRecords.Add(record);
        await _context.SaveChangesAsync();
        return record;
    }

    public async Task<VaccinationRecord> UpdateAsync(VaccinationRecord record)
    {
        record.UpdatedAt = DateTime.Now;
        _context.VaccinationRecords.Update(record);
        await _context.SaveChangesAsync();
        return record;
    }

    public async Task DeleteAsync(Guid id)
    {
        var record = await _context.VaccinationRecords.FindAsync(id);
        if (record != null)
        {
            _context.VaccinationRecords.Remove(record);
            await _context.SaveChangesAsync();
        }
    }
}
