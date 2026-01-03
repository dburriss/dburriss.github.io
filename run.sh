#!/bin/bash
set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT="$SCRIPT_DIR/src/SiteRenderer/SiteRenderer.fsproj"
OUTPUT_DIR="$SCRIPT_DIR/_site"

# Parse arguments
SERVE=false
WATCH=false
CONFIGURATION="Release"
PORT=8080

while [[ $# -gt 0 ]]; do
    case $1 in
        --serve|-s)
            SERVE=true
            shift
            ;;
        --watch|-w)
            WATCH=true
            shift
            ;;
        --debug|-d)
            CONFIGURATION="Debug"
            shift
            ;;
        --port|-p)
            PORT="$2"
            shift 2
            ;;
        *)
            echo "Unknown option: $1"
            echo "Usage: $0 [--serve|-s] [--watch|-w] [--debug|-d] [--port|-p PORT]"
            exit 1
            ;;
    esac
done

if [ "$WATCH" = true ]; then
    echo "Starting SiteRenderer in watch mode..."
    dotnet watch --project "$PROJECT" run -- --source "$SCRIPT_DIR" --output "$OUTPUT_DIR"
    exit 0
fi

# Build the site renderer
echo "Building SiteRenderer ($CONFIGURATION)..."
dotnet build "$PROJECT" -c "$CONFIGURATION"

# Run the site renderer to generate the site
echo "Generating site..."
dotnet run --project "$PROJECT" -c "$CONFIGURATION" --no-build -- --source "$SCRIPT_DIR" --output "$OUTPUT_DIR"

echo "Site generated at $OUTPUT_DIR"

# Optionally serve the generated site
if [ "$SERVE" = true ]; then
    echo "Serving site at http://localhost:$PORT..."
    dotnet serve -o -d "$OUTPUT_DIR" -p "$PORT"
fi
