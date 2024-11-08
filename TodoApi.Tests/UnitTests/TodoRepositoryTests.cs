using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApi.Data;
using TodoApi.Repositories;
using TodoApi.Services;
using TodoApi.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace TodoApi.Tests.UnitTests
{
    public class TodoRepositoryTests
    {
        private readonly TodoService _service;
        private readonly TodoDbContext _context;

        public TodoRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique database for each test
                .Options;

            _context = new TodoDbContext(options);
            _service = new TodoService(new HttpClient(), _context);
        }

        [Fact]
        public async Task GetTodos_ReturnsAllTodos()
        {
            // Arrange
            var todos = new List<TodoItem>
        {
            new TodoItem { Id = 1, Title = "Test Todo" }
        };
            await _context.TodoItems.AddRangeAsync(todos);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetTodosAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("Test Todo", result[0].Title);
        }

        [Fact]
        public async Task AddTodo_CreatesNewTodo()
        {
            // Arrange
            var todo = new TodoItem { Title = "New Todo" };

            // Act
            await _service.AddTodoAsync(todo);

            // Assert
            var todos = await _context.TodoItems.ToListAsync();
            Assert.Single(todos);
            Assert.Equal("New Todo", todos[0].Title);
        }

        [Fact]
        public async Task DeleteTodo_DeletesExistingTodo()
        {
            // Arrange
            var todo = new TodoItem { Id = 1, Title = "Test Todo" };
            await _context.TodoItems.AddAsync(todo);
            await _context.SaveChangesAsync();

            // Act
            await _service.DeleteTodoAsync(todo.Id);

            // Assert
            var todos = await _context.TodoItems.ToListAsync();
            Assert.Empty(todos);
        }
    }
}
