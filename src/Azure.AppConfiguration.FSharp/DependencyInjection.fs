namespace Azure.AppConfiguration.FSharp

open System
open Azure.Identity
open Microsoft.Azure.Functions.Extensions.DependencyInjection
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Newtonsoft.Json

module DependencyInjection =

    type IFunctionsConfigurationBuilder with

        /// <summary>
        /// Adds Azure App Configuration via standard extension
        /// and then puts those config values into Environment Variables
        /// </summary>
        /// <param name="connectionStringName">Name of Environment Variable with connection string to App Configuration</param>
        member builder.AddAppConfigurationProperly<'TConfig>(connectionStringName: string) =
            builder.ConfigurationBuilder.AddAzureAppConfiguration(fun options ->
                options
                    .Connect(Environment.GetEnvironmentVariable(connectionStringName)) |> ignore)
                    .AddEnvironmentVariables()
            |> ignore
            ()

        /// <summary>
        /// Adds Azure App Configuration via standard extension,
        /// then configures KeyVault
        /// and then puts those config values into Environment Variables
        /// </summary>
        /// <param name="connectionStringName">Name of Environment Variable with connection string to App Configuration</param>
        /// <param name="keyVaultCredential">Credentials to access KeyVault in case App Configuration has references to it</param>
        member builder.AddAppConfigurationWithKeyVault<'TConfig>(connectionStringName: string, keyVaultCredential) =
            builder.ConfigurationBuilder.AddAzureAppConfiguration(fun options ->
                options
                    .Connect(Environment.GetEnvironmentVariable(connectionStringName))
                    .ConfigureKeyVault(fun kv -> kv.SetCredential(keyVaultCredential) |> ignore) |> ignore)
                    .AddEnvironmentVariables()
            |> ignore
            ()

    type IFunctionsHostBuilder with
        ///<summary>
        /// Resolves `IConfiguration`, rebuilds json from it and deserializes it to `T` type
        ///</summary>
        member this.RegisterConfig<'T when 'T: not struct>() =
            this.Services.AddSingleton<'T>(
                fun provider ->
                    let config = provider.GetService<IConfiguration>()
                    JsonConfig.bind<'T> (JsonConvert.DefaultSettings.Invoke()) config
                    ) |> ignore
            ()
