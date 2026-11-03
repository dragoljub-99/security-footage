using SecurityFootage.Domain.Entities;

namespace SecurityFootage.Domain.Interfaces
{
    public interface IEventQueueService
    {
        void Enqueue(EventQueue eventQueue);
    }
}