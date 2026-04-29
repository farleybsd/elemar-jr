using Microsoft.EntityFrameworkCore;
using PicPayManager.Crosscutting.Database;
using PicPayManager.Crosscutting.Database.Users;
using PicPayManager.Crosscutting.Database.Users.Interfaces;

namespace PicPayManager.Crosscutting.Extensions;

public static class SqlServerServiceCollectionExtensions
{
    public static IServiceCollection AddSqlServerDb(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Picpay");

        services.AddDbContextPool<PicPaySimplificadoContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlServerOptionsAction
            =>
            {
                sqlServerOptionsAction.EnableRetryOnFailure(
                maxRetryCount: 5, // Número máximo de tentativas em caso de falhas transitórias (ex: timeout, perda de conexão)
                maxRetryDelay: TimeSpan.FromSeconds(1),  // Tempo máximo de espera entre as tentativas de retry
                errorNumbersToAdd: Enumerable.Empty<int>());   // Lista de erros SQL adicionais que devem ser tratados como transitórios (vazio = usa padrão do EF)
            });
        });
        return services;
    }

    public static IServiceCollection AddRepository(this IServiceCollection services)
    {
        services.AddScoped<IUserRepositoryRead, UserRepositoryRead>();
        services.AddScoped<IUserRepositoryWrite, UserRepositoryWrite>();
        return services;
    }
}