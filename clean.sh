#!/bin/bash

# clean.sh - Clean generated site content and build artifacts
#
# DESCRIPTION:
#   Wrapper script that calls the F# clean-site.fsx script to remove all
#   generated content from _site directory while preserving the .git submodule,
#   and runs dotnet clean on the solution.
#
# USAGE:
#   ./clean.sh [OPTIONS]
#
# OPTIONS:
#   --dry-run    Show what would be deleted without actually deleting
#
# EXAMPLES:
#   ./clean.sh           # Clean all generated content
#   ./clean.sh --dry-run # Preview what would be deleted

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Parse arguments
DRY_RUN="false"

while [[ $# -gt 0 ]]; do
    case $1 in
        --dry-run)
            DRY_RUN="true"
            shift
            ;;
        *)
            echo "Unknown option: $1"
            echo "Usage: $0 [--dry-run]"
            exit 1
            ;;
    esac
done

# Call the F# script with the dry-run parameter
dotnet fsi "$SCRIPT_DIR/scripts/clean-site.fsx" "$DRY_RUN"