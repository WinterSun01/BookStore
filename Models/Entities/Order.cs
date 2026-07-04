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
        public string? PaymentMethod { get; set; }

        public string GetStatusDisplay()
        {
            return Status switch
            {
                "Paid" => "Оплачен",
                "Shipped" => "Отправлен",
                "Delivered" => "Доставлен",
                "Cancelled" => "Отменён",
                "Pending" => "В обработке",
                "AwaitingPayment" => "Ожидает оплаты",
                _ => Status
            };
        }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}