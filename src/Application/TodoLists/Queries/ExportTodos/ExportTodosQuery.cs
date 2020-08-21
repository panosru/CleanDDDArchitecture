namespace CleanDDDArchitecture.Application.TodoLists.Queries.ExportTodos
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Aviant.DDD.Application.Queries;
    using Aviant.DDD.Application.Services;
    using Microsoft.EntityFrameworkCore;
    using Persistence;

    public class ExportTodosQuery : Query<ExportTodosVm>
    {
        public int ListId { get; set; }
    }

    public class ExportTodosQueryHandler : QueryHandler<ExportTodosQuery, ExportTodosVm>
    {
        private readonly ITodoDbContext _context;
        private readonly ICsvFileBuilder<TodoItemRecord> _fileBuilder;
        private readonly IMapper _mapper;

        public ExportTodosQueryHandler(
            ITodoDbContext context,
            IMapper mapper,
            ICsvFileBuilder<TodoItemRecord> fileBuilder)
        {
            _context = context;
            _mapper = mapper;
            _fileBuilder = fileBuilder;
        }

        public override async Task<ExportTodosVm> Handle(ExportTodosQuery request, CancellationToken cancellationToken)
        {
            var vm = new ExportTodosVm();

            var records = await _context.TodoItems
                .Where(t => t.ListId == request.ListId)
                .ProjectTo<TodoItemRecord>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            vm.Content = _fileBuilder.BuildTodoItemsFile(records);
            vm.ContentType = "text/csv";
            vm.FileName = "TodoItems.csv";

            return await Task.FromResult(vm);
        }
    }
}