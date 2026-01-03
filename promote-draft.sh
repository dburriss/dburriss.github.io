#!/bin/bash

# promote-draft.sh - Interactive script to promote draft posts to publication
#
# DESCRIPTION:
#   This script lists available drafts in the _drafts/ directory and allows
#   you to select one for promotion. The selected draft will be moved to
#   _posts/ with a proper date prefix and its front matter will be updated
#   to set published: true.
#
# USAGE:
#   ./promote-draft.sh [OPTIONS]
#
# OPTIONS:
#   -d, --date DATE     Custom publish date (YYYY-MM-DD format)
#   -h, --help          Show help message
#
# EXAMPLES:
#   ./promote-draft.sh                    # Promote with today's date
#   ./promote-draft.sh -d 2025-12-25      # Promote with custom date

set -e

# Default values
PUBLISH_DATE=""
HELP=false

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to display help
show_help() {
    cat << EOF
${BLUE}promote-draft.sh${NC} - Promote draft posts to publication

${YELLOW}USAGE:${NC}
    ./promote-draft.sh [OPTIONS]

${YELLOW}OPTIONS:${NC}
    -d, --date DATE     Custom publish date (YYYY-MM-DD format)
    -h, --help          Show this help message

${YELLOW}DESCRIPTION:${NC}
    This script lists available drafts in the _drafts/ directory and allows
    you to select one for promotion. The selected draft will be moved to
    _posts/ with a proper date prefix and its front matter will be updated
    to set published: true.

${YELLOW}EXAMPLES:${NC}
    ./promote-draft.sh                    # Promote with today's date
    ./promote-draft.sh -d 2025-12-25      # Promote with custom date

EOF
}

# Function to print colored output
print_error() {
    echo -e "${RED}ERROR: $1${NC}" >&2
}

print_success() {
    echo -e "${GREEN}SUCCESS: $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}WARNING: $1${NC}"
}

print_info() {
    echo -e "${BLUE}INFO: $1${NC}"
}

# Check front matter has non-empty flow sequence for given key
has_non_empty_flow_seq() {
    local file="$1"
    local key="$2"
    local line=$(sed -n '/^---/,/^---/p' "$file" | grep -E "^${key}:")
    if [[ -z "$line" ]]; then
        return 1
    fi
    # Must contain [ ... ] with at least one value
    if echo "$line" | grep -Eq "\[[^]]+\]"; then
        return 0
    else
        return 1
    fi
}

# Function to validate date format
validate_date() {
    local date="$1"
    if [[ ! "$date" =~ ^[0-9]{4}-[0-9]{2}-[0-9]{2}$ ]]; then
        return 1
    fi
    # Check if date is valid
    if ! date -d "$date" >/dev/null 2>&1; then
        return 1
    fi
    return 0
}

# Function to check if file has valid front matter
has_front_matter() {
    local file="$1"
    if [[ ! -f "$file" ]]; then
        return 1
    fi
    
    # Check if file starts with ---
    local first_line=$(head -n1 "$file")
    if [[ "$first_line" != "---" ]]; then
        return 1
    fi
    
    # Check if file has closing front matter
    if ! grep -q "^---" "$file"; then
        return 1
    fi
    
    return 0
}

# Function to extract title from filename or front matter
extract_title() {
    local file="$1"
    local basename=$(basename "$file" .md)
    
    # Try to extract title from front matter first
    if has_front_matter "$file"; then
        local title=$(sed -n '/^---/,/^---/p' "$file" | grep '^title:' | sed 's/^title:[[:space:]]*["'\'']*\([^"'\'']*\)["'\'']*/\1/')
        if [[ -n "$title" ]]; then
            echo "$title"
            return
        fi
    fi
    
    # Fallback to filename
    echo "$basename" | sed 's/^[0-9]\{4\}-[0-9]\{2\}-[0-9]\{2\}-//' | sed 's/-/ /g' | sed 's/\b\([a-z]\)/\u\1/g'
}

# Function to update front matter
update_front_matter() {
    local file="$1"
    local temp_file=$(mktemp)
    
    if has_front_matter "$file"; then
        # Update existing front matter
        awk '
        BEGIN { in_front_matter = 0; front_matter_ended = 0; published_found = 0 }
        /^---/ {
            if (in_front_matter == 0) {
                in_front_matter = 1
                print "---"
            } else if (front_matter_ended == 0) {
                in_front_matter = 0
                front_matter_ended = 1
                # Add published field if not found
                if (published_found == 0) {
                    print "published: true"
                }
                print "---"
            } else {
                print
            }
            next
        }
        in_front_matter == 1 {
            if ($0 ~ /^published:/) {
                print "published: true"
                published_found = 1
            } else {
                print
            }
            next
        }
        { print }
        ' "$file" > "$temp_file"
    else
        # Create new front matter
        local title=$(extract_title "$file")
        cat > "$temp_file" << EOF
---
layout: post
title: "$title"
published: true
---

$(cat "$file")
EOF
    fi
    
    mv "$temp_file" "$file"
}

# Function to list available drafts
list_drafts() {
    local drafts_dir="_drafts"
    
    if [[ ! -d "$drafts_dir" ]]; then
        print_error "Drafts directory '$drafts_dir' not found"
        return 1
    fi
    
    local draft_files=("$drafts_dir"/*.md)
    
    if [[ ${#draft_files[@]} -eq 0 || ! -f "${draft_files[0]}" ]]; then
        print_warning "No draft files found in '$drafts_dir'"
        return 1
    fi
    
    print_info "Available drafts:"
    local i=1
    for draft in "${draft_files[@]}"; do
        if [[ -f "$draft" ]]; then
            local title=$(extract_title "$draft")
            local has_fm=$(has_front_matter "$draft" && echo "✓" || echo "✗")
            printf "  %d) %s %s\n" "$i" "$title" "$has_fm"
            ((i++))
        fi
    done
    
    return 0
}

# Function to select draft
select_draft() {
    local drafts_dir="_drafts"
    local draft_files=("$drafts_dir"/*.md)
    local valid_files=()
    
    # Filter only existing files
    for draft in "${draft_files[@]}"; do
        if [[ -f "$draft" ]]; then
            valid_files+=("$draft")
        fi
    done
    
    while true; do
        echo -n -e "${BLUE}Select draft to promote (1-${#valid_files[@]}): ${NC}" >&2
        read -r selection
        
        if [[ "$selection" =~ ^[0-9]+$ ]] && [[ "$selection" -ge 1 ]] && [[ "$selection" -le ${#valid_files[@]} ]]; then
            echo "${valid_files[$((selection-1))]}"
            return 0
        else
            print_error "Invalid selection. Please enter a number between 1 and ${#valid_files[@]}"
        fi
    done
}

# Function to check for filename conflicts
check_conflict() {
    local target_file="$1"
    
    if [[ -f "$target_file" ]]; then
        print_warning "Target file already exists: $target_file"
        echo -n -e "${YELLOW}Overwrite? (y/N): ${NC}"
        read -r response
        if [[ "$response" =~ ^[Yy]$ ]]; then
            return 0
        else
            return 1
        fi
    fi
    
    return 0
}

# Main function
main() {
    # Parse command line arguments
    while [[ $# -gt 0 ]]; do
        case $1 in
            -d|--date)
                PUBLISH_DATE="$2"
                shift 2
                ;;
            -h|--help)
                HELP=true
                shift
                ;;
            *)
                print_error "Unknown option: $1"
                show_help
                exit 1
                ;;
        esac
    done
    
    # Show help if requested
    if [[ "$HELP" == true ]]; then
        show_help
        exit 0
    fi
    
    # Validate custom date if provided
    if [[ -n "$PUBLISH_DATE" ]]; then
        if ! validate_date "$PUBLISH_DATE"; then
            print_error "Invalid date format: $PUBLISH_DATE. Use YYYY-MM-DD format."
            exit 1
        fi
    else
        PUBLISH_DATE=$(date +%Y-%m-%d)
    fi
    
    print_info "Using publish date: $PUBLISH_DATE"
    
    # List and select draft
    if ! list_drafts; then
        exit 1
    fi
    
    local selected_draft=$(select_draft)
    local draft_filename=$(basename "$selected_draft")
    local draft_basename="${draft_filename%.md}"
    
    print_info "Selected draft: $selected_draft"
    
    # Validate draft has front matter
    if ! has_front_matter "$selected_draft"; then
        print_warning "Draft does not have valid front matter. A new front matter will be created."
    fi

    # Require topics and keywords
    if ! has_non_empty_flow_seq "$selected_draft" "topics"; then
        print_error "Draft is missing required 'topics' (non-empty). Update front matter and try again."
        exit 1
    fi
    if ! has_non_empty_flow_seq "$selected_draft" "keywords"; then
        print_error "Draft is missing required 'keywords' (non-empty). Update front matter and try again."
        exit 1
    fi
    
    # Generate target filename
    local slug="$draft_basename"
    # Remove date prefix if it exists
    slug=$(echo "$slug" | sed 's/^[0-9]\{4\}-[0-9]\{2\}-[0-9]\{2\}-//')
    local target_filename="${PUBLISH_DATE}-${slug}.md"
    local target_path="_posts/$target_filename"
    
    # Check for conflicts
    if ! check_conflict "$target_path"; then
        print_info "Promotion cancelled."
        exit 0
    fi
    
    # Create _posts directory if it doesn't exist
    mkdir -p "_posts"
    
    # Copy and update the draft
    print_info "Promoting draft to: $target_path"
    cp "$selected_draft" "$target_path"
    update_front_matter "$target_path"
    
    # Verify the update
    if grep -q "^published: true" "$target_path"; then
        # Remove the original draft since promotion was successful
        rm "$selected_draft"
        print_success "Draft successfully promoted!"
        print_info "File: $target_path"
        print_info "Original draft removed."
    else
        print_error "Failed to update front matter. Please check the file manually."
        exit 1
    fi
}

# Run main function
main "$@"