namespace ENCAPSULAMENTO;

public class Session
{
    public Session(string code, string description)
    {
        Code = code;
        Description = description;
    }

    public string Code { get;}
    public string Description { get; private set; }

    public void UpdateDescription(string description)
    { 
        Description = description; 
        /*Codigo*/
    }
        
            
    
}
