using Microsoft.EntityFrameworkCore;
using OberMind.PurchaseOrders.Domain.Entities;
using OberMind.PurchaseOrders.Infrastructure.DbContexts;
using System.Linq.Expressions;

namespace OberMind.PurchaseOrders.Infrastructure.Repositories;

public interface IPurchaseOrderRepository : IRepository<PurchaseOrder>
{

}

public class PurchaseOrderRepository : Repository<PurchaseOrder>, IPurchaseOrderRepository
{
    public PurchaseOrderRepository(DataContext dbContext) : base(dbContext)
    {
    }

    public override async Task<List<PurchaseOrder>> ListAsync(Expression<Func<PurchaseOrder, bool>> predicate = null)
    {
        var query = _dbSet.Include(po => po.LineItems);
        if (predicate != null)
        {
            return await query.Where(predicate).ToListAsync();
        }

        return await query.ToListAsync();
    }
}