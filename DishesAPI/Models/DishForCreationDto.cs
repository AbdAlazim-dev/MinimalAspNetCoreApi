using System.ComponentModel.DataAnnotations;

namespace DishesAPI.Models
{
    public class DishForCreationDto
    {
        [Required]
        public required string Name { get; set; }
    }
}
