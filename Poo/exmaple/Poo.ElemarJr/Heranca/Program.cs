using Heranca;

Student student = new(
name: "Júlio Castro Correia",
cpf: "123.456.789-00",
dateOfBirth: new DateOnly(2006, 5, 15),
addressStreet: "Rua das Flores",
addressCity: "Rua das Figueiras",
addressNumber: "123",
addressPostalCode: "12345-678",
addressNeighborhood: "Centro",
addressComplement: "Apto 101",
age: 17
);
SoccerClass soccerClass = new("01F", "Turma de Futebol A", 17);
if (!soccerClass.CanRegister(student))
    throw new Exception("Aluno não pode realizar inscrição na turma.");
