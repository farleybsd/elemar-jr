namespace POLIMORFISMO;

public class ValidateRegistrationService
{
    public IEnumerable<ClassConfiguration> Validate()
    {
        Student student = new(
                name: "Júlio Castro Correia",
                cpf: "84298736431",
                dateOfBirth: new DateOnly(1996, 11, 11),
                addressStreet: "Rua das Figueiras",
                addressNumber: "28",
                addressNeighborhood: "Floresta",
                addressPostalCode: "93700-000",
                addressCity: "Campo Bom",
                age: 17,
                addressComplement: "Casa"
        );

        IEnumerable<ClassConfiguration> configurations =new ClasseConfigurationDataAccess();

        foreach ( ClassConfiguration configuration in configurations )
        {
            if (!configuration.CanRegister(student)) //O objeto instanciado é do tipo de uma das classes derivadas
            {
                continue;
            }
            yield return configuration;
        }
    }
}