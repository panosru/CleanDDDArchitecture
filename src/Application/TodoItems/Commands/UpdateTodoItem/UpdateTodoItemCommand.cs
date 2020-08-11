namespace CleanDDDArchitecture.Application.TodoItems.Commands.UpdateTodoItem
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Application.Exceptions;
    using Aviant.DDD.Application.Processors;
    using Domain.Entities;
    using Repositories;

    public class UpdateTodoItemCommand : CommandBase<TodoItemEntity>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public bool Done { get; set; }
    }

    public class UpdateTodoItemCommandCommandCommandCommandHandler
        : CommandCommandHandler<UpdateTodoItemCommand, TodoItemEntity>
    {
        private readonly IMapper _mapper;
        private readonly ITodoItemReadRepository _todoItemReadRepository;
        private readonly ITodoItemWriteRepository _todoItemWriteRepository;

        public UpdateTodoItemCommandCommandCommandCommandHandler(
            ITodoItemReadRepository todoItemReadRepository,
            ITodoItemWriteRepository todoItemWriteRepository,
            IMapper mapper)
        {
            _todoItemReadRepository = todoItemReadRepository;
            _todoItemWriteRepository = todoItemWriteRepository;
            _mapper = mapper;
        }

        public override async Task<TodoItemEntity> Handle(
            UpdateTodoItemCommand request,
            CancellationToken cancellationToken)
        {
            var entity = await _todoItemReadRepository.Find(request.Id);

            if (entity == null) throw new NotFoundException(nameof(TodoItemEntity), request.Id);

            entity.Title = request.Title;

            if (request.Done)
                entity.IsCompleted = true;

            await _todoItemWriteRepository.Update(entity);

            //return _mapper.Map<TodoItemDto>(entity);
            return entity;
        }
    }

    // public class UserPreProcessor :
    //     RequestPreProcessorBase<UpdateTodoItemCommand>
    // {
    //     public override Task Process(
    //         UpdateTodoItemCommand request, 
    //         CancellationToken cancellationToken)
    //     {
    //         Console.WriteLine($"Pre handle {request.Title} {request.Done} with ID {request.Id}");
    //         return Task.CompletedTask;
    //     }
    // }

    public class UserPostProcessor : RequestPostProcessorBase<UpdateTodoItemCommand, TodoItemEntity>
    {
        private readonly ITodoItemWriteRepository _todoItemWriteRepository;

        public UserPostProcessor(ITodoItemWriteRepository todoItemWriteRepository)
        {
            _todoItemWriteRepository = todoItemWriteRepository;
        }

        public override async Task Process(
            UpdateTodoItemCommand request,
            TodoItemEntity response,
            CancellationToken cancellationToken)
        {
            if (response.IsCompleted)
            {
                Console.WriteLine("TodoCompletedEvent emitted");
                response.AddEvent(new TodoCompletedEvent(response));
            }

            await _todoItemWriteRepository.Commit(cancellationToken);
        }
    }
}