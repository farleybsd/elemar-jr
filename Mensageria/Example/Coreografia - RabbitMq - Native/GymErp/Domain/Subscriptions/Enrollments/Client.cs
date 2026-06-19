namespace GymErp.Domain.Subscriptions.Enrollments;

public class Client
{
    public string Cpf { get; }
    public string Name { get;  } 
    public string Email { get;  } 
    public string Phone { get;  }  
    public string Address { get;  }

    public Client() { }

    public Client(string cpf, string name, string email, string phone, string address)
    {
        Cpf = cpf;
        Name = name;
        Email = email;
        Phone = phone;
        Address = address;
    }
}
