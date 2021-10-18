namespace Azure.AppConfiguration.FSharp.Samples

open Newtonsoft.Json
open Microsoft.FSharpLu.Json.Compact

module Serialization =

    let DefaultCamelCaseSettings =
        let settings = CamelCaseNoFormatting.CompactCamelCaseNoFormattingSettings.settings
        settings.MissingMemberHandling <- MissingMemberHandling.Ignore
        settings.NullValueHandling <- NullValueHandling.Include
        settings
