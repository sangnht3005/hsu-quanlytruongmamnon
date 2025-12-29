using KindergartenManagement.DTO;
using KindergartenManagement.DAO;

namespace KindergartenManagement.BLO;

// Vaccine BLO
public interface IVaccineBlo
{
    Task<Vaccine?> GetByIdAsync(Guid id);
    Task<IEnumerable<Vaccine>> GetAllAsync();
    Task<IEnumerable<Vaccine>> GetMandatoryVaccinesAsync();
    Task<Vaccine> CreateAsync(Vaccine vaccine);
    Task<Vaccine> UpdateAsync(Vaccine vaccine);
    Task DeleteAsync(Guid id);
}

public class VaccineBlo : IVaccineBlo
{
    private readonly IVaccineDao _vaccineDao;

    public VaccineBlo(IVaccineDao vaccineDao)
    {
        _vaccineDao = vaccineDao;
    }

    public async Task<Vaccine?> GetByIdAsync(Guid id)
    {
        return await _vaccineDao.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Vaccine>> GetAllAsync()
    {
        return await _vaccineDao.GetAllAsync();
    }

    public async Task<IEnumerable<Vaccine>> GetMandatoryVaccinesAsync()
    {
        return await _vaccineDao.GetMandatoryVaccinesAsync();
    }

    public async Task<Vaccine> CreateAsync(Vaccine vaccine)
    {
        // Business validation
        if (string.IsNullOrWhiteSpace(vaccine.Name))
        {
            throw new ArgumentException("Tên vaccine không được để trống");
        }

        if (vaccine.RequiredAgeMonths < 0)
        {
            throw new ArgumentException("Tuổi yêu cầu phải lớn hơn hoặc bằng 0");
        }

        return await _vaccineDao.CreateAsync(vaccine);
    }

    public async Task<Vaccine> UpdateAsync(Vaccine vaccine)
    {
        if (vaccine == null)
        {
            throw new ArgumentNullException(nameof(vaccine));
        }

        var existing = await _vaccineDao.GetByIdAsync(vaccine.Id);
        if (existing == null)
        {
            throw new InvalidOperationException("Vaccine không tồn tại");
        }

        if (string.IsNullOrWhiteSpace(vaccine.Name))
        {
            throw new ArgumentException("Tên vaccine không được để trống");
        }

        if (vaccine.RequiredAgeMonths < 0)
        {
            throw new ArgumentException("Tuổi yêu cầu phải lớn hơn hoặc bằng 0");
        }

        return await _vaccineDao.UpdateAsync(vaccine);
    }

    public async Task DeleteAsync(Guid id)
    {
        var vaccine = await _vaccineDao.GetByIdAsync(id);
        if (vaccine == null)
        {
            throw new InvalidOperationException("Vaccine không tồn tại");
        }

        await _vaccineDao.DeleteAsync(id);
    }
}

// VaccinationRecord BLO
public interface IVaccinationRecordBlo
{
    Task<VaccinationRecord?> GetByIdAsync(Guid id);
    Task<IEnumerable<VaccinationRecord>> GetAllAsync();
    Task<IEnumerable<VaccinationRecord>> GetByStudentIdAsync(Guid studentId);
    Task<VaccinationRecord> CreateAsync(VaccinationRecord record);
    Task<VaccinationRecord> UpdateAsync(VaccinationRecord record);
    Task DeleteAsync(Guid id);
}

public class VaccinationRecordBlo : IVaccinationRecordBlo
{
    private readonly IVaccinationRecordDao _vaccinationRecordDao;
    private readonly IStudentDao _studentDao;
    private readonly IVaccineDao _vaccineDao;

    public VaccinationRecordBlo(IVaccinationRecordDao vaccinationRecordDao, IStudentDao studentDao, IVaccineDao vaccineDao)
    {
        _vaccinationRecordDao = vaccinationRecordDao;
        _studentDao = studentDao;
        _vaccineDao = vaccineDao;
    }

    public async Task<VaccinationRecord?> GetByIdAsync(Guid id)
    {
        return await _vaccinationRecordDao.GetByIdAsync(id);
    }

    public async Task<IEnumerable<VaccinationRecord>> GetAllAsync()
    {
        return await _vaccinationRecordDao.GetAllAsync();
    }

    public async Task<IEnumerable<VaccinationRecord>> GetByStudentIdAsync(Guid studentId)
    {
        return await _vaccinationRecordDao.GetByStudentIdAsync(studentId);
    }

    public async Task<VaccinationRecord> CreateAsync(VaccinationRecord record)
    {
        // Business validation
        var student = await _studentDao.GetByIdAsync(record.StudentId);
        if (student == null)
        {
            throw new ArgumentException("Học sinh không tồn tại");
        }

        var vaccine = await _vaccineDao.GetByIdAsync(record.VaccineId);
        if (vaccine == null)
        {
            throw new ArgumentException("Vaccine không tồn tại");
        }

        // Check for duplicate record (same student + same vaccine)
        var existing = await _vaccinationRecordDao.GetByStudentAndVaccineAsync(record.StudentId, record.VaccineId);
        if (existing != null)
        {
            throw new InvalidOperationException($"Học sinh đã có hồ sơ tiêm vaccine {vaccine.Name}");
        }

        return await _vaccinationRecordDao.CreateAsync(record);
    }

    public async Task<VaccinationRecord> UpdateAsync(VaccinationRecord record)
    {
        if (record == null)
        {
            throw new ArgumentNullException(nameof(record));
        }

        var existing = await _vaccinationRecordDao.GetByIdAsync(record.Id);
        if (existing == null)
        {
            throw new InvalidOperationException("Hồ sơ tiêm chủng không tồn tại");
        }

        return await _vaccinationRecordDao.UpdateAsync(record);
    }

    public async Task DeleteAsync(Guid id)
    {
        var record = await _vaccinationRecordDao.GetByIdAsync(id);
        if (record == null)
        {
            throw new InvalidOperationException("Hồ sơ tiêm chủng không tồn tại");
        }

        await _vaccinationRecordDao.DeleteAsync(id);
    }
}
