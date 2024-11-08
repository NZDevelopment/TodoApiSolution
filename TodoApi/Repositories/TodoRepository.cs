using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        private readonly TodoDbContext _context;

        public TodoRepository(TodoDbContext context) => _context = context;

        public async Task<IEnumerable<TodoItem>> GetTodosAsync() => await _context.TodoItems.ToListAsync();

        public async Task<TodoItem?> GetTodoByIdAsync(int id) => await _context.TodoItems.FindAsync(id);

        public async Task AddTodoAsync(TodoItem todo)
        {
            _context.TodoItems.Add(todo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTodoAsync(TodoItem todo)
        {
            _context.Entry(todo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTodoAsync(int id)
        {
            var todo = await _context.TodoItems.FindAsync(id);
            if (todo != null) _context.TodoItems.Remove(todo);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Category>> GetCategoriesAsync() => await _context.Categories.ToListAsync();
    }
}
