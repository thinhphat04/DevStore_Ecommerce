using BackEnd.DTOs;
using BackEnd.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Extensions
{
    public static class CartExtensions
    {
        public static CartDto MapCartDto(this Cart cart)
        {
            return new CartDto
            {
                Id = cart.Id,
                BuyerId = cart.BuyerId,
                PaymentIntentId = cart.PaymentIntentId,
                ClientSecret = cart.ClientSecret,
                Items = cart.Items.Select(item => new CartItemDto
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Name = item.Product.Name,
                    Brand = item.Product.Brand,
                    Type = item.Product.Type,
                    PictureUrl = item.Product.PictureUrl,
                    Price = item.Product.Price
                }).ToList(),
            };
        }

        public static IQueryable<Cart> RetrieveCartWithItems(this IQueryable<Cart> query, string buyerId)
        {
            return query.Include(i => i.Items)
                .ThenInclude(p => p.Product).Where(b => b.BuyerId == buyerId);
        }
    }
}
