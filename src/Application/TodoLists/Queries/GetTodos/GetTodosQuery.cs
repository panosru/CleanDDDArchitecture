namespace CleanDDDArchitecture.Application.TodoLists.Queries.GetTodos
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Aviant.DDD.Application.Commands;
    using Aviant.DDD.Domain.Enums;
    using Microsoft.EntityFrameworkCore;
    using Persistence;

    public class GetTodosQuery : CommandBase<TodosVm>
    {
    }

    public class GetTodosQueryCommandCommandHandler : CommandCommandHandler<GetTodosQuery, TodosVm>
    {
        private readonly IApplicationDbContextReadOnly _context;
        private readonly IMapper _mapper;

        public GetTodosQueryCommandCommandHandler(IApplicationDbContextReadOnly context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public override async Task<TodosVm> Handle(GetTodosQuery request, CancellationToken cancellationToken)
        {
            return new TodosVm
            {
                PriorityLevels = Enum.GetValues(typeof(PriorityLevel))
                    .Cast<PriorityLevel>()
                    .Select(p => new PriorityLevelDto {Value = (int) p, Name = p.ToString()})
                    .ToList(),

                Lists = await _context.TodoLists
                    .ProjectTo<TodoListDto>(_mapper.ConfigurationProvider)
                    .OrderBy(t => t.Title)
                    .ToListAsync(cancellationToken)
            };
        }
    }
}