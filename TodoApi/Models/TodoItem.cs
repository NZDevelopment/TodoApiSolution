using System.Text.Json.Serialization;

namespace TodoApi.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Priority { get; set; } = 3; // Default priority
        public string? Location { get; set; } // Optional
        public DateTime? DueDate { get; set; } // Optional
        public double? Latitude { get; set; } // Optional
        public double? Longitude { get; set; } // Optional

        // Foreign key for the relationship with Category
        public int? CategoryId { get; set; } // Nullable if it's optional

        // Navigation property to represent the relationship with Category
        [JsonIgnore]
        public Category? Category { get; set; } // Nullable to indicate optional relationship

        // New property for completion status
        public bool IsCompleted { get; set; } = false; // Default to not completed
    }


}
