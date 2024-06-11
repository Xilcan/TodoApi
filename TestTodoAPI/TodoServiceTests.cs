using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApi.Interfaces;
using TodoApi.Models;
using TodoApi.Services;

namespace TestTodoAPI
{
    [TestFixture]
    public class TodoServiceTests
    {
        private Mock<ITodoRepository> _todoRepositoryMock;
        private TodoService _todoService;

        [SetUp]
        public void Setup()
        {
            _todoRepositoryMock = new Mock<ITodoRepository>();
            _todoService = new TodoService(_todoRepositoryMock.Object);
        }

        [Test]
        public async Task GetTodoItems_ReturnsListOfTodoItems()
        {
            // Arrange
            var todoItems = new List<TodoItem> { new TodoItem { Id = 1, Title = "Task1", Description = "Description1" } };
            _todoRepositoryMock.Setup(repo => repo.GetTodoItems()).ReturnsAsync(todoItems);

            // Act
            var result = await _todoService.GetTodoItems();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(todoItems));
        }

        [Test]
        public async Task GetTodoItem_ExistingId_ReturnsTodoItem()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Task1", Description = "Description1" };
            _todoRepositoryMock.Setup(repo => repo.GetTodoItem(1)).ReturnsAsync(todoItem);

            // Act
            var result = await _todoService.GetTodoItem(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(todoItem));
        }

        [Test]
        public async Task GetTodoItem_NonExistingId_ReturnsNull()
        {
            // Arrange
            _todoRepositoryMock.Setup(repo => repo.GetTodoItem(1)).ReturnsAsync((TodoItem)null);

            // Act
            var result = await _todoService.GetTodoItem(1);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task AddTodoItem_ValidTodoItem_CallsRepositoryMethod()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Task1", Description = "Description1" };

            // Act
            await _todoService.AddTodoItem(todoItem);

            // Assert
            _todoRepositoryMock.Verify(repo => repo.AddTodoItem(todoItem), Times.Once);
        }

        [Test]
        public async Task UpdateTodoItem_ValidTodoItem_CallsRepositoryMethod()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Task1", Description = "Description1" };

            // Act
            await _todoService.UpdateTodoItem(todoItem);

            // Assert
            _todoRepositoryMock.Verify(repo => repo.UpdateTodoItem(todoItem), Times.Once);
        }

        [Test]
        public async Task DeleteTodoItem_ExistingId_CallsRepositoryMethod()
        {
            // Arrange
            var id = 1;
            var todoItem = new TodoItem { Id = id };
            _todoRepositoryMock.Setup(repo => repo.GetTodoItem(id)).ReturnsAsync(todoItem);
            _todoRepositoryMock.Setup(repo => repo.TodoItemExists(id)).ReturnsAsync(true);

            // Act
            await _todoService.DeleteTodoItem(id);

            // Assert
            _todoRepositoryMock.Verify(repo => repo.DeleteTodoItem(id), Times.Once);
        }

        [Test]
        public async Task TodoItemExists_ExistingId_ReturnsTrue()
        {
            // Arrange
            _todoRepositoryMock.Setup(repo => repo.TodoItemExists(1)).ReturnsAsync(true);

            // Act
            var result = await _todoService.TodoItemExists(1);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task TodoItemExists_NonExistingId_ReturnsFalse()
        {
            // Arrange
            _todoRepositoryMock.Setup(repo => repo.TodoItemExists(1)).ReturnsAsync(false);

            // Act
            var result = await _todoService.TodoItemExists(1);

            // Assert
            Assert.That(result, Is.False);
        }
        [Test]
        public async Task AddTodoItem_NullTodoItem_DoesNotCallRepositoryMethod()
        {
            // Act
            await _todoService.AddTodoItem(null);

            // Assert
            _todoRepositoryMock.Verify(repo => repo.AddTodoItem(It.IsAny<TodoItem>()), Times.Never);
        }

        [Test]
        public async Task UpdateTodoItem_NullTodoItem_DoesNotCallRepositoryMethod()
        {
            // Act
            await _todoService.UpdateTodoItem(null);

            // Assert
            _todoRepositoryMock.Verify(repo => repo.UpdateTodoItem(It.IsAny<TodoItem>()), Times.Never);
        }

        [Test]
        public async Task DeleteTodoItem_NonExistingId_DoesNotCallRepositoryMethod()
        {
            // Arrange
            var nonExistingId = 999;

            // Act
            await _todoService.DeleteTodoItem(nonExistingId);

            // Assert
            _todoRepositoryMock.Verify(repo => repo.DeleteTodoItem(nonExistingId), Times.Never);
        }

        [Test]
        public async Task DeleteTodoItem_NonExistingId_DoesNotThrow()
        {
            // Arrange
            var nonExistingId = 999;

            // Act & Assert
            Assert.DoesNotThrowAsync(async () => await _todoService.DeleteTodoItem(nonExistingId));
        }

    }
}
