namespace Azure.AppConfiguration.FSharp.Samples

open System
open Microsoft.Azure.Functions.Extensions.DependencyInjection
open Microsoft.Azure.WebJobs.Host.Config
open Microsoft.Extensions.DependencyInjection
open Newtonsoft.Json
open Azure.AppConfiguration.FSharp
open DependencyInjection
open SampleConfig
open Azure.Identity

type Startup() =
    inherit FunctionsStartup()
    do JsonConvert.DefaultSettings <- Func<_>(fun () -> Serialization.DefaultCamelCaseSettings)

    override _.Configure(builder : IFunctionsHostBuilder) =
        builder.Services.AddSingleton<IExtensionConfigProvider, InjectConfiguration>() |> ignore
        builder.RegisterConfig<Config>()
        ()

    override _.ConfigureAppConfiguration(builder: IFunctionsConfigurationBuilder) =
        builder.AddAppConfigurationWithKeyVault<Config>("AppConfigConnection", DefaultAzureCredential())

module DummyModuleOnWhichToAttachAssemblyAttribute =
    [<assembly: FunctionsStartup(typeof<Startup>)>]
    do ()
