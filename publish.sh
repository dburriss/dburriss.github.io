#!/bin/bash
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

# Ensure we're on master branch for the commit
# Submodule may be in detached HEAD state, so we need to checkout or create master
git checkout master 2>/dev/null || git checkout -B master origin/master 2>/dev/null || git checkout -b master

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
    git push origin master -f

    echo "Site published successfully!"
fi
