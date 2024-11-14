namespace TodoApi.Models
{

    public class Category
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }

        public ICollection<Category> SubCategories { get; set; } = new List<Category>();

        public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
    }
}


