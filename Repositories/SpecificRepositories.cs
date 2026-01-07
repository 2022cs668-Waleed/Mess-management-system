using Microsoft.EntityFrameworkCore;
using _2022_CS_668.Data;
using _2022_CS_668.Models;
using _2022_CS_668.Repositories.Interfaces;

namespace _2022_CS_668.Repositories
{
    public class MessGroupRepository : Repository<MessGroup>, IMessGroupRepository
    {
        public MessGroupRepository(ApplicationDbContext context) : base(context) { }

        public async Task<MessGroup?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.Name == name);
        }

        public async Task<IEnumerable<MessGroup>> GetActiveGroupsAsync()
        {
            return await _dbSet.Where(m => m.IsActive).ToListAsync();
        }
    }

    public class UserMessGroupRepository : Repository<UserMessGroup>, IUserMessGroupRepository
    {
        public UserMessGroupRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<UserMessGroup>> GetUserGroupsAsync(string userId)
        {
            return await _dbSet
                .Include(u => u.MessGroup)
                .Where(u => u.UserId == userId && u.IsActive)
                .ToListAsync();
        }

        public async Task<UserMessGroup?> GetUserGroupAsync(string userId, int messGroupId)
        {
            return await _dbSet
                .Include(u => u.MessGroup)
                .FirstOrDefaultAsync(u => u.UserId == userId && u.MessGroupId == messGroupId && u.IsActive);
        }
    }

    public class MenuRepository : Repository<Menu>, IMenuRepository
    {
        public MenuRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<Menu?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(m => m.MessGroup)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Menu>> GetMenuByDateAsync(DateTime date)
        {
            // Normalize date to start of day in UTC for PostgreSQL
            var startOfDay = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
            var endOfDay = startOfDay.AddDays(1);
            
            return await _dbSet
                .Include(m => m.MessGroup)
                .Where(m => m.EffectiveDate >= startOfDay && m.EffectiveDate < endOfDay && m.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Menu>> GetActiveMenusAsync()
        {
            return await _dbSet
                .Include(m => m.MessGroup)
                .Where(m => m.IsActive)
                .OrderByDescending(m => m.EffectiveDate)
                .ToListAsync();
        }

        public async Task<Menu?> GetMenuByDateAndGroupAsync(DateTime date, int messGroupId)
        {
            // Normalize date to start of day in UTC for PostgreSQL
            var startOfDay = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
            var endOfDay = startOfDay.AddDays(1);
            
            return await _dbSet
                .Include(m => m.MessGroup)
                .FirstOrDefaultAsync(m => m.EffectiveDate >= startOfDay 
                    && m.EffectiveDate < endOfDay 
                    && m.MessGroupId == messGroupId 
                    && m.IsActive);
        }
    }

    public class AttendanceRepository : Repository<Attendance>, IAttendanceRepository
    {
        public AttendanceRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Attendance?> GetAttendanceAsync(string userId, DateTime date)
        {
            // Normalize date to start of day in UTC for PostgreSQL
            var startOfDay = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
            var endOfDay = startOfDay.AddDays(1);
            
            return await _dbSet
                .Include(a => a.User)
                .Include(a => a.AttendanceMenuItems)
                    .ThenInclude(ami => ami.Menu)
                        .ThenInclude(m => m.MessGroup)
                .FirstOrDefaultAsync(a => a.UserId == userId && a.Date >= startOfDay && a.Date < endOfDay);
        }

        public async Task<IEnumerable<Attendance>> GetAttendanceByDateAsync(DateTime date)
        {
            // Normalize date to start of day in UTC for PostgreSQL
            var startOfDay = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
            var endOfDay = startOfDay.AddDays(1);
            
            return await _dbSet
                .Include(a => a.User)
                .Include(a => a.AttendanceMenuItems)
                    .ThenInclude(ami => ami.Menu)
                .Where(a => a.Date >= startOfDay && a.Date < endOfDay)
                .ToListAsync();
        }

        public async Task<IEnumerable<Attendance>> GetUserAttendanceAsync(string userId, int month, int year)
        {
            // Create date range for the month in UTC
            var startOfMonth = DateTime.SpecifyKind(new DateTime(year, month, 1), DateTimeKind.Utc);
            var endOfMonth = startOfMonth.AddMonths(1);
            
            return await _dbSet
                .Include(a => a.AttendanceMenuItems)
                    .ThenInclude(ami => ami.Menu)
                        .ThenInclude(m => m.MessGroup)
                .Where(a => a.UserId == userId 
                    && a.Date >= startOfMonth 
                    && a.Date < endOfMonth)
                .OrderBy(a => a.Date)
                .ToListAsync();
        }

        public async Task<int> GetPresentDaysAsync(string userId, int month, int year)
        {
            // Create date range for the month in UTC
            var startOfMonth = DateTime.SpecifyKind(new DateTime(year, month, 1), DateTimeKind.Utc);
            var endOfMonth = startOfMonth.AddMonths(1);
            
            return await _dbSet
                .CountAsync(a => a.UserId == userId 
                    && a.Date >= startOfMonth 
                    && a.Date < endOfMonth 
                    && a.IsPresent);
        }
    }

    public class BillRepository : Repository<Bill>, IBillRepository
    {
        public BillRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<IEnumerable<Bill>> GetAllAsync()
        {
            return await _dbSet
                .Include(b => b.User)
                .ToListAsync();
        }

        public async Task<Bill?> GetBillAsync(string userId, int month, int year)
        {
            return await _dbSet
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.UserId == userId && b.Month == month && b.Year == year);
        }

        public async Task<IEnumerable<Bill>> GetBillsByStatusAsync(BillStatus status)
        {
            return await _dbSet
                .Include(b => b.User)
                .Where(b => b.Status == status)
                .OrderByDescending(b => b.Year)
                .ThenByDescending(b => b.Month)
                .ToListAsync();
        }

        public async Task<IEnumerable<Bill>> GetUserBillsAsync(string userId)
        {
            return await _dbSet
                .Include(b => b.User)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.Year)
                .ThenByDescending(b => b.Month)
                .ToListAsync();
        }

        public async Task<Bill?> GetBillWithDetailsAsync(int billId)
        {
            return await _dbSet
                .Include(b => b.User)
                .Include(b => b.BillDetails)
                    .ThenInclude(bd => bd.Menu)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.Id == billId);
        }
    }

    public class BillDetailRepository : Repository<BillDetail>, IBillDetailRepository
    {
        public BillDetailRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<BillDetail>> GetBillDetailsAsync(int billId)
        {
            return await _dbSet
                .Include(bd => bd.Menu)
                .Where(bd => bd.BillId == billId)
                .OrderBy(bd => bd.Date)
                .ToListAsync();
        }
    }

    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Payment>> GetBillPaymentsAsync(int billId)
        {
            return await _dbSet
                .Where(p => p.BillId == billId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalPaidAmountAsync(int billId)
        {
            return await _dbSet
                .Where(p => p.BillId == billId)
                .SumAsync(p => p.Amount);
        }
    }
}
