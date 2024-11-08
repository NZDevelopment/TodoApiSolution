namespace TodoApi.Models
{
    /*
    public class Category
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? ParentCategoryId { get; set; } // Nullable if it's optional

        // Navigation property to represent the relationship with TodoItems
        public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>(); // Initialize to avoid null reference issues
    }
    */

    public class Category
    {
        public int Id { get; set; }
        public string Title { get; set; }

        // Optional foreign key to represent the parent category
        public int? ParentCategoryId { get; set; }

        // Navigation property for the parent category
        public Category? ParentCategory { get; set; }

        // Collection of subcategories for hierarchical structure
        public ICollection<Category> SubCategories { get; set; } = new List<Category>();

        // Collection of associated TodoItems
        public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
    }
}


