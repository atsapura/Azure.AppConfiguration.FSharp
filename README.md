# Azure.AppConfiguration.FSharp

## Summary

This project improves App Configuration experience in Azure Function Apps, which is a bit far from "It just works".
Apart from that it allows you to use statically typed configuration instead of the common `IConfiguration` approach.

## Motivation

While `IConfiguration` certainly can get the job done, it kinda has dynamic nature, meaning that when accessing values one at a time
you don't know if the entire configuration is complete. This leaves the possibility that your function app will start, do some job, and then fail in the middle because 1 of 10 config values is missing. It would be better to know that on startup, fix it right away and get on with your life.
Besides, it's a stringly-typed approach, meaning that misprints can happen and there's no autocomplete. Sometimes that leads to two different keys pointing to the same value.

Statically typed config, especially with F# records, solves those problems. With `Inject` attribute you can just wire your config using DI.

### Okay, but why not just use `.Get<T>` extension method which does the same thing?

Because that method implementation is a couple of centuries behind. While, e.g. `Newtonsoft.Json` can easily deal with immutable types with no public default constructor available (Both F# records and Unions), this extension method can't do any of those things. It can't handle arrays, it can't handle options, let alone some custom union types. Which means you've got to do in-place checks for nulls, empty strings and stuff like that, which is still has that dynamic touch to it.
Also you can't configure serialization settings for that.

## How to use

1. Define your config type.
2. Register that type in DI container in Startup.fs using `.RegisterConfig<'T>` extension method
3. Add Azure App Configuration using either `.AddAppConfigurationProperly` method or `.AddAppConfigurationWithKeyVault` if you keep some of the settings in KeyVault
4. Use `[<Inject>]` attribute to inject that config in your Azure Function.
5. ??????
6. PROFIT

See [samples](https://github.com/atsapura/Azure.AppConfiguration.FSharp/tree/main/samples/Azure.AppConfiguration.FSharp.Samples) for more.