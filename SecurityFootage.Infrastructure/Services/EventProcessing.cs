using SecurityFootage.Domain.Entities;
using SecurityFootage.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SecurityFootage.Domain;

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

        public async Task AddAsync(EventQueue eventQueue, CancellationToken ct)
        {
            _dbContext.EventQueues.Add(eventQueue);
            await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<EventQueue?> GetEventAsync(CancellationToken ct)
        {

            return await _dbContext.EventQueues.OrderBy(e => e.CreatedAtUtc)
                                               .FirstOrDefaultAsync(ct);
        }

        public async Task<EventResult?> ProcessEvent(CancellationToken ct)
        {
            RequestResult result = RequestResult.FootageNotFound;
            bool isCleared = false;
            string? sasUrl = null;

            var nextEvent = await GetEventAsync(ct);

            if (nextEvent != null)
            {
                var footageInfo = await _dbContext.FootagesInfo.FirstOrDefaultAsync(fi => fi.LocationId == nextEvent.LocationId);

                if (footageInfo != null)
                {
                    result = RequestResult.SasGenerationFailed;

                    try
                    {
                        sasUrl = await _sasUrlClient.GetSasUrlAsync(footageInfo.BlobName, ct);

                        if (sasUrl != null)
                        {
                            if (!string.IsNullOrWhiteSpace(sasUrl))
                            {
                                result = RequestResult.Success;

                                if (await ClearEvent(nextEvent.Id, ct))
                                {
                                    isCleared = true;
                                }
                            }
                        }


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Sas Api is not responding: {ex.Message}");
                    }

                }
            }



            return new EventResult(result, sasUrl, isCleared);

        }

        public async Task<bool> ClearEvent(long id, CancellationToken ct)
        {
            var currentEvent = await _dbContext.EventQueues.FindAsync(id, ct);

            if (currentEvent != null)
            {
                _dbContext.EventQueues.Remove(currentEvent);
                await _dbContext.SaveChangesAsync(ct);
                return true;
            }

            return false;

        }


    }


    public record EventResult(RequestResult Result, string? SasUrl, bool IsCleared);

}