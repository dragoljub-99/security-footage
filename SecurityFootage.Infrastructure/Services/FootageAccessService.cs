using Microsoft.EntityFrameworkCore;
using SecurityFootage.Infrastructure.Persistence;
using SecurityFootage.Domain.Entities;
using SecurityFootage.Domain;
using SecurityFootage.Domain.Interfaces;
using SecurityFootage.Domain.Results;
using SecurityFootage.Infrastructure.Services;

namespace SecurityFootageExercise.Infrastructure.Services
{
    public class FootageAccessService : IFootageAccessService
    {
        private readonly SecurityFootageDbContext _dbContext;
        private readonly IAuditLogService _logsServices;
        private readonly EventProcessing _eventProcessingService;

        public FootageAccessService(SecurityFootageDbContext dbContext, IAuditLogService logsServices,
                                    EventProcessing eventProcessing)
        {
            _dbContext = dbContext;
            _logsServices = logsServices;
            _eventProcessingService = eventProcessing;
        }

        public async Task<CreateUserResult> CreateUserAsync(User createUser, string nickName, CancellationToken ct)
        {
            DateTime now = DateTime.UtcNow;
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Nickname == nickName, ct);

            if (user == null)
            {
                var log = new AuditLog
                {
                    Action = AuditAction.CREATEUSER,
                    Result = AuditOutcome.USERNOTFOUND,
                    Nickname = nickName,
                    CreatedAtUtc = now
                };
                _logsServices.AddAudit(log);
                await _dbContext.SaveChangesAsync(ct);
                return new CreateUserResult(RequestResult.InvalidUser, null);
            }

            var isUserExist = await _dbContext.Users.AnyAsync(u => u.Nickname == createUser.Nickname, ct);

            if (isUserExist)
            {
                var log = new AuditLog
                {
                    Action = AuditAction.CREATEUSER,
                    Result = AuditOutcome.CONFLICT,
                    Nickname = nickName,
                    CreatedAtUtc = now
                };
                _logsServices.AddAudit(log);
                await _dbContext.SaveChangesAsync(ct);
                return new CreateUserResult(RequestResult.Conflict, null);
            }
            var newUser = new User
            {
                Nickname = createUser.Nickname,
                CreatedAtUtc = now
            };

            _dbContext.Users.Add(newUser);

            _logsServices.AddAudit(new AuditLog
            {
                UserId = user.Id,
                Action = AuditAction.CREATEUSER,
                Result = AuditOutcome.SUCCEEDED,
                Nickname = nickName,
                CreatedAtUtc = now
            });

            await _dbContext.SaveChangesAsync(ct);

            return new CreateUserResult(RequestResult.Success, newUser);
        }

        public async Task<FootageAccessResponse> RequestFootageAccessAsync(Location location, string nickName, CancellationToken ct)
        {
            DateTime now = DateTime.UtcNow;

            Location? savedLocation = null;
            string? sasUrl = null;

            RequestResult result = RequestResult.InvalidUser;
            AuditOutcome outcome = AuditOutcome.USERNOTFOUND;

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Nickname == nickName, ct);

            if (user != null)
            {
                result = RequestResult.InvalidAccess;
                outcome = AuditOutcome.ACCESSDENIED;

                savedLocation = await _dbContext.Locations.FirstOrDefaultAsync(l =>
                                                            l.Country == location.Country &&
                                                            l.City == location.City, ct);

                if (savedLocation == null)
                {
                    outcome = AuditOutcome.LOCATIONNOTFOUND;
                }
                else
                {
                    bool hasAccess = await _dbContext.UserAccesses.AnyAsync(ua =>
                                               ua.UserId == user.Id &&
                                               ua.LocationId == savedLocation.Id, ct);

                    if (hasAccess)
                    {

                        await _eventProcessingService.AddAsync(new EventQueue
                        {
                            UserId = user.Id,
                            Nickname = nickName,
                            LocationId = savedLocation.Id,
                            CreatedAtUtc = now,
                            Status = QueueProcessingStatus.Pending
                        }, ct);

                        var eventResult = await _eventProcessingService.ProcessEvent(ct);

                        if (eventResult != null)
                        {
                            if (eventResult.Result == RequestResult.FootageNotFound)
                            {
                                outcome = AuditOutcome.FOOTAGENOTFOUND;
                                result = RequestResult.FootageNotFound;
                            }
                            else if (eventResult.Result == RequestResult.SasGenerationFailed)
                            {
                                outcome = AuditOutcome.SASGENERATIONFAILED;
                                result = RequestResult.SasGenerationFailed;
                            }
                            else if (eventResult.Result == RequestResult.Success && eventResult.IsCleared)
                            {
                                result = RequestResult.Success;
                                outcome = AuditOutcome.SUCCEEDED;

                                sasUrl = eventResult.SasUrl;
                            }
                        }


                    }
                }
            }

            _logsServices.AddAudit(new AuditLog
            {
                UserId = user?.Id,
                LocationId = savedLocation?.Id,
                RequestedCountry = location.Country,
                RequestedCity = location.City,
                Action = AuditAction.REQUEST_FOOTAGE,
                Result = outcome,
                Nickname = user?.Nickname,
                CreatedAtUtc = now
            });

            await _dbContext.SaveChangesAsync(ct);

            return new FootageAccessResponse(result, sasUrl);
        }

        public async Task<User?> GetUserByNicknameAsync(string nickName, CancellationToken ct)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Nickname == nickName, ct);
        }


    }
}