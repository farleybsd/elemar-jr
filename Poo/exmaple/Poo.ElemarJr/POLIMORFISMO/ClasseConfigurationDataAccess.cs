using System.Collections;

namespace POLIMORFISMO;

public class ClasseConfigurationDataAccess : IEnumerable<ClassConfiguration>
{
    public IEnumerator<ClassConfiguration> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
