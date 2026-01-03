#!/bin/bash

# publish.sh - Publishes the site to GitHub Pages
#
# DESCRIPTION:
#   This script commits the contents of _site (a git submodule) and force-pushes
#   to the master branch. It can be run locally for testing or called by CI.
#
# USAGE:
#   ./publish.sh [OPTIONS]
#
# OPTIONS:
#   -n, --dry-run       Show what would be committed without making changes
#   -m, --message MSG   Custom commit message (default: "Deploy site [skip ci]")
#
# EXAMPLES:
#   ./publish.sh
#   ./publish.sh --dry-run
#   ./publish.sh --message "New blog post"

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SITE_DIR="$SCRIPT_DIR/_site"

# Parse arguments
DRY_RUN=false
MESSAGE="Deploy site [skip ci]"

while [[ $# -gt 0 ]]; do
    case $1 in
        --dry-run|-n)
            DRY_RUN=true
            shift
            ;;
        --message|-m)
            MESSAGE="$2"
            shift 2
            ;;
        *)
            echo "Unknown option: $1"
            echo "Usage: $0 [--dry-run|-n] [--message|-m MESSAGE]"
            exit 1
            ;;
    esac
done

# Verify _site directory exists
if [ ! -d "$SITE_DIR" ]; then
    echo "Error: _site directory not found. Run ./run.sh first to generate the site."
    exit 1
fi

cd "$SITE_DIR"

# Check if _site has a .git (submodule pointer)
# The renderer wipes _site, so we may need to reinitialize the submodule
if [ ! -e ".git" ]; then
    echo "Reinitializing _site submodule..."
    cd "$SCRIPT_DIR"
    git submodule update --init _site
    cd "$SITE_DIR"
fi

# Verify _site is a git repository (submodule)
if ! git rev-parse --git-dir > /dev/null 2>&1; then
    echo "Error: _site is not a git repository. Run 'git submodule update --init' first."
    exit 1
fi

# Configure git user if running in CI
if [ -n "$GITHUB_ACTIONS" ]; then
    git config user.name "${GITHUB_ACTOR:-github-actions}"
    git config user.email "${GITHUB_ACTOR:-github-actions}@users.noreply.github.com"
    
    # Configure authentication for pushing to external repo
    # Requires DEPLOY_TOKEN secret with repo scope for dburriss/dburriss.github.io
    if [ -n "$DEPLOY_TOKEN" ]; then
        git remote set-url origin "https://x-access-token:${DEPLOY_TOKEN}@github.com/dburriss/dburriss.github.io.git"
    fi
fi


if [ "$DRY_RUN" = true ]; then
    echo "=== DRY RUN ==="
    echo "Would commit the following changes:"
    git status --short
    echo ""
    echo "Commit message: $MESSAGE"
    echo "Would force-push to origin/master"
else
    echo "Staging changes..."
    git add .

    # Check if there are changes to commit
    if git diff --cached --quiet; then
        echo "No changes to publish."
        exit 0
    fi

    echo "Committing changes..."
    git commit -m "$MESSAGE"

    echo "Pushing to master branch..."
    git push origin HEAD:master -f

    echo "Site published successfully!"
fi
