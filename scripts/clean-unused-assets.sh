#!/bin/bash

# clean-unused-assets.sh
#
# DESCRIPTION:
#   Removes unused legacy assets (Bootstrap, jQuery, Clean Blog theme files)
#   that are no longer used by the SiteRenderer.
#
# USAGE:
#   ./scripts/clean-unused-assets.sh
#
# FILES REMOVED:
#   - css/bootstrap*
#   - css/clean-blog*
#   - css/mixins.less
#   - css/variables.less
#   - js/bootstrap*
#   - js/clean-blog*
#   - js/jquery*
#   - fonts/glyphicons*

set -e

# Ensure we are in the project root (where the script is called from normally)
# or resolve relative to the script location.
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(dirname "$SCRIPT_DIR")"

echo "Cleaning unused assets from $ROOT_DIR..."

# CSS
rm -vf "$ROOT_DIR"/css/bootstrap*
rm -vf "$ROOT_DIR"/css/clean-blog*
rm -vf "$ROOT_DIR"/css/mixins.less
rm -vf "$ROOT_DIR"/css/variables.less

# JS
rm -vf "$ROOT_DIR"/js/bootstrap*
rm -vf "$ROOT_DIR"/js/clean-blog*
rm -vf "$ROOT_DIR"/js/jquery*

# Fonts
rm -vf "$ROOT_DIR"/fonts/glyphicons*

echo "Cleanup complete."
