using SINGLERESPONSIBILITYPRINCIPLE_SRP_.ResultPatern;
using static System.Collections.Specialized.BitVector32;

namespace SINGLERESPONSIBILITYPRINCIPLE_SRP_.Bad;

public class RegistrationService
{
    public Result<Registration> Register(Session session)
    {
        /* code */
    }
    public IEnumerable<Session> Validate(Session session, Student student)
    {
        /* code */
        
    }
    public Result Cancel(Session session, Student student)
    {
        /* code */
    }
}
