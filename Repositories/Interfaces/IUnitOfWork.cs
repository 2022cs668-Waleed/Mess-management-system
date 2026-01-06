namespace _2022_CS_668.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IMessGroupRepository MessGroups { get; }
        IUserMessGroupRepository UserMessGroups { get; }
        IMenuRepository Menus { get; }
        IAttendanceRepository Attendances { get; }
        IBillRepository Bills { get; }
        IBillDetailRepository BillDetails { get; }
        IPaymentRepository Payments { get; }
        
        Task<int> SaveAsync();
    }
}
