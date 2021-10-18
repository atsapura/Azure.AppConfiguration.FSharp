namespace Azure.AppConfiguration.FSharp.Samples

module SampleConfig =

    type InfrastructureConfig =
        {
            BlobConnection: string
            ServiceBusConnection: string
        }

    type SeoConfig =
        {
            BaseImageUrl: string
        }

    type Config =
        {
            Infrastructure: InfrastructureConfig
            IsProduction: bool
            Seo: SeoConfig
        }
