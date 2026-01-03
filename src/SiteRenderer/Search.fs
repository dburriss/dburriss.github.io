namespace SiteRenderer

open System
open System.IO
open System.Text.Json
open System.Text.Encodings.Web
open System.Text.RegularExpressions

module Search =

    let generateDocs (baseUrl: string) (posts: ContentItem list) (outputPath: string) =
        let docs =
            posts
            |> List.map (fun post ->
                let plainText =
                    post.Markdown
                    |> Option.map Parsing.markdownToPlainText
                    |> Option.defaultValue ""

                // Remove newlines and collapse whitespace
                let body =
                    plainText.Replace("\r", " ").Replace("\n", " ")
                    |> fun s -> Regex.Replace(s, @"\s+", " ")
                    |> fun s -> s.Trim()

                // Excerpt should prefer description, then fallback to something else if needed
                let excerpt = post.Meta.Description |> Option.defaultValue ""

                { id = Parsing.combineUrl baseUrl post.PageMeta.Url
                  url = Parsing.combineUrl baseUrl post.PageMeta.Url
                  title = post.PageMeta.Title
                  body = body
                  excerpt =
                    if String.IsNullOrWhiteSpace excerpt then
                        None
                    else
                        Some excerpt
                  date = post.PageMeta.Date |> Option.map (fun d -> d.ToString("yyyy-MM-dd"))
                  keywords = post.Meta.Keywords
                  topics = post.Meta.Topics })
            |> List.sortBy (fun d -> d.url)

        let options = JsonSerializerOptions(WriteIndented = true)
        options.Encoder <- JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        let json = JsonSerializer.Serialize(docs, options)
        File.WriteAllText(outputPath, json)
