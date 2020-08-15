﻿namespace CleanDDDArchitecture.Application.TodoItems.Commands.UpdateTodoItem
{
    using AutoMapper;
    using Aviant.DDD.Application.Mappings;
    using Aviant.DDD.Domain.TransferObjects;
    using Domain.Entities;

    public class TodoItemDto : IMapFrom<TodoItemEntity>, IDto
    {
        public int Id { get; set; }

        public int ListId { get; set; }

        public string Title { get; set; }

        public bool IsCompleted { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<TodoItemEntity, TodoItemDto>();
        }
    }
}