using SecurityFootage.Domain.Entities;
using SecurityFootage.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace SecurityFootage.Infrastructure.Services
{
    public class EventProcessing
    {
        private readonly SecurityFootageDbContext _dbContext;
        private readonly SasUrlClient _sasUrlClient;

        public EventProcessing(SecurityFootageDbContext dbContext, SasUrlClient sasUrlClient)
        {
            _dbContext = dbContext;
            _sasUrlClient = sasUrlClient;
        }

        public async Task AddAsync(EventQueue eventQueue)
        {
            _dbContext.EventQueues.Add(eventQueue);
            await _dbContext.SaveChangesAsync();
        }
        
        public async Task<EventQueue?> GetEventAsync(CancellationToken ct)
        {
            
               return await _dbContext.EventQueues.OrderBy(e => e.CreatedAtUtc)
                                                  .FirstOrDefaultAsync(ct);
        }

        public async Task<string?> ProcessEvent(CancellationToken ct)
        {
            var nextEvent = await GetEventAsync(ct);
            

            if (nextEvent != null)
            {
                var footageInfo = await _dbContext.FootagesInfo.FirstOrDefaultAsync(fi => fi.LocationId == nextEvent.LocationId);

                if (footageInfo != null)
                {
                    var sasUrl = await _sasUrlClient.GetSasUrlAsync(footageInfo.BlobName, ct);

                    if (sasUrl != null)
                    {
                        if (string.IsNullOrWhiteSpace(sasUrl))
                        {
                            return sasUrl;
                        }
                    }
                }
            }

            return null;
        }
               
       
    }

}