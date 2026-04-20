using System.Collections.Generic;

namespace Generics1;

public class Employee
{ }

public class Manager : Employee
{
    public string Name { get; set; }
    public string Department { get; set; }
}

private void ManagerPrinter(IEnumerable<Employee> employees)
    {
        // Executa o método
    }

    /*PERMITE A UTILIZAÇÃO DE UM TIPO MAIS DERIVADO DO QUE O ORIGINAL*/

    private void ManagerPrinter(IEnumerable<Employee> employees)
    {
        // Executa o método
    }

    private List<Manager> managers = new()
{
    new Manager { Name = "Alice", Department = "HR" },
    new Manager { Name = "Bob", Department = "IT" }
};

    /*
     Em C#, coleções genéricas como List<T> são invariantes, ou seja, não se pode usar uma List<Derivado>
     onde se espera List<Base>, mesmo que haja herança. Isso previne alterações inseguras.
     Covariância só se aplica a coleções de leitura, como IEnumerable<T>.
     */

    private ManagerPrinter(managers);



    // 1. O Método Genérico (Sabe lidar com QUALQUER Employee)
    // Pense nisso como um "Clínico Geral"
    void ProcessarFuncionario(Employee e)
    {
        Console.WriteLine($"Processando: {e.Name}");
    }
    // 2. O Erro Intuitivo vs A Realidade
    // Imagine que uma variável pede uma ação EXCLUSIVA para Gerentes
    Action<Manager> acaoParaGerente;
// CONTRAVARIÂNCIA EM AÇÃO:
// Eu posso atribuir o método que lida com Funcionário (Geral)
// para a variável que espera lidar com Gerente (Específico)
acaoParaGerente = ProcessarFuncionario;
// Por que funciona?
// Quando eu chamar acaoParaGerente(umGerente),
// o método ProcessarFuncionario vai receber esse gerente
// Como Gerente É um Funcionário, o método funciona perfeitamente
acaoParaGerente(new Manager { Name = "Bob" });