using Composicao;

var cpf = Cpf.Parse("123.456.789-00");
var studentName = StudentName.Parse("John Doe");
var studentDateOfBirth = StudentDateOfBirth.Parse(DateOnly.Parse("2000-01-01"));
Address address = ("Rua A", "123", "Centro", "12345-000", "BH", "Apto 1");
Student student = new(studentName, cpf, studentDateOfBirth, address);