namespace OberMind.PurchaseOrders.Domain.Entities;

public static class PurchaseOrderStatus
{
    public static string DRAFT = "DRAFT";
    public static string SUBMITTED = "SUBMITTED";
}

public class PurchaseOrder
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public string Status { get; set; }
    public List<LineItem> LineItems { get; set; }
}