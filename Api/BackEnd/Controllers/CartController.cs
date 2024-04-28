using BackEnd.Data;
using BackEnd.DTOs;
using BackEnd.Entities;
using BackEnd.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : BaseApiController
    {
        private readonly StoreContext _context;
        public CartController(StoreContext context)
        {
            _context = context;
        }

        [HttpGet(Name = "GetCart")]
        public async Task<ActionResult<CartDto>> GetCart()
        {
            var cart = await RetrieveCart(GetBuyerId());

            if (cart == null)
            {
                return NotFound();
            }
            return cart.MapCartDto();
        }


        [HttpPost]      
        public async Task<ActionResult<CartDto>> AddIemToCart(int productId, int quantity)
        {
            // Get Cart || Create Cart
            var cart = await RetrieveCart(GetBuyerId());
            if(cart == null)
            {
                cart = CreateCart();
            }

            // Get Product
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return BadRequest(new ProblemDetails { Title = "Product Not Found"});
            
            // Add Item
            cart.AddItem(product, quantity);    //method of class Cart

            // Save changes
            var result = await _context.SaveChangesAsync();
            if(result > 0) return CreatedAtRoute("GetCart", cart.MapCartDto());

            //if fail
            return BadRequest(new ProblemDetails { Title = "Prolem saving item to cart" });

        }
   

        [HttpDelete]
        public async Task<ActionResult> RemoveCartItem(int productId, int quantity)
        {
            // Get Cart
            var cart = await RetrieveCart(GetBuyerId());
            if(cart == null) { return NotFound(); }
            
            // Remove item or reduce quantity
            cart.RemoveItem(productId, quantity);

            // Save changes
            var result = await _context.SaveChangesAsync();
            if (result > 0) return StatusCode(201);

            //if fail
            return BadRequest(new ProblemDetails { Title = "Prolem removing item from cart" });
        }

        private async Task<Cart> RetrieveCart(string buyerId)
        {
            if (string.IsNullOrEmpty(buyerId))
            {
                Response.Cookies.Delete("buyerId");
                return null;
            }

            return await _context.Carts
                .Include(i => i.Items)
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(x => x.BuyerId == buyerId);
        }

        private string GetBuyerId()
        {
            return User.Identity?.Name ?? Request.Cookies["buyerId"];
        }

        private Cart? CreateCart()
        {
            var buyerId = User.Identity?.Name;
            if (string.IsNullOrEmpty(buyerId))
            {
                // Generate globally unique identifier
                buyerId = Guid.NewGuid().ToString();

                // Set cookieoption
                var cookieOptions = new CookieOptions { IsEssential= true, Expires = DateTime.Now.AddDays(30), SameSite=SameSiteMode.None, Secure=true };

                // Add cookie to response
                Response.Cookies.Append("buyerId", buyerId, cookieOptions);

            }

            var basket = new Cart { BuyerId = buyerId };
            _context.Carts.Add(basket);
            return basket;
        }

    }
}
