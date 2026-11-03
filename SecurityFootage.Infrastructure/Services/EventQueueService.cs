using SecurityFootage.Domain;
using SecurityFootage.Domain.Entities;
using SecurityFootage.Domain.Interfaces;
using SecurityFootage.Infrastructure.Persistence;

namespace SecurityFootage.Infrastructure.Services
{
    public class EventQueueService : IEventQueueService
    {
        private readonly SecurityFootageDbContext _dbContext;

        public EventQueueService(SecurityFootageDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Enqueue(EventQueue eventQueue)
        {
            _dbContext.EventQueues.Add(eventQueue);
        }
    }
}