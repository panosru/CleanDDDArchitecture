using AutoMapper;
using CleanDDDArchitecture.Domains.Todo.Core.Entities;
using CleanDDDArchitecture.Domains.Todo.CrossCutting;
using CleanDDDArchitecture.Domains.Todo.Infrastructure.Persistence.Contexts;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.GetBy;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Infrastructure.Repositories;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.GetAll;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Aviant.Application.Exceptions;
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
        var validator = new CreateTodoListInput.CreateTodoListInputValidator(repository);

        var result = await validator.ValidateAsync(new CreateTodoListInput("Shopping"), cancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.ErrorMessage == "The specified title already exists.");
    }

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
        var handler = new GetTodosQuery.GetTodosQueryHandler(environment.ReadContext, environment.Mapper);

        var response = await handler.Handle(new GetTodosQuery(), cancellationToken);

        response.Lists.Should().ContainSingle(list => list.Title == "Shopping");
        response.PriorityLevels.Should().NotBeEmpty();
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

            var mapperConfiguration = new MapperConfiguration(configuration =>
            {
                foreach (var profile in TodoCrossCutting.AutoMapperProfiles())
                {
                    configuration.AddProfile(profile);
                }
            });

            Mapper = mapperConfiguration.CreateMapper();
        }

        public TodoDbContextWrite WriteContext { get; }

        public TodoDbContextRead ReadContext { get; }

        public IMapper Mapper { get; }

        public static async Task<TodoTestEnvironment> CreateAsync(CancellationToken cancellationToken)
        {
            var environment = new TodoTestEnvironment();
            await environment.WriteContext.Database.EnsureCreatedAsync(cancellationToken);

            if (!await environment.WriteContext.TodoLists.AnyAsync(cancellationToken))
            {
                environment.WriteContext.TodoLists.Add(
                    new TodoListEntity
                    {
                        Id = -1,
                        Title = "Shopping",
                        Created = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        CreatedBy = Guid.Empty
                    });
                environment.WriteContext.TodoItems.Add(
                    new TodoItemEntity
                    {
                        Id = -1,
                        ListId = -1,
                        Title = "Apples"
                    });
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
