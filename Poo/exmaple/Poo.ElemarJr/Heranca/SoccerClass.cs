namespace Heranca;

public class SoccerClass : ClassConfiguration
{
    public int AgeLimit { get;}
    public SoccerClass(string code, string description,int ageLimit)
        :base(code,description)
    {
        
    }
    public override bool CanRegister(Student student) => IsOpen() && student.Age <= AgeLimit;


    override protected bool IsOpen() => false;
    
    
}
