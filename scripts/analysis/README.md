# Analysis Scripts
This directory represents useful scripts for analysis

## Top C# Scripts by LOC script
This script outputs top C# files for select directories sorted by loc. Each section is sorted by total C# file LOC.

Run
```bash
# if running from root of Mirror repo:
./scripts/analysis/top-csharp-by-loc.sh

# specify limit (default 10)
LIMIT=25 ./scripts/analysis/top-csharp-by-loc.sh
```

Sample output
```text
$ LIMIT=3 ./scripts/analysis/top-csharp-by-loc.sh 
###########################
### Core Mirror Folders ###
###########################
    High Level Summary
    Folder                                     C# Files       Total LoC(clean)         Total LoC(raw)
    Assets/Mirror/Core                              120                 13,869                 16,247
    Assets/Mirror/Hosting                            97                  6,795                  7,826
    Assets/Mirror/Components                         40                  5,926                  6,996
    Assets/Mirror/Editor                             60                  5,429                  6,542
    Assets/Mirror/Authenticators                      3                    324                    391

    Details for Assets/Mirror/Core
    Top 3 of 120 files
    C# File                                                LoC(clean)       LoC(raw)
    NetworkServer.cs                                            1,653          1,886
    NetworkClient.cs                                            1,500          1,728
    NetworkManager.cs                                           1,260          1,498

    Details for Assets/Mirror/Hosting
    Top 3 of 97 files
    C# File                                                LoC(clean)       LoC(raw)
    Edgegap/Editor/EdgegapWindow.cs                               815            980
    Edgegap/Models/SDK/AppVersionUpdate.cs                        212            240
    Edgegap/Models/SDK/AppVersion.cs                              212            240
...
```
