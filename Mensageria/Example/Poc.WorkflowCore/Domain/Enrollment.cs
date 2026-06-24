using CSharpFunctionalExtensions;
using Poc.WorkflowCore.Domain.Orchestration.Aggreates;

namespace Poc.WorkflowCore.Domain;

public class Enrollment
{
    private Enrollment() { }

    private Enrollment(Guid id, Client client, DateTime requestDate, EState state)
    {
        Id = id;
        Client = client;
        RequestDate = requestDate;
        State = state;
        
    }

    public Guid Id { get; private set; }
    public Client Client { get; private set; } = null!;
    public DateTime RequestDate { get; private set; }
    public EState State { get; private set; }
    public DateTime? SuspensionStartDate { get; private set; }
    public DateTime? SuspensionEndDate { get; private set; }
    

    public static Result<Enrollment> Create(
        string name,
        string email,
        string phone,
        string document,
        DateTime birthDate,
        string gender,
        string address)
    {
        var client = new Client(document, name, email, phone, address);
        return Create(client);
    }
    public static Result<Enrollment> Create(Client client)
    {
        var enrollment = new Enrollment(Guid.NewGuid(), client, DateTime.UtcNow, EState.Suspended);
        return enrollment;
    }

}
