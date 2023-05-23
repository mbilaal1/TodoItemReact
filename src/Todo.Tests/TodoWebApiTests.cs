using Bogus;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using Todo.Api.Controllers;
using Todo.Api.Handlers;
using Todo.Data;
using Todo.Data.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Todo.Api.Tests
{
    [TestFixture]
    public class TodoControllerTests
    {
        private Mock<ISender> _senderMock;
        private TodoController _todoController;
        private List<TodoItem> expectedResult;
        private Mock<ITodoRepository> _todoRepositoryMock;
        private ListTodoItemsHandler _handler;

        [SetUp]
        public void Setup()
        {
            _senderMock = new Mock<ISender>();
            _todoController = new TodoController(_senderMock.Object);
            _todoRepositoryMock = new Mock<ITodoRepository>();
            _handler = new ListTodoItemsHandler(_todoRepositoryMock.Object);



            var itemFaker = new Faker<TodoItem>()
                .RuleFor(t => t.Id, f => Guid.NewGuid())
                .RuleFor(t => t.Completed, f => f.Random.Bool() ? f.Date.Past() : null)
                .RuleFor(t => t.Created, (f, t) => f.Date.Past(refDate: t.Completed))
                .RuleFor(t => t.Text, f => f.Lorem.Sentence());

            expectedResult = itemFaker.Generate(5);
        }

        [Test]
        public async Task List_ReturnsOkResult()
        {
            // Arrange
            var includeCompleted = true;
            _senderMock.Setup(mock => mock.Send(It.IsAny<ListTodoItemsRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResult);

            // Act
            var result = await _todoController.List(includeCompleted);


            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);

            var okResult = (OkObjectResult)result;
            Assert.IsInstanceOf<IEnumerable<TodoItem>>(okResult.Value);

            var todoItems = (IEnumerable<TodoItem>)okResult.Value;
            Assert.That(todoItems.Count(), Is.EqualTo(expectedResult.Count));

        }

        [Test]
        public async Task Handle_CompletesTodoItem()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var item = new TodoItem { Id = itemId, Text = "jeeeeeeeee", Completed = null };

            var todoRepositoryMock = new Mock<ITodoRepository>();
            todoRepositoryMock.Setup(mock => mock.GetItemById(itemId)).ReturnsAsync(item);

            var completeTodoItemRequest = new CompleteTodoItemRequest(itemId);
            var handler = new CompleteTodoItemHandler(todoRepositoryMock.Object);

            // Act
            var result = await handler.Handle(completeTodoItemRequest, CancellationToken.None);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(item.Completed);
            todoRepositoryMock.Verify(mock => mock.Update(item), Times.Once);
        }

        [Test]
        public async Task Handle_ValidTodoItemRequest_CreatesItem()
        {
            // Arrange
            var todoRepositoryMock = new Mock<ITodoRepository>();
            todoRepositoryMock.Setup(mock => mock.Create(It.IsAny<TodoItem>())).ReturnsAsync(Guid.NewGuid());

            var createTodoItemRequest = new CreateTodoItemRequest("Sample Text");
            var handler = new CreateTodoItemHandler(todoRepositoryMock.Object);

            // Act
            var result = await handler.Handle(createTodoItemRequest, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            todoRepositoryMock.Verify(mock => mock.Create(It.IsAny<TodoItem>()), Times.Once);
        }

        [Test]
        public async Task Handle_ReturnsMostRecentTodoItems()
        {
            // Arrange
            var todoRepositoryMock = new Mock<ITodoRepository>();
            var expectedItems = new List<TodoItem>
            {
                new TodoItem { Id = Guid.NewGuid(), Text = "Item 1", Created = DateTime.UtcNow.AddHours(-2) },
                new TodoItem { Id = Guid.NewGuid(), Text = "Item 2", Created = DateTime.UtcNow.AddHours(-1) },
                new TodoItem { Id = Guid.NewGuid(), Text = "Item 3", Created = DateTime.UtcNow },
            };
            todoRepositoryMock.Setup(mock => mock.SortTodoByMostRecent()).ReturnsAsync(expectedItems);

            var handler = new ListMostRecentTodoHandler(todoRepositoryMock.Object);
            var request = new ListMostRecentTodoRequest();

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedItems.Count, result.Count());
            CollectionAssert.AreEquivalent(expectedItems, result);
            todoRepositoryMock.Verify(mock => mock.SortTodoByMostRecent(), Times.Once);
        }

        [Test]
        public async Task Handle_IncludeCompletedTrue_ReturnsAllTodoItems()
        {
            // Arrange
            var todoRepositoryMock = new Mock<ITodoRepository>();
            var expectedItems = new List<TodoItem>
            {
                new TodoItem { Id = Guid.NewGuid(), Text = "Item 1", Completed = DateTime.UtcNow.AddHours(-2) },
                new TodoItem { Id = Guid.NewGuid(), Text = "Item 2", Completed = DateTime.UtcNow.AddHours(-1) },
                new TodoItem { Id = Guid.NewGuid(), Text = "Item 3", Completed = DateTime.UtcNow },
                new TodoItem { Id = Guid.NewGuid(), Text = "Item 4", Completed = null },
                new TodoItem { Id = Guid.NewGuid(), Text = "Item 5", Completed = null },
            };
            todoRepositoryMock.Setup(mock => mock.List(true)).ReturnsAsync(expectedItems);

            var handler = new ListTodoItemsHandler(todoRepositoryMock.Object);
            var request = new ListTodoItemsRequest(true);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedItems.Count, result.Count());
            CollectionAssert.AreEquivalent(expectedItems, result);
            todoRepositoryMock.Verify(mock => mock.List(true), Times.Once);
        }

        [Test]
        public async Task Handle_IncludeCompletedFalse_ReturnsIncompleteTodoItems()
        {
            // Arrange
            var request = new ListTodoItemsRequest(includeCompleted: false);
            var expectedIncompleteItems = new List<TodoItem>
            {
                new TodoItem { Id = Guid.NewGuid(), Text = "Task 1", Completed = null },
                new TodoItem { Id = Guid.NewGuid(), Text = "Task 2", Completed = null }
            };

            var completedItem = new TodoItem { Id = Guid.NewGuid(), Text = "Task 3", Completed = DateTime.UtcNow };

            var allItems = expectedIncompleteItems.Concat(new[] { completedItem });

            _todoRepositoryMock
                .Setup(repository => repository.List(false))
                .ReturnsAsync(expectedIncompleteItems);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedIncompleteItems.Count, result.Count());
            Assert.IsTrue(result.All(item => item.Completed == null));
        }


    }
}
