public async Task<TicketType> GetWithLockAsync(Guid id)
{
    return await context
        .TicketTypes
        .FromSql(
            $@"
            SELECT id, event_id, name, price, currency, quantity
            FROM ticketing.ticket_types
            WHERE id = {id}
            FOR UPDATE NOWAIT") // PostgreSQL: Lock or fail immediately
        .SingleAsync();
}

public async Task<TicketType> GetWithLockAsync(Guid id)
{
    return await context
        .TicketTypes
        .FromSqlInterpolated($@"
            SELECT 
                id,
                event_id,
                name,
                price,
                currency,
                quantity
            FROM ticketing.ticket_types WITH (UPDLOCK, ROWLOCK, NOWAIT)
            WHERE id = {id}") // SqlServer: Lock or fail immediately
        .SingleAsync();
}