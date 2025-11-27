#!/bin/bash
set -e

# Build the site renderer
echo "Building SiteRenderer..."
dotnet build ./src/SiteRenderer/SiteRenderer.fsproj -c Release

# Run the site renderer to generate the site
echo "Generating site..."
dotnet run --project ./src/SiteRenderer/SiteRenderer.fsproj -c Release -- --source . --output _site

# Serve the generated site
echo "Serving site at http://localhost:8080..."
dotnet serve -o -d ./_site/
