using Microsoft.EntityFrameworkCore;
using OberMind.PurchaseOrders.Application.DTOs;
using OberMind.PurchaseOrders.Domain.Entities;
using OberMind.PurchaseOrders.Infrastructure.DbContexts;
using OberMind.PurchaseOrders.Infrastructure.Repositories;

public interface IPurchaseOrderService
{
    Task<PurchaseOrderDTO> CreatePurchaseOrderAsync(Guid userId);
    Task<PurchaseOrderDTO> EditPurchaseOrderAsync(int purchaseOrderId, PurchaseOrderDTO purchaseOrderDto, Guid userId);
    Task<PurchaseOrderDTO> SubmitPurchaseOrderAsync(int purchaseOrderId, Guid userId);
    Task<IEnumerable<PurchaseOrderDTO>> GetPurchaseOrdersAsync(Guid userId);
    Task<PurchaseOrderDTO> GetPurchaseOrderByIdAsync(int id, Guid userId);
    Task DeletePurchaseOrder(int id, Guid userId);
}

public class PurchaseOrderService: IPurchaseOrderService
{
    private readonly DataContext _context;
    private readonly IPurchaseOrderRepository repository;
    private readonly int _maxSubmittedPerDay = 10;

    public PurchaseOrderService(DataContext context, IPurchaseOrderRepository repository)
    {
        _context = context;
        this.repository = repository;
    }

    public async Task<PurchaseOrderDTO> CreatePurchaseOrderAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        var purchaseOrder = new PurchaseOrder
        {
            Name = GeneratePurchaseOrderName(),
            CreatedAt = DateTime.UtcNow,
            Status = PurchaseOrderStatus.DRAFT,
            CreatedById = userId,
            CreatedBy = user,
            LineItems = new List<LineItem>()
        };

        await _context.PurchaseOrders.AddAsync(purchaseOrder);
        await _context.SaveChangesAsync();

        return new PurchaseOrderDTO(purchaseOrder);
    }

    public async Task<PurchaseOrderDTO> EditPurchaseOrderAsync(int purchaseOrderId, PurchaseOrderDTO purchaseOrderDto, Guid userId)
    {
        var purchaseOrder = (await repository.ListAsync(p => p.Id == purchaseOrderId && p.CreatedById == userId)).FirstOrDefault();

        if (purchaseOrder == null)
        {
            throw new ArgumentException("Purchase order does not exist.");
        }

        if (purchaseOrder.Status != PurchaseOrderStatus.DRAFT)
        {
            throw new ArgumentException("Cannot edit a submitted purchase order.");
        }

        var lineItems = purchaseOrderDto.LineItems.Select(li => new LineItem
        {
            Name = li.Name,
            Amount = li.Amount,
            PurchaseOrderId = purchaseOrder.Id,
            PurchaseOrder = purchaseOrder
        }).ToList();

        if (lineItems.Count == 0)
        {
            throw new ArgumentException("Purchase order must have at least one line item.");
        }

        if (lineItems.Count > 10)
        {
            throw new ArgumentException("Purchase order cannot have more than 10 line items.");
        }

        var totalAmount = lineItems.Sum(li => li.Amount);

        if (totalAmount > 10000)
        {
            throw new ArgumentException("Total amount of purchase order cannot exceed 10000.");
        }

        purchaseOrder.LineItems.Clear();
        purchaseOrder.LineItems = lineItems;

        await _context.SaveChangesAsync();
        return new PurchaseOrderDTO(purchaseOrder);
    }

    public async Task<PurchaseOrderDTO> SubmitPurchaseOrderAsync(int purchaseOrderId, Guid userId)
    {
        var purchaseOrder = await _context.PurchaseOrders.Include(po => po.LineItems)
            .FirstOrDefaultAsync(p => p.Id == purchaseOrderId && p.CreatedById == userId) 
            ?? throw new ArgumentException("Invalid purchase order ID");
        
        if (purchaseOrder.Status != PurchaseOrderStatus.DRAFT)
        {
            throw new InvalidOperationException("Cannot submit a purchase order that is not in draft status");
        }

        if (purchaseOrder.LineItems.Count < 1)
        {
            throw new InvalidOperationException("Cannot submit a purchase order with no line items");
        }

        if (purchaseOrder.LineItems.Sum(li => li.Amount) > 10000)
        {
            throw new InvalidOperationException("Cannot submit a purchase order with total amount exceeding 10000");
        }

        if ((await GetSubmittedPurchaseOrderCountForUserAsync(userId)) > _maxSubmittedPerDay)
        {
            throw new InvalidOperationException("Cannot submit more than 10 purchase orders per day");
        }

        purchaseOrder.Status = PurchaseOrderStatus.SUBMITTED;
        purchaseOrder.SubmittedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return new PurchaseOrderDTO(purchaseOrder);
    }

    public async Task<IEnumerable<PurchaseOrderDTO>> GetPurchaseOrdersAsync(Guid userId)
    {
        return (await repository.ListAsync(p => p.CreatedById == userId)).Select(p => new PurchaseOrderDTO(p));
    }

    public async Task<PurchaseOrderDTO> GetPurchaseOrderByIdAsync(int id, Guid userId)
    {
        var purchaseOrder = (await repository.ListAsync(p => p.Id == id && p.CreatedById == userId)).FirstOrDefault();
        if (purchaseOrder == null)
        {
            throw new ArgumentException("Purchase order does not exist.");
        }
        return new PurchaseOrderDTO(purchaseOrder);
    }

    public async Task DeletePurchaseOrder(int id, Guid userId)
    {
        var purchaseOrder = (await repository.ListAsync(p => p.Id == id && p.CreatedById == userId)).FirstOrDefault();
        if (purchaseOrder == null)
        {
            throw new ArgumentException("Purchase order does not exist.");
        }
        await repository.DeleteAsync(purchaseOrder);
    }

    private string GeneratePurchaseOrderName()
    {
        // Generate a unique name for the purchase order based on the current timestamp
        return $"PO_{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid()}";
    }

    private async Task<int> GetSubmittedPurchaseOrderCountForUserAsync(Guid userId)
    {
        var today = DateTime.UtcNow.Date;
        return await _context.PurchaseOrders
            .CountAsync(po => po.CreatedById == userId && po.Status == PurchaseOrderStatus.SUBMITTED && DateTime.UtcNow.Date == po.SubmittedAt.Value.Date);
    }
}