namespace BackEnd.DTOs
{
    public class CartDto
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }
        public string PaymentIntentId { get; set; }
        public string ClientSecret { get; set; }
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    }
}
