namespace BackEnd.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
        public List<CartItem> Items { get; set; } = new ();

        //Add method
        public void AddItem(Product product, int quantity)
        {
            // Check if item already in cart or not
            if(Items.All( item => item.ProductId != product.Id))
            {
                Items.Add(new CartItem { Product = product, Quantity = quantity });
            }

            var existingItem = Items.FirstOrDefault(item => item.ProductId == product.Id);
            if(existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
        }

        public void RemoveItem(int productId, int quantity) {
            var item = Items.FirstOrDefault(item => item.ProductId == productId);
            if(item == null)
            {
                return;
            }
            item.Quantity -= quantity;

            if(item.Quantity ==0)
            {
                Items.Remove(item);
            }

        }
    }
}
