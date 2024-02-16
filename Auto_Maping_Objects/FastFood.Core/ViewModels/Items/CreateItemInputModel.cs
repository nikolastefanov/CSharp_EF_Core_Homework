using System.ComponentModel.DataAnnotations;

namespace FastFood.Core.ViewModels.Items
{
    public class CreateItemInputModel
    {
        [StringLength(30,MinimumLength =3)]
        public string Name { get; set; }

        [Range(typeof(decimal),"0.01","790000000000")]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
