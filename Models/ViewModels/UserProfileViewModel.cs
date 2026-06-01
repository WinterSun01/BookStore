using BookStore.Models;
using BookStore.Models.Entities;

namespace BookStore.Models
{
    public class UserProfileViewModel
    {
        public ApplicationUser? User { get; set; }
        public IEnumerable<Order> Orders { get; set; } = new List<Order>();
    }
}