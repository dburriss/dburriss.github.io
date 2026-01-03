# FlexSearch site search (Bun build + static artifacts)

This document is implementation-oriented guidance for `openspec/changes/add-full-text-search/`.

**Goal**: client-side full-text search for a static site (GitHub Pages), with all indexing done at build time.

**Constraints from the change**
- FlexSearch is the search engine.
- Indexing MUST happen at build time (Bun script), never in the browser.
- No `npm` usage (Bun is allowed).
- Output MUST be deterministic and reproducible (byte-identical rebuild with identical inputs and tool versions).
- Build output artifacts:
  - `/search/docs.json` (renderer-emitted document store)
  - `/search/index.json` (FlexSearch export payload)
  - `/search/manifest.json` (schema + checks)

---

## FlexSearch: what to use and why

FlexSearch offers multiple index types:
- `Index`: flat id → text.
- `Document`: multi-field (title/body/etc) indexing over structured docs.
- `Worker` / persistent DB-backed indexes (NOT a fit here): FlexSearch import/export is **not supported** for persistent indexes or worker indexes, and we need build-time export → browser import.

**Recommendation for this repo**: use `Document`.
- We want at least `title` and `body` as distinct fields.
- We want to boost title matches above body matches.

### Recommended index configuration

Pick one config and keep it identical in:
- the Bun build script (when creating/exporting the index)
- the browser runtime (when creating/importing the index)

A reasonable starting point for English blog posts:
- `tokenize: "forward"` for partial matches while typing
- `encoder: Charset.LatinBalance` (case-insensitive + some phonetic tolerance)
- index `title` at higher `resolution` than `body`

Example:

```js
import { Document, Charset } from "flexsearch";

export const indexConfig = {
  tokenize: "forward",
  encoder: Charset.LatinBalance,
  document: {
    id: "id",
    index: [
      { field: "title", tokenize: "forward", resolution: 9 },
      { field: "body", tokenize: "forward", resolution: 5, minlength: 2 }
    ]
  }
};
```

Notes:
- FlexSearch recommends numeric IDs for memory; we can start with `id = url` for simplicity/determinism, and optimize later if needed.
- `minlength` can reduce index size/noise for body text.

---

## FlexSearch export/import: the key thing to get right

FlexSearch export/import uses a callback-based chunk format:
- `index.export((key, data) => ...)` is called multiple times.
- You MUST store **both** `key` and `data` for every call.
- Later, you MUST call `index.import(key, data)` for every exported key.
- Export/import are async in FlexSearch v0.8, so treat them as `await`-able.

Upstream docs:
- `doc/export-import.md` in the FlexSearch repo.

### Our `index.json` representation

For this project’s static artifact, a simple, inspectable representation is:

```json
{
  "<key>": "<data>",
  "<key2>": "<data2>"
}
```

Where:
- the keys are opaque strings emitted by FlexSearch
- the values are strings (sometimes empty)

To make output deterministic:
- sort keys lexicographically before JSON serialization
- write JSON with a single, consistent format (no timestamps; consistent newline policy)

---

## Bun build script: build and export the index

### Install FlexSearch with Bun

Pin a version (important for reproducibility):

```bash
bun add flexsearch@0.8.2
```

Commit `bun.lockb`.

### Build script sketch

This script reads renderer output (`_site/search/docs.json`), builds the `Document` index, then writes deterministic artifacts:
- `_site/search/index.json`
- `_site/search/manifest.json`

```ts
// scripts/build-search-index.ts
import { Document } from "flexsearch";
import { readFile, writeFile } from "node:fs/promises";
import { createHash } from "node:crypto";
import { indexConfig } from "./search-index-config";

type SearchDoc = {
  id: string; // recommended: canonical URL
  url: string;
  title: string;
  body: string;
  excerpt?: string;
  tags?: string[];
  topics?: string[];
  date?: string; // ISO yyyy-MM-dd
};

function sha256(text: string) {
  return createHash("sha256").update(text, "utf8").digest("hex");
}

function stableObjectFromKeys<T>(obj: Record<string, T>) {
  const keys = Object.keys(obj).sort();
  const out: Record<string, T> = {};
  for (const k of keys) out[k] = obj[k];
  return { keys, out };
}

async function main() {
  const docsPath = "./_site/search/docs.json";
  const indexOutPath = "./_site/search/index.json";
  const manifestOutPath = "./_site/search/manifest.json";

  const docsText = await readFile(docsPath, "utf8");
  const docs: SearchDoc[] = JSON.parse(docsText);

  // Deterministic input order: sort by URL
  docs.sort((a, b) => (a.url < b.url ? -1 : a.url > b.url ? 1 : 0));

  const index = new Document(indexConfig);
  for (const d of docs) {
    index.add({ id: d.id, title: d.title, body: d.body });
  }

  // Export all key/data chunks
  const dump: Record<string, string> = {};
  await index.export((key, data) => {
    // FlexSearch hands you the chunks; treat key/data as opaque
    dump[key] = data;
  });

  // Deterministic serialization
  const { keys, out } = stableObjectFromKeys(dump);
  const indexText = JSON.stringify(out);

  await writeFile(indexOutPath, indexText, "utf8");

  const manifest = {
    schemaVersion: 1,
    documentCount: docs.length,
    flexsearchVersion: "0.8.2",
    indexOptionsHash: sha256(JSON.stringify(indexConfig)),
    docsHash: sha256(docsText),
    indexHash: sha256(indexText),
    exportKeys: keys
  };

  await writeFile(manifestOutPath, JSON.stringify(manifest), "utf8");
}

main().catch((err) => {
  console.error(err);
  process.exit(1);
});
```

Determinism tips:
- Keep `indexConfig` in a single file and import it in both build/runtime.
- Sort `docs` before calling `index.add`.
- Sort export keys before writing `index.json`.
- Avoid pretty-printed JSON unless you lock the exact formatting.

---

## Browser runtime: import-only + query

The client runtime MUST:
- lazy-load `/search/*.json` only on focus
- create the FlexSearch index (same config)
- import the prebuilt export payload
- run queries only (no `add`, `update`, `remove`)

### Import FlexSearch in the browser

FlexSearch provides a production ESM bundle:
- `flexsearch.bundle.module.min.js`

You can vendor it into the repo (preferred for reproducibility) or use a pinned CDN URL.

Per upstream README, module usage looks like:

```html
<script type="module">
  import { Document, Charset, Resolver } from "/assets/flexsearch.bundle.module.min.js";
</script>
```

### Import and query

Example (very small; real UI should debounce input and show loading state):

```js
import { Document } from "/assets/flexsearch.bundle.module.min.js";
import { indexConfig } from "/assets/search-index-config.js";

let loaded;

async function loadSearch() {
  if (loaded) return loaded;
  loaded = (async () => {
    const [manifest, indexDump, docs] = await Promise.all([
      fetch("/search/manifest.json").then(r => r.json()),
      fetch("/search/index.json").then(r => r.json()),
      fetch("/search/docs.json").then(r => r.json())
    ]);

    // Optional sanity checks (non-fatal):
    // - manifest.documentCount === docs.length
    // - hash/indexOptionsHash match expected

    const index = new Document(indexConfig);
    for (const key of Object.keys(indexDump)) {
      await index.import(key, indexDump[key]);
    }

    const docById = new Map(docs.map(d => [d.id, d]));
    return { index, docById };
  })();
  return loaded;
}

export async function querySearch(q) {
  const { index, docById } = await loadSearch();

  // Use Resolver to boost title above body.
  const titleRaw = index.search(q, { resolve: false, pluck: "title", suggest: true });
  const bodyRaw = index.search(q, { resolve: false, pluck: "body", suggest: true });

  const ids = titleRaw.boost(2).or(bodyRaw).resolve({ limit: 20, enrich: false });
  return ids
    .map(id => docById.get(id))
    .filter(Boolean);
}
```

Notes:
- `enrich: false` keeps results as IDs; we then map to `/search/docs.json` for URL/title/excerpt.
- Keeping `docs.json` separate matches the spec requirement and keeps the index payload smaller.

---

## Artifact schemas (recommended)

### `/search/manifest.json`

Minimum required by spec:
- `schemaVersion` (int)
- `documentCount` (int)

Recommended additional fields for validation/debugging:
- `flexsearchVersion` (string)
- `indexOptionsHash` (sha256 of the canonical `indexConfig` JSON)
- `docsHash` (sha256 of raw `docs.json` bytes)
- `indexHash` (sha256 of raw `index.json` bytes)
- `exportKeys` (sorted list of keys present in `index.json`)

### `/search/docs.json`

Minimum required by spec:
- `id` (string)
- `url` (string)
- `title` (string)

Recommended for implementation:
- `body` (string) – the plain text used for indexing
- `excerpt` (string) – optional; can come from front matter or be derived
- `date` (string, ISO yyyy-MM-dd)
- `tags` / `topics` (string arrays)

### `/search/index.json`

- JSON object mapping FlexSearch export `key` → `data` strings.
- Keys should be sorted before writing for deterministic output.

---

## Preparing documents in F# (Markdown → controlled index text)

The spec requires that the renderer produces the canonical doc feed and that the indexer does **not** scrape rendered HTML pages.

This repo already uses Markdig in `src/SiteRenderer/Parsing.fs`.

### Deterministic `id` and ordering

- `id`: use the canonical URL (string) for determinism.
- Stable output ordering: sort all emitted docs by canonical URL before writing JSON.

### Markdown → plain text rules (recommended)

To keep search results relevant and index size reasonable, generate a “controlled” plain-text `body` field.

Rules:
- Strip YAML front matter (already supported by `Parsing.parseMarkdownFile`).
- Exclude fenced code blocks and inline code from the indexed body (code tends to dominate results).
- Keep link text, drop link targets:
  - `[Some Text](https://example.com)` → `Some Text`
- Keep headings text (helpful for relevance), but treat them as normal text.
- Drop raw HTML blocks/inline HTML where possible; keep visible text content only.
- Normalize whitespace:
  - normalize CRLF → LF
  - collapse runs of whitespace to a single space
  - trim leading/trailing whitespace
- Avoid adding any build-time metadata like “generated at”.

### Using Markdig without scraping site HTML

A safe approach is:
1. Parse Markdown source into a Markdig AST (using the same `markdownPipeline` as the renderer).
2. Walk the AST to produce plain text.
3. Skip node types you don’t want indexed (code blocks, HTML blocks).

The important part is that the extraction is deterministic and uses the markdown source as input.

### Suggested `docs.json` shape (F#)

```fsharp
type SearchDoc =
    { id: string
      url: string
      title: string
      body: string
      excerpt: string option
      date: string option
      keywords: string list
      topics: string list }
```

Then:
- create a list of `SearchDoc`
- sort by `url`
- serialize with a deterministic JSON encoder (stable property ordering, stable formatting)

---

## Implementation checklist (for this repo)

- Add deterministic `/search/docs.json` emission in the F# renderer.
- Add Bun script to build `/search/index.json` + `/search/manifest.json` from docs.
- Add client JS that lazy-loads artifacts on focus, imports index, queries only.
- Add a deterministic build check: regenerate index twice and compare hashes/bytes.

---

## References

- FlexSearch README (v0.8): https://github.com/nextapps-de/flexsearch
- Export/Import docs: https://github.com/nextapps-de/flexsearch/blob/master/doc/export-import.md
- Resolver docs (boost/and/or/resolve): https://github.com/nextapps-de/flexsearch/blob/master/doc/resolver.md
