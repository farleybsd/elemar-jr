namespace ENCAPSULAMENTO
{
    namespace ENCAPSULAMENTO
    {
        public class Student
        {
            public Student(
                string name, string cpf, DateOnly dateOfBirth, string addressStreet, string addressNumber,
                string addressComplement, string addressNeighborhood, string addressPostalCode, string addressCity)
            {
                Name = !string.IsNullOrWhiteSpace(name)
                    ? name
                    : throw new ArgumentException("Name is required.", nameof(name));

                Cpf = IsCpf(cpf)
                    ? cpf
                    : throw new ArgumentException("Invalid CPF.", nameof(cpf));

                DateOfBirth = dateOfBirth;

                AddressStreet = !string.IsNullOrWhiteSpace(addressStreet)
                    ? addressStreet
                    : throw new ArgumentException("Address street is required.", nameof(addressStreet));

                AddressNumber = !string.IsNullOrWhiteSpace(addressNumber)
                    ? addressNumber
                    : throw new ArgumentException("Address number is required.", nameof(addressNumber));

                AddressComplement = addressComplement ?? string.Empty;

                AddressNeighborhood = !string.IsNullOrWhiteSpace(addressNeighborhood)
                    ? addressNeighborhood
                    : throw new ArgumentException("Address neighborhood is required.", nameof(addressNeighborhood));

                AddressPostalCode = !string.IsNullOrWhiteSpace(addressPostalCode)
                    ? addressPostalCode
                    : throw new ArgumentException("Address postal code is required.", nameof(addressPostalCode));

                AddressCity = !string.IsNullOrWhiteSpace(addressCity)
                    ? addressCity
                    : throw new ArgumentException("Address city is required.", nameof(addressCity));
            }

            public string Name { get; }
            public string Cpf { get; }
            public DateOnly DateOfBirth { get; }
            public string AddressStreet { get; private set; }
            public string AddressNumber { get; private set; }
            public string AddressComplement { get; private set; }
            public string AddressNeighborhood { get; private set; }
            public string AddressPostalCode { get; private set; }
            public string AddressCity { get; private set; }

            public bool IsCpf(string cpf)
            {
                // Validação real do CPF aqui
                return !string.IsNullOrWhiteSpace(cpf);
            }

            public void ChangeAddress(
                string addressStreet, string addressNumber, string addressComplement, string addressNeighborhood, string addressPostalCode, string addressCity)
            {
                /*Codigo*/
            }
        }
    }
}


