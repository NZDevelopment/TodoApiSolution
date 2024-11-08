using TodoApi.Models;

namespace TodoApi.Repositories
{
    public interface ITodoRepository
    {
        Task<IEnumerable<TodoItem>> GetTodosAsync();
        Task<TodoItem?> GetTodoByIdAsync(int id);
        Task AddTodoAsync(TodoItem todo);
        Task UpdateTodoAsync(TodoItem todo);
        Task DeleteTodoAsync(int id);
        Task<IEnumerable<Category>> GetCategoriesAsync(); // For category retrieval
    }
}
