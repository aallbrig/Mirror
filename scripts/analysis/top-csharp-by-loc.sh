#!/usr/bin/env bash

limit=${LIMIT:-25}
repo_root=$(git rev-parse --show-toplevel)
core_directories=( \
  Assets/Mirror/Authenticators \
  Assets/Mirror/Core \
  Assets/Mirror/Components \
  Assets/Mirror/Editor \
  Assets/Mirror/Hosting \
)
transport_directories=( \
  Assets/Mirror/Transports/Edgegap \
  Assets/Mirror/Transports/KCP \
  Assets/Mirror/Transports/Telepathy \
  Assets/Mirror/Transports/Threaded \
  Assets/Mirror/Transports/Latency \
  Assets/Mirror/Transports/Middleware \
  Assets/Mirror/Transports/Multiplex \
  Assets/Mirror/Transports/SimpleWeb \
)
test_directories=( \
  Assets/Mirror/Tests/Common \
  Assets/Mirror/Tests/Editor \
  Assets/Mirror/Tests/EditorBehaviours \
  Assets/Mirror/Tests/Runtime \
)
examples_directories=( \
  Assets/Mirror/Examples/_Common \
  Assets/Mirror/Examples/AdditiveLevels \
  Assets/Mirror/Examples/AdditiveScenes \
  Assets/Mirror/Examples/Basic \
  Assets/Mirror/Examples/Benchmark \
  Assets/Mirror/Examples/BenchmarkIdle \
  Assets/Mirror/Examples/Billiards \
  Assets/Mirror/Examples/BilliardsPredicted \
  Assets/Mirror/Examples/CCU \
  Assets/Mirror/Examples/CharacterSelection \
  Assets/Mirror/Examples/Chat \
  Assets/Mirror/Examples/CouchCoop \
  Assets/Mirror/Examples/Discovery \
  Assets/Mirror/Examples/LagCompensation \
  Assets/Mirror/Examples/MultipleAdditiveScenes \
  Assets/Mirror/Examples/MultipleMatches \
  Assets/Mirror/Examples/Pong \
  Assets/Mirror/Examples/RigidbodyBenchmark \
  Assets/Mirror/Examples/RigidbodyPhysics \
  Assets/Mirror/Examples/Room \
  Assets/Mirror/Examples/SyncDirection \
  Assets/Mirror/Examples/Tanks \
  Assets/Mirror/Examples/TanksCoop \
)

function process_directories() {
  local directories=("$@")
  local all_find_output=()
  local find_output
  local find_output_count
  local printed_count
  local sorted_target_directories
  declare -A directory_total_loc_counts

  for path in "${directories[@]}"; do
    full_path="$repo_root/$path"
    total_loc=$(find "$full_path" -name '*.cs' -type f -exec wc -l {} + | sort -nr | head -n 1 | awk '{ print $1 }')
    directory_total_loc_counts["$path"]=$total_loc
  done

  sorted_target_directories=($(for key in "${!directory_total_loc_counts[@]}"; do
    echo "${directory_total_loc_counts[$key]} $key"
  done | sort -nr | awk '{ print $2 }'))

  # calculate longest directory
  path_length_with_padding=0
  for path in "${sorted_target_directories[@]}"; do
    current_length=${#path}
    if (( current_length > path_length_with_padding )); then
      path_length_with_padding=$current_length
    fi
  done
  # add padding
  let path_length_with_padding+=2

  echo "High Level Summary"
  for path in "${sorted_target_directories[@]}"; do
    find_output=$(find "$path" -name '*.cs' -type f -exec wc -l {} + | sort -nr | sed "s|$path/||g")
    find_output_count=$(echo "$find_output" | wc -l)
    local find_output_count_without_summary
    if [ $find_output_count -eq 1 ]; then
      find_output_count_without_summary=1
    else
      find_output_count_without_summary=$((find_output_count - 1))
    fi

    LC_NUMERIC=en_US.UTF-8 printf "     Folder: %-${path_length_with_padding}sC# Files: %'3d   Total C# LOC: %'8d\n" \
      "$path" \
      "$find_output_count_without_summary" \
      "${directory_total_loc_counts[$path]}"
  done
  echo

  for path in "${sorted_target_directories[@]}"; do
    find_output=$(find "$path" -name '*.cs' -type f -exec wc -l {} + | sort -nr | sed "s|$path/||g")
    find_output_count=$(echo "$find_output" | wc -l)
    find_output_count_without_summary=$((find_output_count - 1))

    if [ "$find_output_count" -gt "$limit" ]; then
      printed_count=$limit
    elif [ "$find_output_count" -eq 1 ]; then
      printed_count=1
      find_output_count_without_summary=1
    else
      printed_count="${find_output_count_without_summary// /}"
    fi

    if [ "$printed_count" -gt 1 ]; then
      printed_file="files"
    else
      printed_file="file"
    fi

    LC_NUMERIC=en_US.UTF-8 printf "Top %'.d of %'.d %s in %s Folder\n" \
      "$printed_count" \
      "$find_output_count_without_summary" \
      "$printed_file" \
      "$path"

    echo "     LOC C# File"
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

function main() {
  # Check for Bash version 4 or higher (this script uses bash dictionaries)
  if [ "${BASH_VERSINFO[0]}" -lt 4 ]; then
      echo "This script requires Bash version 4 or higher. You are using Bash version ${BASH_VERSION}."
      exit 1
  fi

  echo "###########################"
  echo "### Core Mirror Folders ###"
  echo "###########################"
  process_directories "${core_directories[@]}"
  echo
  echo "###########################"
  echo "#### Transport Folders ####"
  echo "###########################"
  process_directories "${transport_directories[@]}"
  echo
  echo "###########################"
  echo "###### Test  Folders ######"
  echo "###########################"
  process_directories "${test_directories[@]}"
  echo
  echo "###########################"
  echo "#### Examples  Folders ####"
  echo "###########################"
  process_directories "${examples_directories[@]}"
  echo
}

main
