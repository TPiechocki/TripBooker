﻿using Microsoft.EntityFrameworkCore;
using TripBooker.PaymentService.Infrastructure;

namespace TripBooker.PaymentService.Repositories;


internal interface ITimeoutTimestampRepository
{
    Task AddNewAsync(Guid paymentId, CancellationToken cancellationToken);

    Task<IEnumerable<TimeoutTimestamp>> QueryAll(CancellationToken cancellationToken);

    void Remove(TimeoutTimestamp timeoutTimestamp); 
}

internal class TimeoutTimestampRepository : ITimeoutTimestampRepository
{
    private readonly PaymentDbContext _dbContext;

    public TimeoutTimestampRepository(PaymentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddNewAsync(Guid paymentId, CancellationToken cancellationToken)
    {
        await _dbContext.TimeoutTimestamp.AddAsync(new TimeoutTimestamp(paymentId), cancellationToken);
    }

    public async Task<IEnumerable<TimeoutTimestamp>> QueryAll(CancellationToken cancellationToken)
    {
        return await _dbContext.TimeoutTimestamp.Select(x => x).ToListAsync(cancellationToken);
    }

    public void Remove(TimeoutTimestamp timeoutTimestamp)
    {
        _dbContext.TimeoutTimestamp.Remove(timeoutTimestamp);
    }
}
