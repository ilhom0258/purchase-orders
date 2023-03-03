using OberMind.PurchaseOrders.Domain.Entities;

namespace OberMind.PurchaseOrders.Application.DTOs;

public class PurchaseOrderDTO
{
    public PurchaseOrderDTO()
    {
        
    }
    public PurchaseOrderDTO(PurchaseOrder? purchaseOrder)
    {
        Id = purchaseOrder.Id;   
        Name = purchaseOrder.Name;
        CreatedAt = purchaseOrder.CreatedAt;
        CreatedById = purchaseOrder.CreatedById;
        SubmittedAt = purchaseOrder.SubmittedAt;
        Status = purchaseOrder.Status;
        LineItems = purchaseOrder.LineItems.Select(li => new LineItemDTO(li));
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedById { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public string Status { get; set; }
    public IEnumerable<LineItemDTO> LineItems { get; set; }
}