# README

This script was created to simplify the process of connecting to remote servers using `tmux`.

## Usage

```bash
# Connect to the servers using the test config file
./tmux-ssh.sh test
```

```bash
# Without any parameters, the script will show the usage
./tmux-ssh.sh
```

## Parameters

- `test` - The name of the config file to use. This file should be located in the `~/.tmux-ssh` directory.