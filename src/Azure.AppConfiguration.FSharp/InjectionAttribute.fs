namespace Azure.AppConfiguration.FSharp

open System
open System.Threading.Tasks
open Microsoft.Azure.WebJobs.Description
open Microsoft.Azure.WebJobs.Host.Bindings
open Microsoft.Azure.WebJobs.Host.Config
open Microsoft.Azure.WebJobs.Host.Protocols

[<AutoOpen>]
module Injection =

    [<Binding>]
    [<AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)>]
    type InjectAttribute() =
        inherit Attribute()

    type private InjectValueProvider(value : obj) =
        interface IValueProvider with
            member __.Type = value.GetType()
            member __.GetValueAsync() = Task.FromResult value
            member __.ToInvokeString() = value.ToString()

    type InjectBinding(serviceProvider : IServiceProvider, t : Type) =
        interface IBinding with
            member __.FromAttribute = true

            member __.BindAsync (value : obj, ctx : ValueBindingContext) =
                let valueProvider = InjectValueProvider(value) :> IValueProvider
                Task.FromResult valueProvider

            member this.BindAsync (ctx : BindingContext) =
                let value = serviceProvider.GetService(t)
                (this :> IBinding).BindAsync(value, ctx.ValueContext)

            member __.ToParameterDescriptor () = ParameterDescriptor()

    type InjectBindingProvider(serviceProvider : IServiceProvider) =
        interface IBindingProvider with
            member __.TryCreateAsync (context : BindingProviderContext) =
                let binding = InjectBinding(serviceProvider, context.Parameter.ParameterType) :> IBinding
                Task.FromResult binding

    type InjectConfiguration(sp : IServiceProvider) =
        interface IExtensionConfigProvider with
            member __.Initialize(context : ExtensionConfigContext) =
                context.AddBindingRule<InjectAttribute>().Bind(InjectBindingProvider(sp)) |> ignore
