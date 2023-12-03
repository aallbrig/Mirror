# Chore Scripts
These scripts are intended to be used to help with development and maintenance of Mirror.

### Delete Empty Files/Folders Script
This script will delete all empty files and folders that are older than 1 year from today, for files that have YYYY-MM-DD as a comment. This is useful for cleaning up old files that are no longer needed.

```bash
# clear empty files/folders greater than 1 year (assumes files have YYYY-MM-DD as a comment)
./scripts/chores/clean-empty.sh
# manually clean up other files by visiting directories below and reading files
find . -maxdepth 10 -type d -name '*Empty*' -print
```
