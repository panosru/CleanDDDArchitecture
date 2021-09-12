namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.GetAll
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Aviant.DDD.Application.Queries;
    using Aviant.DDD.Core.Configuration;
    using Dtos;
    using Microsoft.EntityFrameworkCore;
    using Todo.Application.Persistence;

    internal sealed class GetTodosQuery : Query<TodosVm>
    {
        #region Nested type: GetTodosQueryHandler

        internal sealed class GetTodosQueryHandler : QueryHandler<GetTodosQuery, TodosVm>
        {
            private readonly ITodoDbContextRead _context;

            private readonly IMapper _mapper;

            public GetTodosQueryHandler(ITodoDbContextRead context, IMapper mapper)
            {
                _context = context;
                _mapper  = mapper;
            }

            public override async Task<TodosVm> Handle(GetTodosQuery request, CancellationToken cancellationToken)
            {
                return new TodosVm
                {
                    PriorityLevels = Enum.GetValues(typeof(PriorityLevel))
                       .Cast<PriorityLevel>()
                       .Select(p => new PriorityLevelDto { Value = (int)p, Name = p.ToString() })
                       .ToList(),

                    Lists = await _context.TodoLists
                       .ProjectTo<TodoListDto>(_mapper.ConfigurationProvider)
                       .OrderBy(t => t.Title)
                       .ToListAsync(cancellationToken)
                       .ConfigureAwait(false)
                };
            }
        }

        #endregion
    }
}