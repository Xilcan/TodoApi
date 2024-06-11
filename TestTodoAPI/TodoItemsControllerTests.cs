using NUnit.Framework;
using Moq;
using TodoApi.Controllers;
using TodoApi.Interfaces;
using TodoApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;

namespace TestTodoAPI
{
    [TestFixture]
    public class TodoItemsControllerTests
    {
        private Mock<ITodoService> _todoServiceMock;
        private TodoItemsController _controller;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _todoServiceMock = new Mock<ITodoService>();
            _controller = new TodoItemsController(_todoServiceMock.Object);
            _fixture = new Fixture();
        }

        [Test]
        public async Task GetTasks_ReturnsOkResult_WithListOfTodoItems()
        {
            // Arrange
            var todoItems = _fixture.Create<List<TodoItem>>();
            _todoServiceMock.Setup(service => service.GetTodoItems()).ReturnsAsync(todoItems);

            // Act
            var result = await _controller.GetTasks();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.IsInstanceOf<IEnumerable<TodoItem>>(okResult.Value);
            Assert.That(okResult.Value, Is.EqualTo(todoItems));
        }

        [Test]
        public async Task GetTask_ExistingId_ReturnsOkResult_WithTodoItem()
        {
            // Arrange
            var todoItem = _fixture.Create<TodoItem>();
            _todoServiceMock.Setup(service => service.GetTodoItem(todoItem.Id)).ReturnsAsync(todoItem);

            // Act
            var result = await _controller.GetTask(todoItem.Id);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.IsInstanceOf<TodoItem>(okResult.Value);
            Assert.That(okResult.Value, Is.EqualTo(todoItem));
        }

        [Test]
        public async Task GetTask_NonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            var nonExistingId = _fixture.Create<int>();
            _todoServiceMock.Setup(service => service.GetTodoItem(nonExistingId)).ReturnsAsync((TodoItem)null);

            // Act
            var result = await _controller.GetTask(nonExistingId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task PostTask_ValidTask_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var todoItem = _fixture.Create<TodoItem>();

            // Act
            var result = await _controller.PostTask(todoItem);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
            var createdAtResult = result.Result as CreatedAtActionResult;
            Assert.That(createdAtResult, Is.Not.Null);
            Assert.That(createdAtResult.ActionName, Is.EqualTo("GetTask"));
            Assert.That(createdAtResult.RouteValues["id"], Is.EqualTo(todoItem.Id));
            Assert.That(createdAtResult.Value, Is.EqualTo(todoItem));
        }

        [Test]
        public async Task PostTask_NullTask_ReturnsInternalServerError()
        {
            // Act
            var result = await _controller.PostTask(null);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            var objectResult = result.Result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task PutTask_ValidTask_ReturnsNoContentResult()
        {
            // Arrange
            var todoItem = _fixture.Create<TodoItem>();

            _todoServiceMock.Setup(service => service.UpdateTodoItem(todoItem)).Returns(Task.CompletedTask);
            _todoServiceMock.Setup(service => service.TodoItemExists(todoItem.Id)).ReturnsAsync(true);

            // Act
            var result = await _controller.PutTask(todoItem.Id, todoItem);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task PutTask_MismatchedIds_ReturnsBadRequestResult()
        {
            // Arrange
            var todoItem = _fixture.Create<TodoItem>();
            var differentId = _fixture.Create<int>();

            // Act
            var result = await _controller.PutTask(differentId, todoItem);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task PutTask_NonExistingTask_ReturnsNotFoundResult()
        {
            // Arrange
            var todoItem = _fixture.Create<TodoItem>();

            _todoServiceMock.Setup(service => service.TodoItemExists(todoItem.Id)).ReturnsAsync(false);

            // Act
            var result = await _controller.PutTask(todoItem.Id, todoItem);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task PutTask_NullTask_ReturnsInternalServerError()
        {
            // Act
            var result = await _controller.PutTask(1, null);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }
        [Test]
        public async Task DeleteTask_ExistingId_ReturnsNoContentResult()
        {
            // Arrange
            var todoItem = _fixture.Create<TodoItem>();

            _todoServiceMock.Setup(service => service.GetTodoItem(todoItem.Id)).ReturnsAsync(todoItem);
            _todoServiceMock.Setup(service => service.DeleteTodoItem(todoItem.Id)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteTask(todoItem.Id);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task DeleteTask_NonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            var nonExistingId = _fixture.Create<int>();
            _todoServiceMock.Setup(service => service.GetTodoItem(nonExistingId)).ReturnsAsync((TodoItem)null);

            // Act
            var result = await _controller.DeleteTask(nonExistingId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task PostTask_InvalidModelState_ReturnsBadRequestResult()
        {
            // Arrange
            _controller.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = await _controller.PostTask(new TodoItem());

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result.Result);
        }

        [Test]
        public async Task PutTask_InvalidModelState_ReturnsBadRequestResult()
        {
            // Arrange
            var todoItem = _fixture.Create<TodoItem>();
            _controller.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = await _controller.PutTask(todoItem.Id, todoItem);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task DeleteTask_ExistingId_DeletesTodoItem()
        {
            // Arrange
            var todoItem = _fixture.Create<TodoItem>();
            _todoServiceMock.Setup(service => service.GetTodoItem(todoItem.Id)).ReturnsAsync(todoItem);
            _todoServiceMock.Setup(service => service.DeleteTodoItem(todoItem.Id)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteTask(todoItem.Id);

            // Assert
            _todoServiceMock.Verify(service => service.DeleteTodoItem(todoItem.Id), Times.Once);
        }

        [Test]
        public async Task DeleteTask_NonExistingId_DoesNotDeleteTodoItem()
        {
            // Arrange
            var nonExistingId = _fixture.Create<int>();
            _todoServiceMock.Setup(service => service.GetTodoItem(nonExistingId)).ReturnsAsync((TodoItem)null);

            // Act
            var result = await _controller.DeleteTask(nonExistingId);

            // Assert
            _todoServiceMock.Verify(service => service.DeleteTodoItem(nonExistingId), Times.Never);
        }

        [Test]
        public async Task PutTask_ExistingId_InvalidModel_ReturnsBadRequestResult()
        {
            // Arrange
            var todoItem = _fixture.Create<TodoItem>();
            _controller.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = await _controller.PutTask(todoItem.Id, todoItem);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task PutTask_NonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            var todoItem = _fixture.Create<TodoItem>();

            _todoServiceMock.Setup(service => service.TodoItemExists(todoItem.Id)).ReturnsAsync(false);

            // Act
            var result = await _controller.PutTask(todoItem.Id, todoItem);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task PutTask_ExistingId_ValidModel_ReturnsNoContentResult()
        {
            // Arrange
            var todoItem = _fixture.Create<TodoItem>();

            _todoServiceMock.Setup(service => service.UpdateTodoItem(todoItem)).Returns(Task.CompletedTask);
            _todoServiceMock.Setup(service => service.TodoItemExists(todoItem.Id)).ReturnsAsync(true);

            // Act
            var result = await _controller.PutTask(todoItem.Id, todoItem);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }
    }
}