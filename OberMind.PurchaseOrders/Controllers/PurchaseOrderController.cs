using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OberMind.PurchaseOrders.Application.DTOs;

namespace OberMind.PurchaseOrders.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrderService _purchaseOrderService;

        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService)
        {
            _purchaseOrderService = purchaseOrderService;
        }

        [HttpPost]
        public async Task<ActionResult<PurchaseOrderDTO>> CreatePurchaseOrder()
        {
            var purchaseOrder = await _purchaseOrderService.CreatePurchaseOrderAsync(GetUserId());

            return Ok(purchaseOrder);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PurchaseOrderDTO>> EditPurchaseOrder(int id, [FromBody] PurchaseOrderDTO editDto)
        {
            try
            {
                var purchaseOrder = await _purchaseOrderService.EditPurchaseOrderAsync(id, editDto, GetUserId());
                return Ok(purchaseOrder);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{id}/submit")]
        public async Task<ActionResult<PurchaseOrderDTO>> SubmitPurchaseOrder(int id)
        {
            try
            {
                var purchaseOrder = await _purchaseOrderService.SubmitPurchaseOrderAsync(id, GetUserId());
                return Ok(purchaseOrder);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseOrderDTO>>> GetPurchaseOrders()
        {
            var purchaseOrders = await _purchaseOrderService.GetPurchaseOrdersAsync(GetUserId());

            return Ok(purchaseOrders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseOrderDTO>> GetPurchaseOrder(int id)
        {
            var purchaseOrder = await _purchaseOrderService.GetPurchaseOrderByIdAsync(id, GetUserId());

            if (purchaseOrder == null)
            {
                return NotFound();
            }

            return Ok(purchaseOrder);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePurchaseOrder(int id)
        {
            try
            {
                await _purchaseOrderService.DeletePurchaseOrder(id, GetUserId());
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        private Guid GetUserId()
        {
            return Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
        }
    }

}
