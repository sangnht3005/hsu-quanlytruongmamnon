using KindergartenManagement.DTO;
using KindergartenManagement.DAO;

namespace KindergartenManagement.BLO;

public interface IStaffLeaveBlo
{
    Task<StaffLeave?> GetByIdAsync(Guid id);
    Task<IEnumerable<StaffLeave>> GetAllAsync();
    Task<IEnumerable<StaffLeave>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<StaffLeave>> GetPendingLeavesAsync();
    Task<StaffLeave> CreateAsync(StaffLeave staffLeave);
    Task<StaffLeave> UpdateAsync(StaffLeave staffLeave);
    Task<StaffLeave> ApproveLeaveAsync(Guid leaveId, Guid approverId, string? notes);
    Task<StaffLeave> RejectLeaveAsync(Guid leaveId, Guid approverId, string? notes);
    Task DeleteAsync(Guid id);
    Task<int> GetLeaveDaysAsync(Guid userId, DateTime startDate, DateTime endDate);
    Task FixAllStatusValuesAsync();
}

public class StaffLeaveBlo : IStaffLeaveBlo
{
    private readonly IStaffLeaveDao _staffLeaveDao;
    private readonly IUserDao _userDao;

    public StaffLeaveBlo(IStaffLeaveDao staffLeaveDao, IUserDao userDao)
    {
        _staffLeaveDao = staffLeaveDao;
        _userDao = userDao;
    }

    public async Task<StaffLeave?> GetByIdAsync(Guid id)
    {
        return await _staffLeaveDao.GetByIdAsync(id);
    }

    public async Task<IEnumerable<StaffLeave>> GetAllAsync()
    {
        return await _staffLeaveDao.GetAllAsync();
    }

    public async Task<IEnumerable<StaffLeave>> GetByUserIdAsync(Guid userId)
    {
        return await _staffLeaveDao.GetByUserIdAsync(userId);
    }

    public async Task<IEnumerable<StaffLeave>> GetPendingLeavesAsync()
    {
        return await _staffLeaveDao.GetByStatusAsync("Pending");
    }

    public async Task<StaffLeave> CreateAsync(StaffLeave staffLeave)
    {
        // Business validation
        if (staffLeave.StartDate >= staffLeave.EndDate)
        {
            throw new ArgumentException("Ngày bắt đầu phải trước ngày kết thúc");
        }

        if (staffLeave.StartDate < DateTime.Now.Date)
        {
            throw new ArgumentException("Không thể tạo đơn nghỉ cho ngày trong quá khứ");
        }

        // Verify user exists
        var user = await _userDao.GetByIdAsync(staffLeave.UserId);
        if (user == null)
        {
            throw new ArgumentException("Nhân viên không tồn tại");
        }

        // Check for overlapping leaves
        var existingLeaves = await _staffLeaveDao.GetByUserIdAsync(staffLeave.UserId);
        var hasOverlap = existingLeaves.Any(el => 
            el.Status != "Rejected" &&
            ((staffLeave.StartDate >= el.StartDate && staffLeave.StartDate <= el.EndDate) ||
             (staffLeave.EndDate >= el.StartDate && staffLeave.EndDate <= el.EndDate) ||
             (staffLeave.StartDate <= el.StartDate && staffLeave.EndDate >= el.EndDate)));

        if (hasOverlap)
        {
            throw new InvalidOperationException("Đã có đơn nghỉ trong khoảng thời gian này");
        }

        staffLeave.Status = "Pending";
        staffLeave.LeaveType = NormalizeLeaveType(staffLeave.LeaveType);
        return await _staffLeaveDao.CreateAsync(staffLeave);
    }

    public async Task<StaffLeave> UpdateAsync(StaffLeave staffLeave)
    {
        if (staffLeave == null)
        {
            throw new ArgumentNullException(nameof(staffLeave));
        }

        var existing = await _staffLeaveDao.GetByIdAsync(staffLeave.Id);
        if (existing == null)
        {
            throw new InvalidOperationException("Đơn nghỉ không tồn tại");
        }

        // Normalize status values (trim and compare case-insensitive)
        var existingStatus = existing.Status?.Trim() ?? "Pending";
        var newStatus = staffLeave.Status?.Trim() ?? "Pending";
        
        System.Diagnostics.Debug.WriteLine($"UpdateAsync - Existing Status: '{existingStatus}', New Status: '{newStatus}'");

        // Allow status change from Pending to Approved/Rejected (admin approval)
        // But prevent changing other fields if status is not Pending
        if (!existingStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase) && 
            existingStatus.Equals(newStatus, StringComparison.OrdinalIgnoreCase))
        {
            System.Diagnostics.Debug.WriteLine("Throwing exception: Cannot edit non-pending leave without status change");
            throw new InvalidOperationException("Chỉ có thể sửa đơn nghỉ đang chờ duyệt");
        }

        // Ensure status is properly formatted before saving
        staffLeave.Status = NormalizeStatus(newStatus);
        staffLeave.LeaveType = NormalizeLeaveType(staffLeave.LeaveType);
        
        System.Diagnostics.Debug.WriteLine($"Proceeding with update, normalized status: {staffLeave.Status}");
        return await _staffLeaveDao.UpdateAsync(staffLeave);
    }

    private string NormalizeStatus(string status)
    {
        var normalized = status?.Trim() ?? "Pending";
        
        // Handle Vietnamese status values
        return normalized.ToLower() switch
        {
            "pending" or "chờ duyệt" or "cho duyet" => "Pending",
            "approved" or "đã duyệt" or "da duyet" => "Approved",
            "rejected" or "từ chối" or "tu choi" => "Rejected",
            _ => normalized
        };
    }

    private string NormalizeLeaveType(string leaveType)
    {
        var normalized = leaveType?.Trim() ?? "Vacation";
        
        // Handle Vietnamese leave type values
        return normalized.ToLower() switch
        {
            "vacation" or "nghỉ phép" or "nghi phep" => "Vacation",
            "sick" or "nghỉ ốm" or "nghi om" => "Sick",
            "personal" or "nghỉ cá nhân" or "nghi ca nhan" => "Personal",
            "unpaid" or "nghỉ không lương" or "nghi khong luong" => "Unpaid",
            _ => normalized
        };
    }

    public async Task<StaffLeave> ApproveLeaveAsync(Guid leaveId, Guid approverId, string? notes)
    {
        var leave = await _staffLeaveDao.GetByIdAsync(leaveId);
        if (leave == null)
        {
            throw new InvalidOperationException("Đơn nghỉ không tồn tại");
        }

        if (leave.Status != "Pending")
        {
            throw new InvalidOperationException("Đơn nghỉ đã được xử lý");
        }

        // Verify approver exists
        var approver = await _userDao.GetByIdAsync(approverId);
        if (approver == null)
        {
            throw new ArgumentException("Người duyệt không tồn tại");
        }

        leave.Status = "Approved";
        leave.ApprovedBy = approverId;
        leave.ApprovalDate = DateTime.Now;
        leave.ApprovalNotes = notes;

        return await _staffLeaveDao.UpdateAsync(leave);
    }

    public async Task<StaffLeave> RejectLeaveAsync(Guid leaveId, Guid approverId, string? notes)
    {
        var leave = await _staffLeaveDao.GetByIdAsync(leaveId);
        if (leave == null)
        {
            throw new InvalidOperationException("Đơn nghỉ không tồn tại");
        }

        if (leave.Status != "Pending")
        {
            throw new InvalidOperationException("Đơn nghỉ đã được xử lý");
        }

        // Verify approver exists
        var approver = await _userDao.GetByIdAsync(approverId);
        if (approver == null)
        {
            throw new ArgumentException("Người duyệt không tồn tại");
        }

        leave.Status = "Rejected";
        leave.ApprovedBy = approverId;
        leave.ApprovalDate = DateTime.Now;
        leave.ApprovalNotes = notes;

        return await _staffLeaveDao.UpdateAsync(leave);
    }

    public async Task DeleteAsync(Guid id)
    {
        var leave = await _staffLeaveDao.GetByIdAsync(id);
        if (leave == null)
        {
            throw new InvalidOperationException("Đơn nghỉ không tồn tại");
        }

        if (leave.Status != "Pending")
        {
            throw new InvalidOperationException("Chỉ có thể xóa đơn nghỉ đang chờ duyệt");
        }

        await _staffLeaveDao.DeleteAsync(id);
    }

    public async Task<int> GetLeaveDaysAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        var leaves = await _staffLeaveDao.GetByUserIdAsync(userId);
        var approvedLeaves = leaves.Where(l => 
            l.Status == "Approved" &&
            l.StartDate >= startDate &&
            l.EndDate <= endDate);

        int totalDays = 0;
        foreach (var leave in approvedLeaves)
        {
            totalDays += (leave.EndDate - leave.StartDate).Days + 1;
        }

        return totalDays;
    }

    public async Task FixAllStatusValuesAsync()
    {
        var allLeaves = await _staffLeaveDao.GetAllAsync();
        int fixedCount = 0;

        foreach (var leave in allLeaves)
        {
            var originalStatus = leave.Status;
            var originalLeaveType = leave.LeaveType;
            var normalizedStatus = NormalizeStatus(leave.Status);
            var normalizedLeaveType = NormalizeLeaveType(leave.LeaveType);
            
            if (originalStatus != normalizedStatus || originalLeaveType != normalizedLeaveType)
            {
                leave.Status = normalizedStatus;
                leave.LeaveType = normalizedLeaveType;
                await _staffLeaveDao.UpdateAsync(leave);
                fixedCount++;
                System.Diagnostics.Debug.WriteLine($"Fixed Leave {leave.Id}: Status '{originalStatus}' -> '{normalizedStatus}', Type '{originalLeaveType}' -> '{normalizedLeaveType}'");
            }
        }

        System.Diagnostics.Debug.WriteLine($"Fixed {fixedCount} leave records");
    }
}
