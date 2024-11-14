using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Data;
using TodoApi.Models;
using TodoApi.Services;

public static class SeedData
{

    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<TodoDbContext>();

            if (!context.Categories.Any())
            {
                // parent category
                var workCategory = new Category { Title = "Work" };
                context.Categories.Add(workCategory);

                // Save to get generated ID for the parent category
                await context.SaveChangesAsync();

                var categories = new[]
                {
                    new Category { Title = "Meetings", ParentCategoryId = workCategory.Id },
                    new Category { Title = "Reports", ParentCategoryId = workCategory.Id },
                    new Category { Title = "Urgent" }  
                };

                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // Fetch & store todos if needed
            var todoService = scope.ServiceProvider.GetRequiredService<TodoService>();
            await todoService.FetchAndStoreTodosAsync();
        }
    }
}
