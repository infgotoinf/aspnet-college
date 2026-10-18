#!/usr/bin/env bash

for f in Models/TaskItem.cs Pages/Tasks/Create.cshtml Pages/Tasks/Create.cshtml.cs; do
  printf '\n--- %s ---\n' "$f"
  cat "$f"
done
