#!/usr/bin/env bash

limit=${LIMIT:-25}
repo_root=$(git rev-parse --show-toplevel)
target_directories_unsorted=( \
  ./Assets/Mirror/Core \
  ./Assets/Mirror/Components \
  ./Assets/Mirror/Transports/KCP \
  ./Assets/Mirror/Transports/SimpleWeb \
  ./Assets/Mirror/Transports/Telepathy \
  ./Assets/Mirror/Transports/Edgegap \
  ./Assets/Mirror/Transports/Multiplex \
  ./Assets/Mirror/Transports/Middleware \
  ./Assets/Mirror/Transports/Latency \
  ./Assets/Mirror/Transports/Threaded \
  ./Assets/Mirror/Authenticators \
  ./Assets/Mirror/Hosting \
  ./Assets/Mirror/Tests/Runtime \
  ./Assets/Mirror/Tests/Editor \
  ./Assets/Mirror/Tests/EditorBehaviours \
  ./Assets/Mirror/Tests/Common \
)

function main() {
  # Check for Bash version 4 or higher (this script uses bash dictionaries)
  if [ "${BASH_VERSINFO[0]}" -lt 4 ]; then
      echo "This script requires Bash version 4 or higher. You are using Bash version ${BASH_VERSION}."
      exit 1
  fi

  local all_find_output=()
  local find_output
  local find_output_count
  local printed_count
  local sorted_target_directories
  # declare dictionary to hold line counts for each directory, to sort directories by total loc
  declare -A directory_total_loc_counts

  for path in "${target_directories_unsorted[@]}"; do
    full_path="$repo_root/$path"
    total_loc=$(find "$full_path" -name '*.cs' -type f -exec wc -l {} + | sort -nr | head -n 1 | awk '{ print $1 }')
    directory_total_loc_counts["$path"]=$total_loc
  done

  sorted_target_directories=($(for key in "${!directory_total_loc_counts[@]}"; do
    echo "${directory_total_loc_counts[$key]} $key"
  done | sort -nr | awk '{ print $2 }'))

  for path in "${sorted_target_directories[@]}"; do
    find_output=$(find "$path" -name '*.cs' -type f -exec wc -l {} + | sort -nr | sed "s|$path/||g")
    find_output_count=$(echo "$find_output" | wc -l)

    if [ "$find_output_count" -gt "$limit" ]; then
      printed_count=$limit
    else
      printed_count="${find_output_count// /}"
    fi

    if [ "$printed_count" -gt 1 ]; then
      printed_file="files"
    else
      printed_file="file"
    fi

    LC_NUMERIC=en_US.UTF-8 printf "Folder: %s, Total C# LOC: %'.d\n" \
      "$path" \
      "${directory_total_loc_counts[$path]}"

    printf "Top %'.d C# %s by LOC:\n" \
      "$printed_count" \
      "$printed_file"

    if [ "$find_output_count" -gt 1 ]; then
      filtered_results=$(echo -n "$find_output" | tail -n +2 | head -n "${limit}")
      echo -n "$filtered_results"
    else
      echo -n "$find_output"
    fi
    echo
    echo
  done

}

main
