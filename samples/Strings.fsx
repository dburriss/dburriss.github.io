let toUpper (s:string) = s.ToUpper()
let toLower (s:string) = s.ToLower()
let replace (oldValue:string) (newValue:string) (s:string) = s.Replace(oldValue,newValue)

module StringBuilder =
    open System.Text
    let init() = new StringBuilder()
    let initWith(s:string) = new StringBuilder(s)
    let append (s:string) (sb:StringBuilder) = sb.Append(s)