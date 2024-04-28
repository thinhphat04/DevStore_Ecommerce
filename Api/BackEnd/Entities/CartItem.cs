using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Entities
{
    [Table("CartItems")]
    public class CartItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        //navigation properties
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int CartId { get; set; }
        public Cart cart { get; set; }

    }
}