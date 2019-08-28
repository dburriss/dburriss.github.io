let toUpper (s:string) = s.ToUpper()
let toLower (s:string) = s.ToLower()
let replace (oldValue:string) (newValue:string) (s:string) = s.Replace(oldValue,newValue)
let trim (s:string) = s.Trim()


module StringBuilder =
    open System.Text
    let init() = new StringBuilder()
    let initWith(s:string) = new StringBuilder(s)
    let append (s:string) (sb:StringBuilder) = sb.Append(s)


let makeEmail (name) = 
    let sanitizeString (s:string) = s |> Option.ofObj
    name
    |> Option.bind sanitizeString
    |> Option.orElse (Some "info")
    |> Option.map (fun n -> sprintf "%s@acme.com" n)
    |> Option.get

Some "bob" |> makeEmail
Some null |> makeEmail