﻿using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Services
{
    public class TodoService
    {
        private readonly HttpClient _httpClient;
        private readonly TodoDbContext _context;

        public TodoService(HttpClient httpClient, TodoDbContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        public async Task FetchAndStoreTodosAsync()
        {
            // Fetch data from the external API
            var response = await _httpClient.GetStringAsync("https://dummyjson.com/todos");

            // Deserialize the response
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var todos = JsonSerializer.Deserialize<TodoApiResponse>(response, options);

            if (todos?.Todos == null)
            {
                throw new Exception("Failed to fetch or deserialize todos.");
            }

            foreach (var todo in todos.Todos)
            {
                var todoItem = new TodoItem
                {
                    Title = todo.Todo,
                    IsCompleted = todo.Completed,
                    CategoryId = _context.Categories.First().Id,
                    Latitude = null, 
                    Longitude = null,
                    DueDate = DateTime.Now.AddDays(7) 
                };
                _context.TodoItems.Add(todoItem);
            }

            // Save changes to db
            await _context.SaveChangesAsync();
        }

        public async Task<List<TodoItem>> GetTodosAsync()
        {
            try
            {
                return await _context.TodoItems.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetTodosAsync: {ex.Message}");
                throw; 
            }
        }

        public async Task AddTodoAsync(TodoItem todo)
        {
            _context.TodoItems.Add(todo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTodoAsync(int id)
        {
            var todo = await _context.TodoItems.FindAsync(id);
            if (todo != null)
            {
                _context.TodoItems.Remove(todo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<TodoItem>> GetIncompleteTodosAsync()
        {
            return await _context.TodoItems.Where(t => !t.IsCompleted).ToListAsync();
        }
    }

    public class TodoApiResponse
    {
        public TodoItemApi[] Todos { get; set; }
        public int Total { get; set; }
        public int Skip { get; set; }
        public int Limit { get; set; }
    }

    public class TodoItemApi
    {
        public int Id { get; set; }
        public string Todo { get; set; }
        public bool Completed { get; set; }
        public int UserId { get; set; }
    }
}