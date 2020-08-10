namespace Aviant.DDD.Domain.Events
{
    using System;
    using System.Collections.Generic;
    using MediatR;
    using Services;

    public class EventsFacade
    {
        [ThreadStatic] private static IEvents _mockContainer;

        private static bool _fromTesting;

        public static void SetTestingEnvironment()
        {
            _fromTesting = true;
        }

        /// <summary>
        ///     This method should be used only for testing purpose
        ///     Under normal use the container is obtained via DI
        /// </summary>
        /// <param name="mockContainer"></param>
        /// <exception cref="Exception"></exception>
        public static void SetEventsContainer(IEvents mockContainer)
        {
            if (_fromTesting == false)
                throw new Exceptions.DomainException(
                    @"For SetNotificationsContainer to work properly SetTestingEnvironment() should be called first. 
                                      This method should be used only for testing purpose");
            _mockContainer = mockContainer;
        }

        private static IEvents GetContainer()
        {
            if (_fromTesting)
                return _mockContainer;
            
            return ServiceLocator.ServiceContainer.GetService<IEvents>(typeof(IEvents));
        }

        public static void AddEvent(IEvent @event)
        {
            var container = GetContainer();
            container.AddEvent(@event);
        }

        public static List<IEvent> GetAll()
        {
            return GetContainer().GetAll();
        }

        public static bool HasEvents()
        {
            return GetContainer().HasEvents();
        }
    }
}