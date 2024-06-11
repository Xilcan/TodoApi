using NUnit.Framework;
using Moq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Data;
using TodoApi.Interfaces;
using TodoApi.Models;
using TodoApi.Repositories;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace TestTodoAPI
{
    [TestFixture]
    public class TodoRepositoryTests
    {
        private TodoContext _context;
        private ITodoRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: "TodoDatabase")
                .Options;

            _context = new TodoContext(options);
            _repository = new TodoRepository(_context);
        }

        [TearDown]
        public void Teardown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetTodoItems_ReturnsAllTodoItems()
        {
            // Arrange
            _context.TodoItems.AddRange(
                new TodoItem { Id = 1, Title = "Task 1", Description = "Description 1" },
                new TodoItem { Id = 2, Title = "Task 2", Description = "Description 2" }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetTodoItems();

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Task 1", result.First().Title);
        }

        [Test]
        public async Task GetTodoItems_ReturnsEmptyListWhenNoItems()
        {
            // Act
            var result = await _repository.GetTodoItems();

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetTodoItem_ReturnsCorrectTodoItem()
        {
            // Arrange
            _context.TodoItems.AddRange(
                new TodoItem { Id = 1, Title = "Task 1", Description = "Description 1" },
                new TodoItem { Id = 2, Title = "Task 2", Description = "Description 2" }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetTodoItem(1);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Task 1", result.Title);
        }

        [Test]
        public async Task GetTodoItem_ReturnsNullWhenItemDoesNotExist()
        {
            // Act
            var result = await _repository.GetTodoItem(99);

            // Assert
            Assert.Null(result);
        }

        [Test]
        public async Task AddTodoItem_AddsItemToDatabase()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Task 1", Description = "Description 1" };

            // Act
            await _repository.AddTodoItem(todoItem);

            // Assert
            var result = await _context.TodoItems.FindAsync(todoItem.Id);
            Assert.NotNull(result);
            Assert.AreEqual("Task 1", result.Title);
        }

        [Test]
        public void AddTodoItem_DoesNotAddNullItem()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _repository.AddTodoItem(null));
        }

        [Test]
        public async Task UpdateTodoItem_UpdatesItemInDatabase()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Task 1", Description = "Description 1" };
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            todoItem.Title = "Updated Task 1";

            // Act
            await _repository.UpdateTodoItem(todoItem);

            // Assert
            var result = await _context.TodoItems.FindAsync(todoItem.Id);
            Assert.NotNull(result);
            Assert.AreEqual("Updated Task 1", result.Title);
        }

        [Test]
        public void UpdateTodoItem_ThrowsExceptionWhenItemDoesNotExist()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Task 1", Description = "Description 1" };

            // Act & Assert
            Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _repository.UpdateTodoItem(todoItem));
        }

        [Test]
        public async Task DeleteTodoItem_RemovesItemFromDatabase()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Task 1", Description = "Description 1" };
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteTodoItem(1);

            // Assert
            var result = await _context.TodoItems.FindAsync(todoItem.Id);
            Assert.Null(result);
        }

        [Test]
        public async Task DeleteTodoItem_DoesNothingWhenItemDoesNotExist()
        {
            // Act
            await _repository.DeleteTodoItem(99);

            // Assert
            var result = await _context.TodoItems.FindAsync(99);
            Assert.Null(result);
        }

        [Test]
        public async Task TodoItemExists_ReturnsTrueIfItemExists()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Task 1", Description = "Description 1" };
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.TodoItemExists(1);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task TodoItemExists_ReturnsFalseWhenItemDoesNotExist()
        {
            // Act
            var result = await _repository.TodoItemExists(99);

            // Assert
            Assert.IsFalse(result);
        }
    }
    }
