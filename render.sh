#!/bin/bash

# render.sh - Generates the site content
#
# DESCRIPTION:
#   This script runs the F# SiteRenderer to generate the static site content
#   to the _site directory and runs validation tests by default.
#
# USAGE:
#   ./render.sh [OPTIONS]
#
# OPTIONS:
#   -d, --debug         Run in Debug configuration (default: Release)
#   --skip-tests        Skip validation tests after generation
#
# EXAMPLES:
#   ./render.sh              # Generate site and run tests
#   ./render.sh --debug      # Generate site in Debug mode and run tests
#   ./render.sh --skip-tests # Generate site without running tests

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT="$SCRIPT_DIR/src/SiteRenderer/SiteRenderer.fsproj"
OUTPUT_DIR="$SCRIPT_DIR/_site"

# Parse arguments
CONFIGURATION="Release"
SKIP_TESTS=false

while [[ $# -gt 0 ]]; do
    case $1 in
        --debug|-d)
            CONFIGURATION="Debug"
            shift
            ;;
        --skip-tests)
            SKIP_TESTS=true
            shift
            ;;
        *)
            echo "Unknown option: $1"
            echo "Usage: $0 [--debug|-d] [--skip-tests]"
            exit 1
            ;;
    esac
done

# Run the site renderer to generate the site
echo "Generating site..."
dotnet run --project "$PROJECT" -c "$CONFIGURATION" --no-build -- --source "$SCRIPT_DIR" --output "$OUTPUT_DIR"

echo "Building search index..."
bun run scripts/build-search-index.ts

echo "Site generated at $OUTPUT_DIR"

# Run tests by default (unless skipped)
if [[ "$SKIP_TESTS" == "false" ]]; then
    echo ""
    echo "Running validation tests..."
    echo "=========================="
    
    echo "Running unit tests..."
    dotnet test src/SiteRenderer.Tests/ || {
        echo "❌ Unit tests failed"
        exit 1
    }
    
    echo "Running site validation..."
    dotnet fsi scripts/validate-site.fsx || {
        echo "❌ Site validation failed"
        exit 1
    }
    
    echo "✅ All tests passed!"
fi
