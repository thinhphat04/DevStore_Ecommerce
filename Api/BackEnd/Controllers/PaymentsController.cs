using BackEnd.Data;
using BackEnd.DTOs;
using BackEnd.Entities.OrderAggregare;
using BackEnd.Extensions;
using BackEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly StoreContext _context;
        private readonly IConfiguration _config;
        public PaymentsController(PaymentService paymentService, StoreContext context, IConfiguration config)
        {
            _config = config;
            _context = context;
            _paymentService = paymentService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CartDto>> CreateOrUpdatePaymentIntent()
        {
            var cart = await _context.Carts
                .RetrieveCartWithItems(User.Identity.Name)
                .FirstOrDefaultAsync();

            if (cart == null) return NotFound();

            var intent = await _paymentService.CreateOrUpdatePaymentIntent(cart);

            if (intent == null) return BadRequest(new ProblemDetails { Title = "Problem creating payment intent" });

            cart.PaymentIntentId = string.IsNullOrWhiteSpace(cart.PaymentIntentId) ? intent.Id : cart.PaymentIntentId;
            cart.ClientSecret = string.IsNullOrWhiteSpace(cart.ClientSecret) ? intent.ClientSecret : cart.ClientSecret;

            _context.Update(cart);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return BadRequest(new ProblemDetails { Title = "Problem updating basket with intent" });

            return cart.MapCartDto();
        }

        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"],
                _config["StripeSettings:WhSecret"]);

            var charge = (Charge)stripeEvent.Data.Object;

            var order = await _context.Orders.FirstOrDefaultAsync(x => x.PaymentIntentId == charge.PaymentIntentId);

            if (charge.Status == "succeeded") {
                order.OrderStatus = OrderStatus.PaymentReceived;
            }    

            await _context.SaveChangesAsync();

            return new EmptyResult();
        }
    }
}
