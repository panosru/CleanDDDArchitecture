﻿namespace CleanDDDArchitecture.Infrastructure.Persistence
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Application.Common.Interfaces;
    using Aviant.DDD.Application;
    using Aviant.DDD.Application.Identity;
    using Aviant.DDD.Domain.Entity;
    using Aviant.DDD.Infrastructure;
    using Aviant.DDD.Infrastructure.Persistance;
    using Domain.Entities;
    using IdentityServer4.EntityFramework.Options;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using ApplicationRole = Identity.ApplicationRole;
    using ApplicationUser = Identity.ApplicationUser;

    public class ApplicationDbContext :
        ApplicationDbContextBase<ApplicationDbContext, ApplicationUser, ApplicationRole>, IApplicationDbContext
    {

        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions,
            ICurrentUserService currentUserService,
            IMediator mediator,
            IDateTime dateTime) : base(options, operationalStoreOptions, currentUserService,
                mediator, dateTime)
        {
        }

        public DbSet<TodoList> TodoLists { get; set; }

        public DbSet<TodoItem> TodoItems { get; set; }
    }
}