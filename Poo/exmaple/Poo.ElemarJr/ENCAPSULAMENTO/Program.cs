using ENCAPSULAMENTO;
using ENCAPSULAMENTO.ENCAPSULAMENTO;

var session = new Session("TURMA-001", "Turma de C# básico");
Console.WriteLine($"Código: {session.Code}");
Console.WriteLine($"Descrição: {session.Description}");

session.UpdateDescription("Turma de C# intermediário");
Console.WriteLine($"Nova descrição: {session.Description}");

var student = new Student(
    name: "Farley",
    cpf: "12345678900",
    dateOfBirth: new DateOnly(1995, 5, 10),
    addressStreet: "Rua A",
    addressNumber: "100",
    addressComplement: "Apto 202",
    addressNeighborhood: "Centro",
    addressPostalCode: "35000-000",
    addressCity: "Governador Valadares");

Console.WriteLine($"Aluno: {student.Name}");
Console.WriteLine($"CPF: {student.Cpf}");
Console.WriteLine($"Cidade: {student.AddressCity}");

student.ChangeAddress(
    addressStreet: "Rua B",
    addressNumber: "200",
    addressComplement: "Casa",
    addressNeighborhood: "São Pedro",
    addressPostalCode: "35010-000",
    addressCity: "Governador Valadares");

Console.WriteLine($"Novo endereço: {student.AddressStreet}, {student.AddressNumber}");

var registration = new Registration(
    number: "MAT-2026-0001",
    sessionCode: session.Code,
    studentCpf: student.Cpf);

Console.WriteLine($"Matrícula: {registration.Number}");
Console.WriteLine($"Turma: {registration.SessionCode}");
Console.WriteLine($"Aluno CPF: {registration.StudentCpf}");
Console.WriteLine($"Data da matrícula: {registration.RegisteredIn}");

registration.ChangeSession("TURMA-002");
Console.WriteLine($"Turma alterada: {registration.SessionCode}");