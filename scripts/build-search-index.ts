import { Document, Charset } from "flexsearch";
import { readFile, writeFile } from "node:fs/promises";
import { createHash } from "node:crypto";
import { getIndexConfig } from "../js/search-index-config.js";

const indexConfig = getIndexConfig(Charset);

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
  for (const k of keys) {
      // @ts-ignore
      out[k] = obj[k];
  }
  return { keys, out };
}

async function main() {
  const docsPath = "./_site/search/docs.json";
  const indexOutPath = "./_site/search/index.json";
  const manifestOutPath = "./_site/search/manifest.json";

  console.log(`Reading docs from ${docsPath}...`);
  let docsText = "";
  try {
      docsText = await readFile(docsPath, "utf8");
  } catch (e) {
      console.error(`Error reading ${docsPath}. Make sure the site is built first.`);
      throw e;
  }
  
  const docs: SearchDoc[] = JSON.parse(docsText);
  console.log(`Loaded ${docs.length} documents.`);

  // Deterministic input order: sort by URL
  docs.sort((a, b) => (a.url < b.url ? -1 : a.url > b.url ? 1 : 0));

  console.log("Building index...");
  // @ts-ignore
  const index = new Document(indexConfig);
  for (const d of docs) {
    index.add({ id: d.id, title: d.title, body: d.body });
  }

  // Export all key/data chunks
  console.log("Exporting index...");
  const dump: Record<string, string> = {};
  await index.export((key, data) => {
    // FlexSearch hands you the chunks; treat key/data as opaque
    dump[key] = data;
  });

  // Deterministic serialization
  const { keys, out } = stableObjectFromKeys(dump);
  const indexText = JSON.stringify(out);

  await writeFile(indexOutPath, indexText, "utf8");
  console.log(`Wrote index to ${indexOutPath}`);

  const manifest = {
    schemaVersion: 1,
    documentCount: docs.length,
    flexsearchVersion: "0.8.212",
    indexOptionsHash: sha256(JSON.stringify(indexConfig)),
    docsHash: sha256(docsText),
    indexHash: sha256(indexText),
    exportKeys: keys
  };

  await writeFile(manifestOutPath, JSON.stringify(manifest), "utf8");
  console.log(`Wrote manifest to ${manifestOutPath}`);
}

main().catch((err) => {
  console.error(err);
  process.exit(1);
});
