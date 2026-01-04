# site-search Specification

## Purpose
TBD - created by archiving change add-full-text-search. Update Purpose after archive.
## Requirements
### Requirement: Build-time full-text search using FlexSearch
The site SHALL provide full-text search over published content using FlexSearch, where document indexing occurs at build time and the browser only loads and queries a prebuilt index.

#### Scenario: Build pipeline produces prebuilt search artifacts
- **WHEN** the site is generated for publication
- **THEN** the output contains a prebuilt FlexSearch index artifact
- **AND** the output contains a document store artifact used to render search results

#### Scenario: Browser does not perform indexing
- **WHEN** a reader loads the site and uses search
- **THEN** the client code only imports/loads the prebuilt index and queries it
- **AND** it does not build an index from raw documents in the browser

### Requirement: GitHub Pages compatible search
Search MUST work on GitHub Pages without any backend services.

#### Scenario: No backend required
- **WHEN** the site is hosted as static files on GitHub Pages
- **THEN** search operates using only static files served from the same origin
- **AND** no server-side APIs are required

### Requirement: Deterministic and reproducible index generation
Index generation MUST be deterministic and reproducible given the same inputs.

#### Scenario: Byte-identical rebuild
- **WHEN** the index is generated twice from identical document inputs and tool versions
- **THEN** the produced index artifacts are byte-identical

### Requirement: Search input documents are produced by the renderer
The renderer SHALL produce the canonical input document set used for indexing.

#### Scenario: Indexing does not scrape HTML
- **WHEN** the index build step runs
- **THEN** it consumes a renderer-produced document feed
- **AND** it does not crawl or parse rendered HTML pages to discover documents

### Requirement: Static search artifact format is documented
The project SHALL emit and document a stable, versioned static search artifact format.

The published output SHALL include the following files:
- `/search/manifest.json`
- `/search/docs.json`
- `/search/index.json`

The manifest (`/search/manifest.json`) SHALL include, at minimum:
- `schemaVersion` (integer)
- `documentCount` (integer)

The document store (`/search/docs.json`) SHALL be a JSON array of documents, each containing at minimum:
- `id` (string)
- `url` (string)
- `title` (string)

The index payload (`/search/index.json`) SHALL contain the data needed to import the prebuilt FlexSearch index in the browser.

#### Scenario: Format is inspectable and versioned
- **WHEN** a developer inspects the generated search artifacts
- **THEN** there is a documented schema with a version identifier
- **AND** the schema describes how to map search results to URLs and titles

#### Scenario: Artifacts are present in published output
- **WHEN** the site is generated for publication
- **THEN** `/search/manifest.json`, `/search/docs.json`, and `/search/index.json` exist in the output

### Requirement: Lazy loading of search artifacts
The client SHALL lazy-load the search index and document store only when the search input is focused to minimize initial page load impact.

#### Scenario: No upfront loading of search data
- **WHEN** a reader loads the homepage
- **THEN** the search artifacts are not fetched or loaded until the search input is focused

#### Scenario: Visual feedback during loading
- **WHEN** a reader focuses the search input
- **THEN** the UI provides visual feedback indicating loading (e.g., greying out a search button) until the artifacts are downloaded and ready

### Requirement: Default result presentation
Search results SHALL include a title and link to the matching content, and SHOULD include an excerpt/snippet when available.

#### Scenario: Reader sees actionable results
- **WHEN** a reader searches for a term that matches at least one document
- **THEN** the UI renders a list of results
- **AND** each result includes a link and the document title

