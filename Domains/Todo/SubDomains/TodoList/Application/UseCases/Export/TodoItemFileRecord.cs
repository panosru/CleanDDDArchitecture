﻿// ReSharper disable UnusedAutoPropertyAccessor.Global

#pragma warning disable 8618

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Export
{
    using Aviant.DDD.Application.Mappings;
    using Todo.Core.Entities;

    public sealed class TodoItemRecord : IMapFrom<TodoItemEntity>
    {
        public string Title { get; set; }

        public bool Done { get; set; }
    }
}