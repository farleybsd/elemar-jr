namespace Heranca;

public abstract class ClassConfiguration //Classe base. Não pode ser instanciada
{
    protected ClassConfiguration(string code, string description)
    {
        Code = code;
        Description = description;
    }
    public string Code { get;}
    public string Description { get;}

    public abstract bool CanRegister(Student student);  //Requer que as classes derivadas
                                                        // implementem esse método
    protected virtual bool IsOpen() => true;         //Permite que esse método seja
                                                    //sobrescrito pelas classes derivadas
}
