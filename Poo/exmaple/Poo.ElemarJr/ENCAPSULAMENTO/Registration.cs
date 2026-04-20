namespace ENCAPSULAMENTO;

public class Registration
{
    public Registration(string number, string sessionCode, string studentCpf)
    {
        Number = number;
        SessionCode = sessionCode;
        StudentCpf = IsCpf(studentCpf) ? studentCpf : throw new ArgumentException("Invalid CPF", nameof(studentCpf)); // Objeto nao e criado em um estado invalido
        RegisteredIn = DateTime.Now;
    }

    public string Number { get;}
    public string SessionCode { get; private set; } // So Pode ser Alterado pelo método ChangeSession
    public string StudentCpf { get;}
    public DateTime RegisteredIn { get; }
    public bool IsCpf(string cpf) { /*Cpf Validation*/ return true; }

    public void ChangeSession(string sessionCode)
    {
        //O código da turma pode ser alterado internamente
        SessionCode = sessionCode;

    }
}
