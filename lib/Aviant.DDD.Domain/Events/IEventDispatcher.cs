namespace Aviant.DDD.Domain.Events
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Notifications;

    public interface IEventDispatcher
    {
        #region Pre Commit Events

        List<INotification> GetPreCommitEvents();

        void AddPreCommitEvent(INotification evnt);

        void RemovePreCommitEvent(INotification evnt);

        Task FirePreCommitEvents();

        #endregion

        #region Post Commit Events

        List<INotification> GetPostCommitEvents();

        void AddPostCommitEvent(INotification evnt);

        void RemovePostCommitEvent(INotification evnt);

        Task FirePostCommitEvents();

        #endregion
    }
}