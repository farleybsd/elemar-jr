using Microsoft.Extensions.Options;

namespace IntelligentServiceFindZipCode.App.Crosscutting.Integrations.ViaCep.Validates;

public class ViaCepSettingsValidate : IValidateOptions<ViaCepSettings>
{
    public ValidateOptionsResult Validate(string? name, ViaCepSettings options)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(options.BaseAddress))
            errors.Add("BaseAddress not found");

        return errors.Count > 0 ?
            ValidateOptionsResult.Fail(string.Join(';', errors))
            : ValidateOptionsResult.Success;
    }
}

