namespace Aviant.DDD.Domain.Notifications
{
    using System;
    using System.Collections.Generic;
    using Services;

    public static class Facade
    {
        [ThreadStatic] private static INotifications _mockContainer;

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
        public static void SetNotificationsContainer(INotifications mockContainer)
        {
            if (_fromTesting == false)
                throw new Exceptions.DomainException(
                    @"For SetNotificationsContainer to work properly SetTestingEnvironment() should be called first. 
                                      This method should be used only for testing purpose");
            _mockContainer = mockContainer;
        }

        private static INotifications GetContainer()
        {
            if (_fromTesting)
                return _mockContainer;
            return ServiceLocator.ServiceContainer?.GetService<INotifications>(typeof(INotifications));
        }

        public static void AddNotification(INotification notification)
        {
            var container = GetContainer();
            container.AddNotification(notification);
        }

        public static List<INotification> GetAll()
        {
            return GetContainer().GetAll();
        }

        public static bool HasNotifications()
        {
            return GetContainer().HasNotifications();
        }
    }
}