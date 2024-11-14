using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;
using TodoApi.Repositories;
using TodoApi.Services;


namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoRepository _todoRepository;
        private readonly WeatherService _weatherService;
        private readonly TodoService _todoService;
        private readonly TodoDbContext _context;
        private readonly ILogger<TodoController> _logger;



    public TodoController(
        ITodoRepository todoRepository,
        TodoService todoService,
        WeatherService weatherService,
        TodoDbContext context,
        ILogger<TodoController> logger)
        {
            _todoRepository = todoRepository;
            _todoService = todoService;
            _weatherService = weatherService;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetTodos()
        {
            try
            {
                var todos = await _context.TodoItems.Include(t => t.Category).ToListAsync();
                return Ok(todos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching ToDo items.");
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while retrieving ToDo items.",
                    detail = ex.Message
                });
            }
        }



        [HttpPost]
        public async Task<IActionResult> AddTodoItem(TodoItem todo)
        {
            try
            {
                // Check if categoryId exists in the Categories table
                var categoryExists = await _context.Categories.AnyAsync(c => c.Id == todo.CategoryId);

                if (!categoryExists)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Invalid category ID provided."
                    });
                }

                // Proceed to add the todo item if the categoryId is valid
                await _todoRepository.AddTodoAsync(todo);
                return CreatedAtAction(nameof(GetTodoById), new { id = todo.Id }, todo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    StatusCode = 500,
                    Message = "Error adding To-Do item",
                    Detail = ex.Message
                });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(int id, TodoItem updatedTodo)
        {
            // Check if the ID in the path matches the ID in the body
            if (id != updatedTodo.Id)
                return BadRequest(new ApiErrorResponse { StatusCode = 400, Message = "ID mismatch" });

            // Check if the provided CategoryId exists
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == updatedTodo.CategoryId);
            if (!categoryExists)
            {
                return BadRequest(new ApiErrorResponse
                {
                    StatusCode = 400,
                    Message = "Invalid CategoryId",
                    Detail = $"The CategoryId {updatedTodo.CategoryId} does not exist. Please provide a valid CategoryId."
                });
            }

            try
            {
                // Proceed with the update if validation passes
                await _todoRepository.UpdateTodoAsync(updatedTodo);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound(new ApiErrorResponse { StatusCode = 404, Message = "ToDo item not found for update" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    StatusCode = 500,
                    Message = "Error updating ToDo item",
                    Detail = ex.Message
                });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            try
            {
                await _todoRepository.DeleteTodoAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    StatusCode = 500,
                    Message = "Error deleting To-Do item",
                    Detail = ex.Message
                });
            }
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchTodos(string? title, int? priority, DateTime? dueDate) // Make dueDate nullable
        {
            var todos = await _todoRepository.GetTodosAsync();

            if (!string.IsNullOrWhiteSpace(title))
            {
                todos = todos.Where(t => t.Title.Contains(title));
            }
            if (priority.HasValue)
            {
                todos = todos.Where(t => t.Priority == priority);
            }
            if (dueDate.HasValue) // Check if dueDate has a value
            {
                // Access Date property safely
                todos = todos.Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == dueDate.Value.Date);
            }

            return Ok(todos);
        }

        [HttpGet("{id}/weather")]
        public async Task<IActionResult> GetTodoWeather(int id)
        {
            var todo = await _todoRepository.GetTodoByIdAsync(id);
            if (todo == null || todo.Latitude == null || todo.Longitude == null)
            {
                return NotFound();
            }

            var weather = await _weatherService.GetWeatherAsync(todo.Latitude.Value, todo.Longitude.Value);
            return Ok(weather);
        }


        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _todoRepository.GetCategoriesAsync();
            return Ok(categories);
        }

        // Example usage in a controller action
        [HttpGet("incomplete")]
        public async Task<IActionResult> GetIncompleteTodos()
        {
            var incompleteTodos = await _todoService.GetIncompleteTodosAsync();
            return Ok(incompleteTodos);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoById(int id)
        {
            var todo = await _todoRepository.GetTodoByIdAsync(id);

            if (todo == null)
            {
                return NotFound(new ApiErrorResponse { StatusCode = 404, Message = "To-Do item not found" });
            }

            var response = new
            {
                todo.Id,
                todo.Title,
                todo.Priority,
                todo.Location,
                DueDate = todo.DueDate?.Date,
                todo.Latitude,
                todo.Longitude
            };

            return Ok(response);
        }

    }

}
