using CleanDDDArchitecture.Domains.Todo.Core.Entities;
using CleanDDDArchitecture.Domains.Todo.CrossCutting;
using CleanDDDArchitecture.Domains.Todo.Infrastructure.Persistence.Contexts;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.GetBy;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Infrastructure.Repositories;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.GetAll;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Infrastructure.Repositories;
using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Aviant.Application.Exceptions;
using Aviant.Application.Identity;
using Xunit;

namespace CleanDDDArchitecture.Domains.Todo.Tests.Integration;

public sealed class TodoQueryAndValidationTests
{
    [Fact]
    public async Task CreateTodoListValidatorShouldRejectDuplicateSeededTitle()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var environment = await TodoTestEnvironment.CreateAsync(cancellationToken);
        var repository = new TodoListRepositoryRead(environment.ReadContext);
        // The seeded "Shopping" list belongs to Guid.Empty.
        var validator = new CreateTodoListInput.CreateTodoListInputValidator(repository, new CurrentUser(Guid.Empty));

        var result = await validator.ValidateAsync(new CreateTodoListInput("Shopping"), cancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.ErrorMessage == "You already have a list with this title.");
    }

    [Fact]
    public async Task CreateTodoListValidatorShouldAllowATitleAnotherUserAlreadyUses()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var environment = await TodoTestEnvironment.CreateAsync(cancellationToken);
        var repository = new TodoListRepositoryRead(environment.ReadContext);
        var validator = new CreateTodoListInput.CreateTodoListInputValidator(repository, new CurrentUser(Guid.NewGuid()));

        var result = await validator.ValidateAsync(new CreateTodoListInput("Shopping"), cancellationToken);

        result.IsValid.Should().BeTrue("list titles are unique per owner, not across all users");
    }

    private sealed record CurrentUser(Guid UserId) : ICurrentUserService;

    [Fact]
    public async Task GetTodoItemQueryHandlerShouldReturnSeededTodoTitle()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var environment = await TodoTestEnvironment.CreateAsync(cancellationToken);
        var repository = new TodoItemRepositoryRead(environment.ReadContext);
        var handler = new GetTodoItemQuery.GetTodoItemQueryHandler(repository);

        var title = await handler.Handle(new GetTodoItemQuery(-1), cancellationToken);

        title.Should().Be("Apples");
    }

    [Fact]
    public async Task GetTodoItemQueryHandlerShouldThrowNotFoundExceptionWhenItemDoesNotExist()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var environment = await TodoTestEnvironment.CreateAsync(cancellationToken);
        var repository = new TodoItemRepositoryRead(environment.ReadContext);
        var handler = new GetTodoItemQuery.GetTodoItemQueryHandler(repository);

        Func<Task> act = async () => await handler.Handle(new GetTodoItemQuery(999), cancellationToken);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Entity \"TodoItem\" (999) was not found.");
    }

    [Fact]
    public async Task GetTodosQueryHandlerShouldReturnSeededListAndPriorityLevels()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var environment = await TodoTestEnvironment.CreateAsync(cancellationToken);
        var handler = new GetTodosQuery.GetTodosQueryHandler(environment.ReadContext);

        var response = await handler.Handle(new GetTodosQuery(), cancellationToken);

        response.Lists.Should().ContainSingle(list => list.Title == "Shopping");
        response.PriorityLevels.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetTodosQueryHandlerShouldProjectListsWithTheirItems()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var environment = await TodoTestEnvironment.CreateAsync(cancellationToken);
        var handler = new GetTodosQuery.GetTodosQueryHandler(environment.ReadContext);

        var response = await handler.Handle(new GetTodosQuery(), cancellationToken);

        var shopping = response.Lists.Single(list => list.Title == "Shopping");
        shopping.Id.Should().Be(-1);
        shopping.Items.Should().HaveCount(8).And.OnlyContain(item => item.ListId == -1);
        shopping.Items.Should().ContainSingle(item => item.Id == -1 && item.Title == "Apples");
    }

    [Fact]
    public async Task ADeletedListIsKeptButNoLongerListed()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var environment = await TodoTestEnvironment.CreateAsync(cancellationToken);
        var shopping = await environment.WriteContext.TodoLists.SingleAsync(list => list.Id == -1, cancellationToken);

        environment.WriteContext.TodoLists.Remove(shopping);
        await environment.WriteContext.SaveChangesAsync(cancellationToken);

        var response = await new GetTodosQuery.GetTodosQueryHandler(environment.ReadContext)
           .Handle(new GetTodosQuery(), cancellationToken);
        response.Lists.Should().BeEmpty();

        var stored = await environment.ReadContext.TodoLists.IgnoreQueryFilters()
           .SingleAsync(list => list.Id == -1, cancellationToken);
        stored.IsDeleted.Should().BeTrue();
        stored.Deleted.Should().NotBeNull();
    }

    private sealed class TodoTestEnvironment : IAsyncDisposable
    {
        private readonly InMemoryDatabaseRoot _databaseRoot = new();
        private readonly DbContextOptions<TodoDbContextWrite> _writeOptions;
        private readonly DbContextOptions<TodoDbContextRead> _readOptions;

        private TodoTestEnvironment()
        {
            var databaseName = Guid.NewGuid().ToString("N");
            _writeOptions = new DbContextOptionsBuilder<TodoDbContextWrite>()
                .UseInMemoryDatabase(databaseName, _databaseRoot)
                .Options;
            _readOptions = new DbContextOptionsBuilder<TodoDbContextRead>()
                .UseInMemoryDatabase(databaseName, _databaseRoot)
                .Options;

            WriteContext = new TodoDbContextWrite(_writeOptions);
            ReadContext = new TodoDbContextRead(_readOptions);

        }

        public TodoDbContextWrite WriteContext { get; }

        public TodoDbContextRead ReadContext { get; }

        public static async Task<TodoTestEnvironment> CreateAsync(CancellationToken cancellationToken)
        {
            var environment = new TodoTestEnvironment();
            await environment.WriteContext.Database.EnsureCreatedAsync(cancellationToken);

            if (!await environment.WriteContext.TodoLists.AnyAsync(cancellationToken))
            {
                var shopping = TodoListEntity.Create("Shopping");
                shopping.Id        = -1;
                shopping.Created   = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                shopping.CreatedBy = Guid.Empty;
                environment.WriteContext.TodoLists.Add(shopping);

                var apples = TodoItemEntity.Create(listId: -1, title: "Apples");
                apples.Id = -1;
                environment.WriteContext.TodoItems.Add(apples);
                await environment.WriteContext.SaveChangesAsync(cancellationToken);
            }

            return environment;
        }

        public async ValueTask DisposeAsync()
        {
            await WriteContext.DisposeAsync();
            await ReadContext.DisposeAsync();
        }
    }
}
