public class Apartment
{
    public Guid Id { get; set; }

    [Timestamp]
    public byte[] Version { get; set; }
}

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Apartment>()
        .Property<byte[]>("Version")
        .IsRowVersion();
}

SELECT a.Id, a.Version
FROM Apartments a
WHERE a.Id = @p0

UPDATE Apartments a
SET a.LastBookedOnUtc = @p0
WHERE a.Id = @p1 AND a.Version = @p2;



public Result<Guid> Handle(
    ReserveBooking command,
    AppDbContext dbContext)
{
    var user = dbContext.Users.GetById(command.UserId);
    var apartment = dbContext.Apartments.GetById(command.ApartmentId);
    var (startDate, endDate) = command;

    if (dbContext.Bookings.IsOverlapping(apartment, startDate, endDate))
    {
        return Result.Failure<Guid>(BookingErrors.Overlap);
    }

    try
    {
        var booking = Booking.Reserve(apartment, user, startDate, endDate);

        dbContext.Add(booking);

        dbContext.SaveChanges();

        return booking.Id;
    }
    catch (DbUpdateConcurrencyException) // Aqui
    {
        return Result.Failure<Guid>(BookingErrors.Overlap);
    }
}