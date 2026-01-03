#!/bin/bash

# build.sh - Builds the F# SiteRenderer solution
#
# DESCRIPTION:
#   This script builds the F# SiteRenderer project in the specified configuration.
#
# USAGE:
#   ./build.sh [OPTIONS]
#
# OPTIONS:
#   -d, --debug         Build in Debug configuration (default: Release)
#
# EXAMPLES:
#   ./build.sh          # Build in Release mode
#   ./build.sh --debug  # Build in Debug mode

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT="$SCRIPT_DIR/src/SiteRenderer/SiteRenderer.fsproj"

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

# Build the site renderer
echo "Building SiteRenderer ($CONFIGURATION)..."
dotnet build "$PROJECT" -c "$CONFIGURATION"

echo "Build completed successfully!"