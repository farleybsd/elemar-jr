using System.Text.Json.Serialization;

namespace IntelligentServiceFindZipCode.App.ZipCode.Endpoints;

public readonly record struct ZipCode([property: JsonPropertyName("cep")] string Cep, [property: JsonPropertyName("logradouro")] string Logradouro,
                                      [property: JsonPropertyName("complemento")] string Complemento, [property: JsonPropertyName("unidade")] string Unidade,
                                      [property: JsonPropertyName("bairro")] string Bairro, [property: JsonPropertyName("localidade")] string Localidade,
                                      [property: JsonPropertyName("uf")] string Uf, [property: JsonPropertyName("estado")] string Estado,
                                      [property: JsonPropertyName("regiao")] string Regiao, [property: JsonPropertyName("ibge")] string Ibge,
                                      [property: JsonPropertyName("gia")] string Gia, [property: JsonPropertyName("ddd")] string Ddd,
                                      [property: JsonPropertyName("siafi")] string Siafi
);