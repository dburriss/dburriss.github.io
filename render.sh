#!/bin/bash

# render.sh - Generates the site content
#
# DESCRIPTION:
#   This script runs the F# SiteRenderer to generate the static site content
#   to the _site directory.
#
# USAGE:
#   ./render.sh [OPTIONS]
#
# OPTIONS:
#   -d, --debug         Run in Debug configuration (default: Release)
#   --test             Run wiki link validation tests after generation
#   --test-only        Only run tests, skip site generation
#
# EXAMPLES:
#   ./render.sh         # Generate site in Release mode
#   ./render.sh --debug # Generate site in Debug mode
#   ./render.sh --test  # Generate site and run validation tests

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT="$SCRIPT_DIR/src/SiteRenderer/SiteRenderer.fsproj"
OUTPUT_DIR="$SCRIPT_DIR/_site"

# Parse arguments
CONFIGURATION="Release"
RUN_TESTS=false
TEST_ONLY=false

while [[ $# -gt 0 ]]; do
    case $1 in
        --debug|-d)
            CONFIGURATION="Debug"
            shift
            ;;
        --test)
            RUN_TESTS=true
            shift
            ;;
        --test-only)
            TEST_ONLY=true
            shift
            ;;
        *)
            echo "Unknown option: $1"
            echo "Usage: $0 [--debug|-d] [--test] [--test-only]"
            exit 1
            ;;
    esac
done

# Run tests only if requested
if [[ "$TEST_ONLY" == "true" ]]; then
    echo "Running wiki link tests only..."
    echo "================================"
    
    echo "Running unit tests..."
    dotnet fsi test-wiki-links.fsx || {
        echo "❌ Unit tests failed"
        exit 1
    }
    
    echo "Running site validation tests..."
    dotnet fsi test-site-validation.fsx || {
        echo "❌ Site validation tests failed"
        exit 1
    }
    
    echo "✅ All wiki link tests passed!"
    exit 0
fi

# Run the site renderer to generate the site
echo "Generating site..."
dotnet run --project "$PROJECT" -c "$CONFIGURATION" --no-build -- --source "$SCRIPT_DIR" --output "$OUTPUT_DIR"

echo "Building search index..."
bun run scripts/build-search-index.ts

echo "Site generated at $OUTPUT_DIR"

# Run tests if requested
if [[ "$RUN_TESTS" == "true" ]]; then
    echo ""
    echo "Running wiki link validation tests..."
    echo "====================================="
    
    dotnet fsi test-site-validation.fsx || {
        echo "❌ Wiki link validation failed"
        exit 1
    }
    
    echo "✅ Wiki link validation passed!"
fi
