using _2022_CS_668.Data;
using _2022_CS_668.Repositories.Interfaces;

namespace _2022_CS_668.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        
        public ApplicationDbContext Context => _context;

        public IMessGroupRepository MessGroups { get; }
        public IUserMessGroupRepository UserMessGroups { get; }
        public IMenuRepository Menus { get; }
        public IAttendanceRepository Attendances { get; }
        public IBillRepository Bills { get; }
        public IBillDetailRepository BillDetails { get; }
        public IPaymentRepository Payments { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            MessGroups = new MessGroupRepository(_context);
            UserMessGroups = new UserMessGroupRepository(_context);
            Menus = new MenuRepository(_context);
            Attendances = new AttendanceRepository(_context);
            Bills = new BillRepository(_context);
            BillDetails = new BillDetailRepository(_context);
            Payments = new PaymentRepository(_context);
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
