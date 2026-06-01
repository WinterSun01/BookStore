namespace BookStore.Models.Entities
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        public int Quantity { get; set; } = 1;
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}