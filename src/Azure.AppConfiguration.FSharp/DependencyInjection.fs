namespace Azure.AppConfiguration.FSharp

open System
open Azure.Identity
open Microsoft.Azure.Functions.Extensions.DependencyInjection
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection

module DependencyInjection =

    type IFunctionsConfigurationBuilder with
        member builder.AddAppConfigurationProperly<'TConfig>(connectionStringName: string) =
            builder.ConfigurationBuilder.AddAzureAppConfiguration(fun options ->
                options
                    .Connect(Environment.GetEnvironmentVariable(connectionStringName)) |> ignore)
                    .AddEnvironmentVariables()
            |> ignore
            ()

        member builder.AddAppConfigurationWithKeyVault<'TConfig>(connectionStringName: string) =
            builder.ConfigurationBuilder.AddAzureAppConfiguration(fun options ->
                options
                    .Connect(Environment.GetEnvironmentVariable(connectionStringName))
                    .ConfigureKeyVault(fun kv -> kv.SetCredential(DefaultAzureCredential()) |> ignore) |> ignore)
                    .AddEnvironmentVariables()
            |> ignore
            ()


    type IFunctionsHostBuilder with
        member this.RegisterConfig<'T when 'T: not struct>() =
            this.Services.AddSingleton<'T>(
                fun provider ->
                    let config = provider.GetService<IConfiguration>()
                    JsonConfig.bind<'T> config
                    ) |> ignore
            ()
