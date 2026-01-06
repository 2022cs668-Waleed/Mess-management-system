using _2022_CS_668.Models;

namespace _2022_CS_668.Repositories.Interfaces
{
    public interface IMessGroupRepository : IRepository<MessGroup>
    {
        Task<MessGroup?> GetByNameAsync(string name);
        Task<IEnumerable<MessGroup>> GetActiveGroupsAsync();
    }

    public interface IUserMessGroupRepository : IRepository<UserMessGroup>
    {
        Task<IEnumerable<UserMessGroup>> GetUserGroupsAsync(string userId);
        Task<UserMessGroup?> GetUserGroupAsync(string userId, int messGroupId);
    }

    public interface IMenuRepository : IRepository<Menu>
    {
        Task<IEnumerable<Menu>> GetMenuByDateAsync(DateTime date);
        Task<IEnumerable<Menu>> GetActiveMenusAsync();
        Task<Menu?> GetMenuByDateAndGroupAsync(DateTime date, int messGroupId);
    }

    public interface IAttendanceRepository : IRepository<Attendance>
    {
        Task<Attendance?> GetAttendanceAsync(string userId, DateTime date);
        Task<IEnumerable<Attendance>> GetAttendanceByDateAsync(DateTime date);
        Task<IEnumerable<Attendance>> GetUserAttendanceAsync(string userId, int month, int year);
        Task<int> GetPresentDaysAsync(string userId, int month, int year);
    }

    public interface IBillRepository : IRepository<Bill>
    {
        Task<Bill?> GetBillAsync(string userId, int month, int year);
        Task<IEnumerable<Bill>> GetBillsByStatusAsync(BillStatus status);
        Task<IEnumerable<Bill>> GetUserBillsAsync(string userId);
        Task<Bill?> GetBillWithDetailsAsync(int billId);
    }

    public interface IBillDetailRepository : IRepository<BillDetail>
    {
        Task<IEnumerable<BillDetail>> GetBillDetailsAsync(int billId);
    }

    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<IEnumerable<Payment>> GetBillPaymentsAsync(int billId);
        Task<decimal> GetTotalPaidAmountAsync(int billId);
    }
}
