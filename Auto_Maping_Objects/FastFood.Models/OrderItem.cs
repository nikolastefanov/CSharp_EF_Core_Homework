namespace FastFood.Models
{
    using System.ComponentModel.DataAnnotations;

    public class OrderItem
	{
		public int OrderId { get; set; }

		//[Required]
		public Order Order { get; set; }

		public int ItemId { get; set; }

		//[Required]
		public Item Item { get; set; }

		//[Range(1, 1000)]
		public int Quantity { get; set; }
	}
}