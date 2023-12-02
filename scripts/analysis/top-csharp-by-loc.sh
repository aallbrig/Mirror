#!/usr/bin/env bash

limit=${LIMIT:-10}
# awk program used to ignore blank lines and comments in C# files
clean_csharp_awk='
BEGIN { code_line=0; }
/^\s*\/\// {next}          # Skip single-line comments
/^\s*$/ {next}             # Skip blank lines
/\/\*/,/\*\// {next}       # Skip multiline comments
{ code_line++; }
END { printf "%8d %s\n", code_line, FILENAME; }
'
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

declare -A files_for_dir_dict
declare -A total_files_for_dir_dict
declare -A total_raw_csharp_loc_for_dir_dict
declare -A total_clean_csharp_loc_for_dir_dict
declare -A loc_raw_for_csharp_file_dict
declare -A loc_clean_for_csharp_file_dict

function report::__longest_item_length() {
  local items=("$@")
  local longest_length=0

  for item in "${items[@]}"; do
    current_length=${#item}
    if (( current_length > longest_length )); then
      longest_length=$current_length
    fi
  done

  echo $longest_length
}

function report::sort_directories() {
  for key in "${!total_clean_csharp_loc_for_dir_dict[@]}"; do
    echo "${total_clean_csharp_loc_for_dir_dict[$key]} $key"
  done | sort -nr | awk '{ print $2 }'
}

function report::__get_stats() {
  local directory="$1"
  local full_path="$repo_root/$directory"

  local find_csharp_files=$(find "$full_path" -name '*.cs' -type f)
  local csharp_file_count=$(printf '%s\n' "$find_csharp_files" | wc -l)
  total_files_for_dir_dict[$directory]="$csharp_file_count"

  local find_raw_loc=$(printf '%s\n' "$find_csharp_files" | xargs -I {} wc -l {} | sed "s|$full_path/||g" | sort -nr)
  local find_clean_loc=$(printf '%s\n' "$find_csharp_files" | xargs -I {} awk "$clean_csharp_awk" {} | sed "s|$full_path/||g" | sort -nr)
  local csharp_files_sort_by_clean_loc=("$(printf '%s\n' "$find_clean_loc" | awk '{ print $2 }')")
  files_for_dir_dict[$directory]=$(IFS="|"; echo "${csharp_files_sort_by_clean_loc[@]}")

  while IFS= read -r loc_line; do
    local csharp_filepath=$(echo $loc_line | awk '{ print $2 }')
    local count=$(echo $loc_line | awk '{ print $1 }')
    loc_raw_for_csharp_file_dict["$csharp_filepath"]=$count
  done <<< $find_raw_loc

  while IFS= read -r loc_line; do
    local csharp_filepath=$(echo $loc_line | awk '{ print $2 }')
    local count=$(echo $loc_line | awk '{ print $1 }')
    loc_clean_for_csharp_file_dict["$csharp_filepath"]=$count
  done <<< $find_clean_loc

  local total_raw_csharp_loc=$(echo -n "$find_raw_loc" | awk '{ sum += $1 } END { print sum }')
  local total_clean_csharp_loc=$(echo -n "$find_clean_loc" | awk '{ sum += $1 } END { print sum }')

  total_raw_csharp_loc_for_dir_dict["$directory"]=$total_raw_csharp_loc
  total_clean_csharp_loc_for_dir_dict["$directory"]=$total_clean_csharp_loc
}

function report::__high_level_summary() {
  local directories=("$@")
  local longest_length=$(report::__longest_item_length "${directories[@]}")
  local display_length=20
  if [ $longest_length -gt $display_length ]; then
    display_length=$longest_length
  fi
  local header_row_format="    %-${display_length}s   %20s   %20s   %20s\n"
  local data_row_format="    %-${display_length}s   %'20d   %'20d   %'20d\n"

  echo "    High Level Summary"
  printf "$header_row_format" "Folder" "C# Files" "Total LoC(clean)" "Total LoC(raw)"
  for directory in "${directories[@]}"; do
    local csharp_file_count="${total_files_for_dir_dict[$directory]}"
    local total_raw_csharp_loc="${total_raw_csharp_loc_for_dir_dict[$directory]}"
    local total_clean_csharp_loc="${total_clean_csharp_loc_for_dir_dict[$directory]}"
    LC_NUMERIC=en_US.UTF-8 printf "$data_row_format" "$directory" "$csharp_file_count" "$total_clean_csharp_loc" "$total_raw_csharp_loc"
  done
  echo
}

function report::__details__files_info() {
  local csharp_files="$@"
  local count=$(printf "%s\n" $csharp_files | wc -l)

  local printed_count=$count
  local printed_file="files"
  if [ $count -eq 1 ]; then
    printed_count=1
    printed_file="file"
  elif [ $count -gt $limit ]; then
    printed_count=$limit
  fi

  LC_NUMERIC=en_US.UTF-8 printf "    Top %'.d of %'.d %s\n" \
    "$printed_count" \
    "$count" \
    "$printed_file"
}

function report::__details() {
  local directories=("$@")

  for directory in "${directories[@]}"; do
    echo "    Details for $directory"
    mapfile -t csharp_files_sort_by_clean_loc <<< "${files_for_dir_dict[$directory]}"
    report::__details__files_info "${csharp_files_sort_by_clean_loc[@]}"
    csharp_files_sort_by_clean_loc_limited=("${csharp_files_sort_by_clean_loc[@]:0:$limit}")

    local longest_length=$(report::__longest_item_length "${csharp_files_sort_by_clean_loc_limited[@]}")
    local display_length=50
    if [ $longest_length -gt $display_length ]; then
      display_length=$longest_length
    fi
    local header_row_format="    %-${display_length}s   %12s   %12s\n"
    local data_row_format="    %-${display_length}s   %'12d   %'12d\n"

    printf "$header_row_format" "C# File" "LoC(clean)" "LoC(raw)"
    for csharp_file in "${csharp_files_sort_by_clean_loc_limited[@]}"; do
      local loc_raw="${loc_raw_for_csharp_file_dict[$csharp_file]}"
      local loc_clean="${loc_clean_for_csharp_file_dict[$csharp_file]}"
      LC_NUMERIC=en_US.UTF-8 printf "$data_row_format" "$csharp_file" "$loc_clean" "$loc_raw"
    done
    echo
  done
  echo
}

function report::process_directories() {
  local directories=("$@")

  # reset dictionaries
  declare -A files_for_dir_dict
  declare -A total_raw_csharp_loc_for_dir_dict
  declare -A total_clean_csharp_loc_for_dir_dict
  declare -A loc_raw_for_csharp_file_dict
  declare -A loc_clean_for_csharp_file_dict

  for directory in "${directories[@]}"; do
    full_path="$repo_root/$directory"
    report::__get_stats "$directory"
  done

  IFS=$'\n' read -r -d '' -a sorted_directories < <(report::sort_directories && printf '\0')

  report::__high_level_summary "${sorted_directories[@]}"
  report::__details "${sorted_directories[@]}"
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
  report::process_directories "${core_directories[@]}"
  echo "###########################"
  echo "#### Transport Folders ####"
  echo "###########################"
  report::process_directories "${transport_directories[@]}"
  echo "###########################"
  echo "###### Test  Folders ######"
  echo "###########################"
  report::process_directories "${test_directories[@]}"
  echo "###########################"
  echo "#### Examples  Folders ####"
  echo "###########################"
  report::process_directories "${examples_directories[@]}"
}

main
