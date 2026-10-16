for f in \
  Hubs/MonitoringHub.cs \
  Services/ConnectionService.cs \
  wwwroot/index.html \
  wwwroot/js/app.js \
  appsettings.json \
  appsettings.Development.json \
  shell.nix
do
  if [ -f "$f" ]; then
    printf '\n--- %s ---\n' "$f"
    cat "$f"
  fi
done
