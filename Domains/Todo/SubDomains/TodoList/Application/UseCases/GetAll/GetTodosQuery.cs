using AutoMapper;
using AutoMapper.QueryableExtensions;
using Aviant.Application.Queries;
using Aviant.Core.Configuration;
using CleanDDDArchitecture.Domains.Todo.Application.Persistence;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.GetAll.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.GetAll;

internal sealed record GetTodosQuery : Query<TodosVm>
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
