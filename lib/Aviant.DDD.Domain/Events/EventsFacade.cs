namespace Aviant.DDD.Domain.Events
{
    using System;
    using System.Collections.Generic;
    using Exceptions;
    using Services;

    public static class EventsFacade //TODO: Revisit
    {
        [ThreadStatic]
        private static IHaveEvents _mockContainer;

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
        public static void SetEventsContainer(IHaveEvents mockContainer)
        {
            if (_fromTesting == false)
                throw new DomainException(
                    @"For SetNotificationsContainer to work properly SetTestingEnvironment() should be called first. 
                                      This method should be used only for testing purpose");
            _mockContainer = mockContainer;
        }

        private static IHaveEvents GetContainer()
        {
            if (_fromTesting)
                return _mockContainer;

            return ServiceLocator.ServiceContainer.GetService<IHaveEvents>(typeof(IHaveEvents));
        }

        public static void AddEvent(EventBase @event)
        {
            var container = GetContainer();
            container.AddEvent(@event);
        }

        public static List<EventBase> GetAll()
        {
            return GetContainer().GetAll();
        }

        public static bool HasEvents()
        {
            return GetContainer().HasEvents();
        }
    }
}