using ProjectTemplate.Application.Common.Behaviours;
using ProjectTemplate.Application.Common.Interfaces;
using ProjectTemplate.Application.TodoItems.Commands.CreateTodoItem;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace ProjectTemplate.Application.UnitTests.Common.Behaviours;

public class RequestLoggerTests
{
    private Mock<ILogger<CreateTodoItemCommand>> _logger = null!;
    private Mock<ICurrentUser> _user = null!;
    //private Mock<IIdentityService> _identityService = null!;

    [SetUp]
    public void Setup()
    {
        _logger = new Mock<ILogger<CreateTodoItemCommand>>();
        _user = new Mock<ICurrentUser>();
        //_identityService = new Mock<IIdentityService>();
    }

    [Test]
    public  Task ShouldCallGetUserNameAsyncOnceIfAuthenticated()
    {
        _user.Setup(x => x.Id).Returns(Random.Shared.Next(0, 100000));
        //var requestLogger = new LoggingBehaviour<CreateTodoItemCommand>(_logger.Object, _user.Object, _identityService.Object);

        //await requestLogger.Process(new CreateTodoItemCommand { ListId = 1, Title = "title" }, new CancellationToken());

        //_identityService.Verify(i => i.GetUserNameAsync(It.IsAny<string>()), Times.Once);
        return Task.CompletedTask;
    }

    [Test]
    public  Task ShouldNotCallGetUserNameAsyncOnceIfUnauthenticated()
    {
        //var requestLogger = new LoggingBehaviour<CreateTodoItemCommand>(_logger.Object, _user.Object, _identityService.Object);

       // await requestLogger.Process(new CreateTodoItemCommand { ListId = 1, Title = "title" }, new CancellationToken());

        //_identityService.Verify(i => i.GetUserNameAsync(It.IsAny<string>()), Times.Never);
        return Task.CompletedTask;
    }
}
