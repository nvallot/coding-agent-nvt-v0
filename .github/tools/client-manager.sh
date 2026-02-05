#!/bin/bash

# Client Manager Script
# Manages switching between different client configurations
# Usage: ./client-manager.sh [command] [client-name]

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
CLIENTS_DIR="$SCRIPT_DIR/../clients"
ACTIVE_CLIENT_FILE="$CLIENTS_DIR/active-client.json"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Print colored output
print_info() {
    echo -e "${BLUE}ℹ ${NC}$1"
}

print_success() {
    echo -e "${GREEN}✓${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}⚠${NC} $1"
}

print_error() {
    echo -e "${RED}✗${NC} $1"
}

# Show usage
show_usage() {
    cat << EOF
Client Manager - Manage GitHub Copilot agent client configurations

Usage:
    $0 list                     List all available clients
    $0 show                     Show current active client
    $0 activate <client-name>   Activate a specific client
    $0 create <client-name>     Create a new client from template
    $0 validate <client-name>   Validate client configuration
    $0 help                     Show this help message

Examples:
    $0 list
    $0 show
    $0 activate contoso
    $0 create fabrikam
    $0 validate contoso

EOF
}

# List all available clients
list_clients() {
    print_info "Available clients:"
    echo ""
    
    for dir in "$CLIENTS_DIR"/*; do
        if [ -d "$dir" ] && [ "$(basename "$dir")" != "template" ]; then
            client_name=$(basename "$dir")
            if [ -f "$dir/CLIENT.md" ]; then
                # Extract client name from CLIENT.md if possible
                display_name=$(grep -m 1 "^#" "$dir/CLIENT.md" | sed 's/^# //' | sed 's/ - Client Configuration.*//')
                if [ "$client_name" == "default" ]; then
                    echo "  • $client_name (Default configuration)"
                else
                    echo "  • $client_name"
                fi
            else
                print_warning "  • $client_name (missing CLIENT.md)"
            fi
        fi
    done
    echo ""
}

# Show current active client
show_current() {
    if [ ! -f "$ACTIVE_CLIENT_FILE" ]; then
        print_error "No active client configuration found"
        exit 1
    fi
    
    active_client=$(grep -o '"activeClient": *"[^"]*"' "$ACTIVE_CLIENT_FILE" | sed 's/"activeClient": *"//' | sed 's/"//')
    last_updated=$(grep -o '"lastUpdated": *"[^"]*"' "$ACTIVE_CLIENT_FILE" | sed 's/"lastUpdated": *"//' | sed 's/"//')
    
    print_info "Current active client: ${GREEN}$active_client${NC}"
    print_info "Last updated: $last_updated"
    echo ""
    
    client_dir="$CLIENTS_DIR/$active_client"
    if [ -d "$client_dir" ] && [ -f "$client_dir/CLIENT.md" ]; then
        print_info "Client details:"
        echo ""
        # Show first few lines of CLIENT.md
        head -n 20 "$client_dir/CLIENT.md"
        echo ""
        print_info "Full configuration: $client_dir/CLIENT.md"
    fi
}

# Activate a specific client
activate_client() {
    local client_name="$1"
    
    if [ -z "$client_name" ]; then
        print_error "Client name required"
        echo "Usage: $0 activate <client-name>"
        exit 1
    fi
    
    client_dir="$CLIENTS_DIR/$client_name"
    
    if [ ! -d "$client_dir" ]; then
        print_error "Client '$client_name' does not exist"
        print_info "Available clients:"
        list_clients
        exit 1
    fi
    
    if [ ! -f "$client_dir/CLIENT.md" ]; then
        print_error "Client '$client_name' is missing CLIENT.md configuration"
        exit 1
    fi
    
    # Update active-client.json
    timestamp=$(date -u +"%Y-%m-%dT%H:%M:%SZ")
    cat > "$ACTIVE_CLIENT_FILE" << EOF
{
  "activeClient": "$client_name",
  "lastUpdated": "$timestamp",
  "description": "Defines which client configuration is currently active. Change 'activeClient' to switch between client contexts."
}
EOF
    
    print_success "Activated client: ${GREEN}$client_name${NC}"
    print_info "Configuration: $client_dir/CLIENT.md"
    print_info "Updated: $timestamp"
}

# Create a new client from template
create_client() {
    local client_name="$1"
    
    if [ -z "$client_name" ]; then
        print_error "Client name required"
        echo "Usage: $0 create <client-name>"
        exit 1
    fi
    
    # Validate client name (lowercase, alphanumeric, hyphens only)
    if [[ ! "$client_name" =~ ^[a-z0-9-]+$ ]]; then
        print_error "Invalid client name. Use only lowercase letters, numbers, and hyphens."
        exit 1
    fi
    
    client_dir="$CLIENTS_DIR/$client_name"
    template_dir="$CLIENTS_DIR/template"
    
    if [ -d "$client_dir" ]; then
        print_error "Client '$client_name' already exists"
        exit 1
    fi
    
    if [ ! -d "$template_dir" ]; then
        print_error "Template directory not found: $template_dir"
        exit 1
    fi
    
    print_info "Creating new client: $client_name"
    
    # Copy template
    mkdir -p "$client_dir/config"
    cp "$template_dir/CLIENT.md" "$client_dir/"
    
    # Replace [Client Name] placeholder
    if [[ "$OSTYPE" == "darwin"* ]]; then
        # macOS
        sed -i '' "s/\[Client Name\]/$client_name/g" "$client_dir/CLIENT.md"
    else
        # Linux
        sed -i "s/\[Client Name\]/$client_name/g" "$client_dir/CLIENT.md"
    fi
    
    print_success "Created client: $client_name"
    print_info "Configuration: $client_dir/CLIENT.md"
    print_warning "Please edit CLIENT.md to customize the configuration"
    echo ""
    print_info "To activate this client, run:"
    echo "  $0 activate $client_name"
}

# Validate client configuration
validate_client() {
    local client_name="$1"
    
    if [ -z "$client_name" ]; then
        print_error "Client name required"
        echo "Usage: $0 validate <client-name>"
        exit 1
    fi
    
    client_dir="$CLIENTS_DIR/$client_name"
    
    print_info "Validating client: $client_name"
    echo ""
    
    # Check if client directory exists
    if [ ! -d "$client_dir" ]; then
        print_error "Client directory does not exist: $client_dir"
        return 1
    fi
    print_success "Client directory exists"
    
    # Check if CLIENT.md exists
    if [ ! -f "$client_dir/CLIENT.md" ]; then
        print_error "CLIENT.md not found"
        return 1
    fi
    print_success "CLIENT.md exists"
    
    # Check if config directory exists
    if [ ! -d "$client_dir/config" ]; then
        print_warning "config/ directory does not exist (optional)"
    else
        print_success "config/ directory exists"
    fi
    
    # Check for placeholder values in CLIENT.md
    if grep -q "\[Client Name\]" "$client_dir/CLIENT.md" || \
       grep -q "\[e.g.," "$client_dir/CLIENT.md" || \
       grep -q "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX" "$client_dir/CLIENT.md"; then
        print_warning "CLIENT.md contains placeholder values - please customize"
    else
        print_success "No obvious placeholders in CLIENT.md"
    fi
    
    echo ""
    print_success "Validation complete for: $client_name"
}

# Main script logic
case "${1:-}" in
    list)
        list_clients
        ;;
    show|current)
        show_current
        ;;
    activate|switch)
        activate_client "$2"
        ;;
    create|new)
        create_client "$2"
        ;;
    validate|check)
        validate_client "$2"
        ;;
    help|--help|-h)
        show_usage
        ;;
    *)
        print_error "Unknown command: ${1:-}"
        echo ""
        show_usage
        exit 1
        ;;
esac
