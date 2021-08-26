﻿namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Export
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Aviant.DDD.Application.Queries;
    using Aviant.DDD.Application.Services;
    using Microsoft.EntityFrameworkCore;
    using Todo.Application.Persistence;

    internal sealed class ExportTodosQuery : Query<ExportTodosVm>
    {
        public ExportTodosQuery(int listId) => ListId = listId;

        private int ListId { get; }

        #region Nested type: ExportTodosQueryHandler

        internal sealed class ExportTodosQueryHandler : QueryHandler<ExportTodosQuery, ExportTodosVm>
        {
            private readonly ITodoDbContextWrite _context;

            private readonly ICsvFileBuilder<TodoItemRecord> _fileBuilder;

            private readonly IMapper _mapper;

            public ExportTodosQueryHandler(
                ITodoDbContextWrite             context,
                IMapper                         mapper,
                ICsvFileBuilder<TodoItemRecord> fileBuilder)
            {
                _context     = context;
                _mapper      = mapper;
                _fileBuilder = fileBuilder;
            }

            public override async Task<ExportTodosVm> Handle(
                ExportTodosQuery  request,
                CancellationToken cancellationToken)
            {
                var vm = new ExportTodosVm();

                List<TodoItemRecord> records = await _context.TodoItems
                   .Where(t => t.ListId == request.ListId)
                   .ProjectTo<TodoItemRecord>(_mapper.ConfigurationProvider)
                   .ToListAsync(cancellationToken)
                   .ConfigureAwait(false);

                vm.Content     = _fileBuilder.BuildTodoItemsFile(records);
                vm.ContentType = "text/csv";
                vm.FileName    = "TodoItems.csv";

                return await Task.FromResult(vm).ConfigureAwait(false);
            }
        }

        #endregion
    }
}