using KindergartenManagement.DTO;
using System.Threading.Tasks;

namespace KindergartenManagement.Services
{
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(Account account, string permissionName);
        Task LoadPermissionsAsync(Account account);
        bool HasPermission(string permissionName);
    }
}
