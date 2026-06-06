using System.ComponentModel.DataAnnotations;

namespace BookStore.Models.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending";

        public string? ShippingAddress { get; set; }
        public string? PaymentMethod { get; set; } //имитация платежа

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}