import { Document, Charset } from "/js/vendor/flexsearch.bundle.module.min.js";
import { getIndexConfig } from "/js/search-index-config.js";

let loaded;

async function loadSearch() {
  if (loaded) return loaded;
  loaded = (async () => {
    try {
        const [manifest, indexDump, docs] = await Promise.all([
        fetch("/search/manifest.json").then(r => r.json()),
        fetch("/search/index.json").then(r => r.json()),
        fetch("/search/docs.json").then(r => r.json())
        ]);

        const indexConfig = getIndexConfig(Charset);
        const index = new Document(indexConfig);
        for (const key of Object.keys(indexDump)) {
        await index.import(key, indexDump[key]);
        }

        const docById = new Map(docs.map(d => [d.id, d]));
        return { index, docById };
    } catch (e) {
        console.error("Failed to load search index", e);
        throw e;
    }
  })();
  return loaded;
}

export async function querySearch(q) {
  const { index, docById } = await loadSearch();

  const titleRaw = index.search(q, { resolve: false, pluck: "title", suggest: true });
  const bodyRaw = index.search(q, { resolve: false, pluck: "body", suggest: true });

  const ids = titleRaw.boost(2).or(bodyRaw).resolve({ limit: 20, enrich: false });
  return ids
    .map(id => docById.get(id))
    .filter(Boolean);
}

export function isLoaded() {
    return !!loaded;
}

export function preload() {
    loadSearch();
}
