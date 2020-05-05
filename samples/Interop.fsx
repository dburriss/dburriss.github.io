// This is for demonstration purposes only
type CSharpyType() =
    // seq<int>
    let mutable enumerableTProp = Seq.empty
    // seq<obj>
    let mutable enumerableProp = Seq.empty
    // int []
    let mutable arrayTProp = Array.empty
    // obj []
    let mutable arrayProp = Array.empty
    // int list
    let mutable listTProp = ResizeArray()
    // int []
    let mutable ilistTProp  = Array.empty
    // int []
    let mutable icollectionTProp = Array.empty
    // unit -> DateTimeOffset
    let mutable dtFun = fun () -> System.DateTimeOffset.UtcNow
    // Convert between expressions: http://www.fssnip.net/ts/title/F-lambda-to-C-LINQ-Expression
    
    member _.IEnumerableTProp 
        with get() : System.Collections.Generic.IEnumerable<int> = enumerableTProp
        and set(v : System.Collections.Generic.IEnumerable<int>) = enumerableTProp <- v 
    
    member _.IEnumerableProp 
        with get() : System.Collections.IEnumerable = enumerableProp :> System.Collections.IEnumerable
        and set(v : System.Collections.IEnumerable ) = enumerableProp <- v |> Seq.cast

    member _.ArrayTProp 
        with get() : int[] = arrayTProp
        and set(v  : int[]) = arrayTProp <- v 

    member _.ArrayProp   
        with get() : System.Array = arrayProp :> System.Array
        and set(v : System.Array) = arrayProp <- v |> System.Linq.Enumerable.OfType<obj> |> Seq.toArray

    member _.ListTProp 
        with get() : System.Collections.Generic.List<int> = listTProp
        and set(v : System.Collections.Generic.List<int>) = listTProp <- v

    member _.ICollectionTProp 
        with get() : System.Collections.Generic.ICollection<int> = icollectionTProp :> System.Collections.Generic.ICollection<int>
        and set(v : System.Collections.Generic.ICollection<int>) = icollectionTProp <- v |> Seq.toArray

    member _.IListTProp 
        with get() : System.Collections.Generic.IList<int> = ilistTProp :> System.Collections.Generic.IList<int>
        and set(v : System.Collections.Generic.IList<int>) = ilistTProp <- v |> Seq.toArray

    member _.FuncProp 
        with get() : System.Func<System.DateTimeOffset> = System.Func<System.DateTimeOffset> dtFun
        and set(f : System.Func<System.DateTimeOffset>) = dtFun <- fun () -> f.Invoke()

    member _.Print() = do
        printfn "enumerableT: %A" enumerableTProp
        printfn "enumerable: %A" enumerableProp
        printfn "arrayTProp: %A" arrayTProp
        printfn "arrayProp: %A" arrayProp
        printfn "listTProp: %A" listTProp
        printfn "ilistTProp: %A" ilistTProp
        printfn "icollectionTProp: %A" icollectionTProp

// Examples
let csharp = CSharpyType()
do csharp.Print()

// System.Collections.Generic.IEnumerable<_>
// https://github.com/fsharp/fsharp/blob/3bc41f9e10f9abbdc1216e984a98e91aad351cff/src/fsharp/FSharp.Core/prim-types.fs#L3287
// https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1?view=netcore-3.1

// well a reminder that this works
let mutable ss = seq { 1; 2 }
ss <- [1;2]
ss <- [|1;2|]

// open System.Collections.Generic
// ...
// type seq<'T> = IEnumerable<'T>
// type List<'T> = 
//        | ([])  :                  'T list
//        | (::)  : Head: 'T * Tail: 'T list -> 'T list
//        interface System.Collections.Generic.IEnumerable<'T>
//        interface System.Collections.IEnumerable
//        interface System.Collections.Generic.IReadOnlyCollection<'T>
//        interface System.Collections.Generic.IReadOnlyList<'T>

// type ResizeArray<'T> = System.Collections.Generic.List<'T>

// Array is equivalent to System.Array

csharp.IEnumerableTProp <- seq { 0..10 }
csharp.IEnumerableTProp <- [0..10]
csharp.IEnumerableTProp <- [|0..10|]

let it2s : int seq = csharp.IEnumerableTProp
let it2a : int array = csharp.IEnumerableTProp |> Seq.toArray
let it2l : int list = csharp.IEnumerableTProp |> Seq.toList

// System.Collections.IEnumerable
// https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable?view=netcore-3.1
csharp.IEnumerableProp <- seq { 0..10 }
csharp.IEnumerableProp <- [0..10]
csharp.IEnumerableProp <- [|0..10|]

for i in (csharp.IEnumerableProp) do 
    printfn "i: %A" i

// int []
csharp.ArrayTProp <- [|0..10|]
//csharp.ArrayTProp <- seq {0..10} // Compile error: This expression was expected to have type 'int []' but here has type 'seq<int>'
let i2s : int seq = csharp.IEnumerableProp |> Seq.cast
let i2a : int array = csharp.IEnumerableProp |> Seq.cast |> Seq.toArray
let i2l : int list = csharp.IEnumerableProp |> Seq.cast |> Seq.toList


// System.Array
// https://docs.microsoft.com/en-us/dotnet/api/system.array?view=netcore-3.1
csharp.ArrayProp <- [|0..10|]

// System.Collections.Generic.List<_>
// https://github.com/fsharp/fsharp/blob/3bc41f9e10f9abbdc1216e984a98e91aad351cff/src/fsharp/FSharp.Core/prim-types.fs#L3129
// type ResizeArray<'T> = System.Collections.Generic.List<'T>
// https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1?view=netcore-3.1

csharp.ListTProp <- [0..10] |> ResizeArray
csharp.ListTProp <- [|0..10|] |> ResizeArray
csharp.ListTProp <- seq { 0..10 } |> ResizeArray
csharp.IEnumerableTProp <- csharp.ListTProp

// System.Collections.Generic.ICollection<_>
// https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.icollection-1?view=netcore-3.1
// https://docs.microsoft.com/en-us/dotnet/api/system.collections.icollection?view=netcore-3.1

csharp.ICollectionTProp <- [|0..10|]
csharp.ICollectionTProp <- [|0..10|] |> ResizeArray

let ic2seq : int seq = csharp.ICollectionTProp :> seq<_>
let ic2arr : int array = csharp.ICollectionTProp |> Seq.toArray
let collect = ic2seq :?> System.Collections.Generic.ICollection<int>
printfn "icollect %A" collect

// System.Collections.Generic.IList<_>
// https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ilist-1?view=netcore-3.1
csharp.IListTProp <- [|0..10|]
csharp.IListTProp <- [|0..10|] |> ResizeArray

let ils : int seq = csharp.IListTProp :> seq<_>
let ila : int array = csharp.IListTProp |> Seq.toArray
// ResizeArray
let resizeArr = [0..10] |> ResizeArray
let arr = resizeArr.ToArray()
let xs = resizeArr :> seq<_>
let lst = xs |> Seq.toList
let lstCollect = resizeArr :> System.Collections.Generic.ICollection<int>
let ra2ilist = resizeArr :> System.Collections.Generic.IList<int>

// System.Func<_>
csharp.FuncProp <- (fun () -> System.DateTimeOffset.UnixEpoch)
let f = fun () -> csharp.FuncProp.Invoke()
do csharp.Print()