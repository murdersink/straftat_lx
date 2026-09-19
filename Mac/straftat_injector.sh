#!/bin/zsh
set -euo pipefail

# Standalone startup injector for the native macOS Unity player.
# Usage:
#   ./Mac/straftat_injector.sh "/path/to/STRAFTAT"
#
# The game must already contain BepInEx and the two Straftat_lx plugin DLLs.
# BepInEx 5.4.22's runner is used here because it is stable with this Unity
# player. This starts the game before injection; it does not attach to an
# already running process.

GAME_DIR="${1:-${STRAFTAT_GAME_DIR:-$HOME/Library/Application Support/Steam/steamapps/common/STRAFTAT}}"
GAME_APP="$GAME_DIR/STRAFTAT.app"
RUNNER="$GAME_DIR/run_bepinex.sh"

if [[ ! -d "$GAME_APP" ]]; then
  print -u2 "STRAFTAT.app was not found in: $GAME_DIR"
  print -u2 "Pass the game directory as the first argument."
  exit 1
fi

if [[ ! -x "$RUNNER" ]]; then
  print -u2 "BepInEx runner was not found or is not executable: $RUNNER"
  exit 1
fi

if [[ ! -f "$GAME_DIR/BepInEx/plugins/Straftat_lx.BepInEx.dll" ]]; then
  print -u2 "Straftat_lx.BepInEx.dll is missing from BepInEx/plugins."
  exit 1
fi

if [[ "$(uname -m)" == "arm64" ]]; then
  exec arch -x86_64 "$RUNNER" "$GAME_APP"
else
  exec "$RUNNER" "$GAME_APP"
fi
