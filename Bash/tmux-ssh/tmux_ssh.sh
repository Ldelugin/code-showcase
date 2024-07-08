#!/bin/bash

# Check if an environment name is provided
if [ $# -eq 0 ]; then
    echo "Usage: $0 <environment>"
    exit 1
fi

ENVIRONMENT=$1
CONFIG_FILE="config.$ENVIRONMENT.cfg"

# Check if the configuration file exists
if [ ! -f $CONFIG_FILE ]; then
    echo "Configuration file for '$ENVIRONMENT' does not exist."
    exit 1
fi


# Source the configuration
source $CONFIG_FILE

echo "Username: ${USER}"
echo "Domain: ${DOMAIN}"
echo "Key: ${KEY}"

declare -a SSH_COMMANDS=()

# Now iterate over the associative array
for label in "${!SERVERS[@]}"; do
    host=${SERVERS[$label]}
    echo "Label: $label, Host: $host"
    SSH_COMMANDS+=("ssh -i $KEY $USER@$DOMAIN@$host")
done

# Start a new tmux session and detach immediately
tmux new-session -s SSH_Sessions -n "$ENVIRONMENT" -d

# Split the window into four panes
tmux split-window -h
tmux split-window -v
tmux select-pane -t 0
tmux split-window -v

# Start an SSH session in each pane
for i in {0..3}; do
  tmux send-keys -t ${i} "${SSH_COMMANDS[$i]}" C-m
done

tmux attach-session -t SSH_Sessions # Attach to the session at the end