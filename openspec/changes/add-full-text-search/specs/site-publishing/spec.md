## ADDED Requirements
### Requirement: Search index generation integrated into render scripts
The project SHALL integrate the Bun search index generation script into `./render.sh` and `./render.ps1`, running after the dotnet site generation.

#### Scenario: Render scripts produce search artifacts
- **WHEN** a developer runs `./render.sh` or `./render.ps1`
- **THEN** the generated output includes the static search artifacts required by the client search UI

#### Scenario: CI render produces search artifacts
- **WHEN** GitHub Actions builds and deploys the site
- **THEN** the published site includes the static search artifacts required by the client search UI

### Requirement: Bun-based build step (no npm)
The search index build step SHALL use Bun for execution and dependency management and SHALL NOT require invoking `npm`.

#### Scenario: Pipeline does not invoke npm
- **WHEN** the search index is generated in CI
- **THEN** the pipeline uses Bun to run the indexing step
- **AND** it does not invoke `npm`
