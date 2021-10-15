namespace Azure.AppConfiguration.FSharp

open Microsoft.Extensions.Configuration
open Newtonsoft.Json

[<RequireQualifiedAccess>]
module JsonConfig =

    open System.Collections.Generic
    type Pair<'K, 'V> = KeyValuePair<'K, 'V>

    [<RequireQualifiedAccess>]
    module Dict =
        let get k (d: Dictionary<_,_>) =
            match d.TryGetValue k with
            | true, v -> Some v
            | false, _ -> None

        let safeAdd (k, v) d =
            match get k d with
            | None -> d.Add(k, v)
            | Some _ ->
                d.Remove k |> ignore
                d.Add(k, v)

        let (|Value|NestedObject|Absent|) (d,k) =
            match get k d with
            | None -> Absent
            | Some x ->
                match box x with
                | :? string as x -> Value x
                | :? Dictionary<string, obj> as x -> NestedObject x
                | _ -> Absent

    let (|ArrayStr|JustStr|NoString|) s =
        match s with
        | null | "" -> NoString
        | s ->
            if s.StartsWith('[') && s.EndsWith(']') then
                let arr = s.Substring(1, s.Length - 2).Split(',')
                ArrayStr (arr |> Array.map (fun s -> s.Trim()))
            else
                JustStr s

    let private handleKeyValue acc (pair: Pair<string, string>) =
        match pair.Key with
        | null | "" -> acc
        | key ->
            let keys = key.Split(':')
            let last = keys.Length - 1
            let rec loop i dict =
                let key = keys.[i]
                if i = last then
                    match pair.Value with
                    | ArrayStr arr -> Dict.safeAdd (key, box arr) dict
                    | JustStr s -> Dict.safeAdd (key, box s) dict
                    | NoString -> ()
                else
                    match dict, key with
                    | Dict.Absent ->
                        let insideDict = Dictionary<string, obj>()
                        Dict.safeAdd (key, box insideDict) dict
                        loop (i+1) insideDict
                    | Dict.Value v -> ()
                    | Dict.NestedObject n -> loop (i+1) n
            loop 0 acc
            acc

    let rebuildJson (values: Pair<string, string> seq) =
        let acc = Dictionary<string, obj>()
        for value in values do
            handleKeyValue acc value |> ignore
        acc

    let bind<'T> (config: IConfiguration) =
        let values = config.AsEnumerable()
        let json = rebuildJson values
        let jsonStr = JsonConvert.SerializeObject json
        let cfg = JsonConvert.DeserializeObject<'T>(jsonStr)
        cfg
