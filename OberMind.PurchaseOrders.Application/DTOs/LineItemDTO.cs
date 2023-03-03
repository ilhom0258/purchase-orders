using OberMind.PurchaseOrders.Domain.Entities;

namespace OberMind.PurchaseOrders.Application.DTOs;

public class LineItemDTO
{
    public LineItemDTO()
    {
        
    }
    public LineItemDTO(LineItem lineItem)
    {
        Id = lineItem.Id;
        Name = lineItem.Name;
        Amount = lineItem.Amount;
        PurchaseOrderId = lineItem.PurchaseOrderId;   
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public int PurchaseOrderId { get; set; }
}