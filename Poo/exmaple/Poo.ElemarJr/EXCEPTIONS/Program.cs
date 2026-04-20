/*
 C# NATIVAMENTE TRATA FALHAS À PARTIR DE EXCEÇÕES
É A FORMA QUE AS BIBLIOTECAS NATIVAS DO C# TRATAM
FALHAS
TRATAR FALHAS COM EXCEPTIONS É MAIS FAMILIAR PARA
DESENVOLVEDORES C# DO QUE ABORDAGENS FUNCIONAIS
 */

// Modo 1
/*
 * Re-lança a mesma exceção
 * Preserva o stack trace original
 * Não cria novo objeto
 */
using EXCEPTIONS;

try
{
    // Code
}
catch (Exception ex)
{
    // Log
    throw;
}

// Modo 2
/*
 * Cria um novo objeto de exceção
 * A original vira InnerException
 * Perde o stack trace original na exceção principal
 */
try
{
    // Code
}
catch (Exception ex)
{
    // Log
    throw new Exception("New exception.", ex);
}


// Obs : Siga a diretiva CA2201!

/* Argumento inválido : ArgumentNullException
 * Fora do intervalo permitido de valores: ArgumentOutOfRangeException
 * Valor do enum inválido: InvalidEnumArgumentException
 * Contém um formato que não atende às especificações: FormatException
 * É inválido: ArgumentException
 */

StudentCannotRegisterInTheClassException.ThrowIf(
    student.IsBlocked,
    "Aluno está bloqueado para matrícula.",
    student.Name,
    class.Code
);


public void RegisterStudent(Student student)
{
    StudentCannotRegisterInTheClassException.ThrowIf(
        student == null,
        "Aluno inválido.",
        "N/A",
        Code
    );

    StudentCannotRegisterInTheClassException.ThrowIf(
        student.IsBlocked,
        "Aluno está bloqueado.",
        student.Name,
        Code
    );

    StudentCannotRegisterInTheClassException.ThrowIf(
        IsFull,
        "Turma já está cheia.",
        student.Name,
        Code
    );

    // fluxo normal
    _students.Add(student);
}


try
{
    classroom.RegisterStudent(student);
}
catch (StudentCannotRegisterInTheClassException ex)
{
    Console.WriteLine($"Erro: {ex.Message}");
    Console.WriteLine($"Aluno: {ex.StudentName}");
    Console.WriteLine($"Turma: {ex.ClassCode}");
}