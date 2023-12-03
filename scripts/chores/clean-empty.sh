#!/usr/bin/env bash

find_max_depth=10

function remove_empty() {
  empty=$1

  if [[ -f $empty ]]; then
    rm "$empty"
  fi
  if [[ -d $empty ]]; then
    rm -rf "$empty"
  fi
  if [[ -f "$empty.meta" ]]; then
    rm "$empty.meta"
  fi
}

function try_remove_by_parse_date() {
  csharp_file=$1
  # assumption 1: a date is contained in the first line of the file
  # assumption 2: if a date is found, it is describing a removal
  # assumption 3: the date is in the format YYYY-MM-DD
  parse_YYYY_MM_DD_date=$(cat "$csharp_file" | head -n 1 | grep -Eo '[0-9]{4}-[0-9]{2}-[0-9]{2}')
  if [[ -z $parse_YYYY_MM_DD_date ]]; then
    return
  fi

  if [[ $parse_YYYY_MM_DD_date < $one_year_ago_YYYY_MM_DD ]]; then
    echo "DELETE $(basename $csharp_file) because $parse_YYYY_MM_DD_date is older than $one_year_ago_YYYY_MM_DD"
    remove_empty $csharp_file
  fi
}

function main() {
  local repo_root=$(git rev-parse --show-toplevel)
  local one_year_ago_YYYY_MM_DD=$(date -v -1y +%Y-%m-%d)

  # find and delete "empty" files (files that were removed or moved or refactored)
  while IFS= read -r -d '' csharp_file; do
    try_remove_by_parse_date "$csharp_file"
  done < <(find $repo_root -maxdepth $find_max_depth -type f -name '*.cs' -print0)

  # remove
  while IFS= read -r -d '' empty_dir; do
    # remove empty directories
    empty_sub_dirs=$(find "$empty_dir" -type d -empty -delete -print)
    echo "$empty_sub_dirs" | xargs -I {} rm -rf "{}.meta"

    # find files but ignore .meta files
    files_count=$(find "$empty_dir" -type f ! -name '*.meta' | wc -l)
    if [ $files_count -eq 0 ]; then
      echo "DELETE $empty_dir because it is empty"
      rm -rf "$empty_dir"
    fi
  done < <(find $repo_root -maxdepth $find_max_depth -type d -name '*Empty*' -print0)
}

main