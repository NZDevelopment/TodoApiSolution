using System.Text.Json.Serialization;

namespace TodoApi.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Priority { get; set; } = 3; // Default priority
        public string? Location { get; set; } 
        public DateTime? DueDate { get; set; } 
        public double? Latitude { get; set; } 
        public double? Longitude { get; set; } 

        // Foreign key for the relationship with Category
        public int? CategoryId { get; set; }

        [JsonIgnore]
        public Category? Category { get; set; } 

        public bool IsCompleted { get; set; } = false;
    }


}
