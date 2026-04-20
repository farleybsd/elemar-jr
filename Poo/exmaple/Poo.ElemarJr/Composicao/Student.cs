namespace Composicao;

public class Student
{
    public Student(StudentName name, Cpf cpf, StudentDateOfBirth dateOfBirth, Address addressStreet)
    {
        Name = name;
        Cpf = cpf;
        DateOfBirth = dateOfBirth;
        Address = addressStreet;
        ;
    }

    public StudentName Name { get; }
    public Cpf Cpf { get; }
    public StudentDateOfBirth DateOfBirth { get; }
    public Address Address { get; }

    public int Age => CalculateAge(DateOfBirth);

    private int CalculateAge(StudentDateOfBirth dateOfBirth)
    { /* Age calculation */ return 0; }
}

/*
 * SÃO TIPOS COMO STRING, LONG, DOUBLE, INT, BOOL, ETC
   DADOS EM UM LOCAL E COMPORTAMENTO EM OUTRO
   NO ENCAPSULAMENTO VIMOS QUE OS DADOS E O
   COMPORTAMENTO NÃO DEVEM ESTAR SEPARADOS, MAS
   COMBINADOS EM OBJETOS
   A SOLUÇÃO PARA ESSE PROBLEMA RESIDE NA ADOÇÃO DE
   PRÁTICAS DE OO

   
  CONTINUA SENDO UMA STRUCT COM TODOS OS SEUS BENEFÍCIOS
  BÔNUS DO SUGAR SINTAX PROVIDO PELO RECORD
  ADDRESS É UM EXEMPLO DO QUE COMUMENTE CHAMAMOS DE VALUE OBJECT
 */