using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Composicao;

public struct Address
{
    private readonly string _Street;
    private readonly string _Number;
    private readonly string _Neighborhood;
    private readonly string _PostalCode;
    private readonly string _City;
    private readonly string _Complement;
    public Address(
        string street, string number, string neighborhood,
        string postalCode, string city, string complement = "")
    {
        _Street = street;
        _Number = number;
        _Neighborhood = neighborhood;
        _PostalCode = postalCode;
        _City = city;
        _Complement = complement;
    }

    public static Address Parse(string street, string number, string neighborhood,
                               string postalCode, string city, string complement)
    => (TryParse(street, number, neighborhood, postalCode, city, complement, out var result))
    ? result
    : throw new ArgumentException("Error ao criar o endereço");

    public static bool TryParse(string street, string number, string neighborhood,
        string postalCode, string city, string complement, out Address address)
    {
        if (!IsValid(street, number, neighborhood, postalCode, city))
        {
            address = default;
            return false;
        }
        address = new Address(street, number, neighborhood, postalCode, city, complement);
        return true;

    }

    public static implicit operator Address(
     (string street, string number, string neighborhood, string postalCode, string city, string complement) value)
     => Parse(value.street, value.number, value.neighborhood, value.postalCode, value.city, value.complement);

    public override string ToString()
    {
        return $"{_Street}, {_Number}, {_Neighborhood}, {_PostalCode}, {_City} - {_Complement}";
    }

    public bool IsEmpty => IsValid(_Street, _Number, _Neighborhood, _PostalCode, _City);

    public static bool IsValid(string street, string number, string neighborhood,
                               string postalCode, string city)
    {
        return !string.IsNullOrWhiteSpace(street) &&
               !string.IsNullOrWhiteSpace(number) &&
               !string.IsNullOrWhiteSpace(neighborhood) &&
               !string.IsNullOrWhiteSpace(postalCode) &&
               !string.IsNullOrWhiteSpace(city);
    }

}
