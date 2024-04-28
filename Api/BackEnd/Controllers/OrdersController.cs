﻿using BackEnd.Data;
using BackEnd.DTOs;
using BackEnd.Entities;
using BackEnd.Entities.OrderAggregare;
using BackEnd.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly StoreContext _context;

        public OrdersController(StoreContext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderDto>>> GetOrders()
        {
            return await _context.Orders
                .ProjectOrderToOrderDto()
                .Where(x => x.BuyerId == User.Identity.Name)
                .ToListAsync();
        }

        [HttpGet("{id}", Name = "GetOrder")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            return await _context.Orders
                .ProjectOrderToOrderDto()
                .Where(o => o.BuyerId == User.Identity.Name && o.Id == id)
                .FirstOrDefaultAsync();
        }

        [HttpPost]
        public async Task<ActionResult<int>> CreateOrder(CreateOrderDto orderDto)
        {
            try
            {
                var cart = await _context.Carts
               .RetrieveCartWithItems(User.Identity.Name)
               .FirstOrDefaultAsync();

                if (cart == null) return BadRequest(new ProblemDetails { Title = "Could not locate basket" });

                var items = new List<OrderItem>();

                foreach (var item in cart.Items)
                {
                    var productItem = await _context.Products.FindAsync(item.ProductId);
                    var itemOrdered = new ProductItemOrdered
                    {
                        ProductId = productItem.Id,
                        Name = productItem.Name,
                        PictureUrl = productItem.PictureUrl
                    };

                    var orderItem = new OrderItem
                    {
                        ItemOrdered = itemOrdered,
                        Price = productItem.Price,
                        Quantity = item.Quantity
                    };

                    items.Add(orderItem);

                    productItem.QuantityInStock -= item.Quantity;
                }

                var subtotal = items.Sum(item => item.Price * item.Quantity);
                var deliveryFee = subtotal > 10000 ? 0 : 500;

                var order = new Order
                {
                    OrderItems = items,
                    BuyerId = User.Identity.Name,
                    ShippingAddress = orderDto.ShippingAddress,
                    SubTotal = subtotal,
                    DeliveryFee = deliveryFee,
                    PaymentIntentId = cart.PaymentIntentId
                };

                _context.Orders.Add(order);
                _context.Carts.Remove(cart);

                if (orderDto.SaveAddress)
                {
                    var user = await _context.Users
                        .Include(a => a.Address)
                        .FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);

                    var address = new UserAddress
                    {
                        FullName = orderDto.ShippingAddress.FullName,
                        Address1 = orderDto.ShippingAddress.Address1,
                        Address2 = orderDto.ShippingAddress.Address2,
                        City = orderDto.ShippingAddress.City,
                        State = orderDto.ShippingAddress.State,
                        Zip = orderDto.ShippingAddress.Zip,
                        Country = orderDto.ShippingAddress.Country,
                    };

                    user.Address = address;
                }

                var result = await _context.SaveChangesAsync() > 0;

                if (result) return CreatedAtRoute("GetOrder", new { id = order.Id }, order.Id);
                return BadRequest("Problem creating order");

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occurred while creating the order.");
                return 178;

            }
                 
        }
    }
}