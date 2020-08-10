namespace Aviant.DDD.Domain.Events
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IEventDispatcher
    {
        #region Pre Commit Events

        List<IEvent> GetPreCommitEvents();

        void AddPreCommitEvent(IEvent @event);

        void RemovePreCommitEvent(IEvent @event);

        Task FirePreCommitEvents();

        #endregion

        #region Post Commit Events

        List<IEvent> GetPostCommitEvents();

        void AddPostCommitEvent(IEvent @event);

        void RemovePostCommitEvent(IEvent @event);

        Task FirePostCommitEvents();

        #endregion
    }
}