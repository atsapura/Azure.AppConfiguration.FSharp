namespace Azure.AppConfiguration.FSharp.Samples
open Microsoft.Azure.WebJobs
open Microsoft.Extensions.Logging
open SampleConfig
open Azure.AppConfiguration.FSharp

module Functions =

    [<FunctionName("test-config")>]
    let testConfig
        ([<TimerTrigger("*/10 * * * * *")>]timer: TimerInfo)
        (logger: ILogger)
        ([<Inject>]config: Config) =
        let a = config.Seo
        logger.LogInformation($"Hey! Here's your config!\n%A{config}")
