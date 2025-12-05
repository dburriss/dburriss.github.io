#!/bin/bash
set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT="$SCRIPT_DIR/src/SiteRenderer/SiteRenderer.fsproj"
OUTPUT_DIR="$SCRIPT_DIR/_site"

# Parse arguments
CONFIGURATION="Release"

while [[ $# -gt 0 ]]; do
    case $1 in
        --debug|-d)
            CONFIGURATION="Debug"
            shift
            ;;
        *)
            echo "Unknown option: $1"
            echo "Usage: $0 [--debug|-d]"
            exit 1
            ;;
    esac
done

# Run the site renderer to generate the site
echo "Generating site..."
dotnet run --project "$PROJECT" -c "$CONFIGURATION" --no-build -- --source "$SCRIPT_DIR" --output "$OUTPUT_DIR"

echo "Site generated at $OUTPUT_DIR"