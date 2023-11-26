# Analysis Scripts
This directory represents useful scripts for analysis

## Top C# Scripts by LOC script
This script outputs top C# files for select directories sorted by loc. Each section is sorted by total C# file LOC.

Run
```bash
# if running from root of Mirror repo:
./scripts/analysis/top-csharp-by-loc.sh

# specify limit (default 25)
LIMIT=10 ./scripts/analysis/top-csharp-by-loc.sh
```

Sample output
```text
bash-5.2$ LIMIT=3 ./scripts/analysis/top-csharp-by-loc.sh 

Folder: ./Assets/Mirror/Tests/Editor, Total C# LOC: 22,699
Top 3 C# files by LOC:
    1728 NetworkReaderWriter/NetworkWriterTest.cs
    1374 NetworkServer/NetworkServerTest.cs
     967 NetworkBehaviour/NetworkBehaviourTests.cs

Folder: ./Assets/Mirror/Core, Total C# LOC: 16,214
Top 3 C# files by LOC:
    1886 NetworkServer.cs
    1728 NetworkClient.cs
    1465 NetworkManager.cs

Folder: ./Assets/Mirror/Hosting, Total C# LOC: 7,795
Top 3 C# files by LOC:
     949 Edgegap/Editor/EdgegapWindow.cs
     240 Edgegap/Models/SDK/AppVersionUpdate.cs
     240 Edgegap/Models/SDK/AppVersion.cs

Folder: ./Assets/Mirror/Components, Total C# LOC: 6,996
Top 3 C# files by LOC:
     685 NetworkRoomManager.cs
     619 NetworkAnimator.cs
     476 NetworkTransform/NetworkTransformBase.cs

Folder: ./Assets/Mirror/Transports/KCP, Total C# LOC: 3,988
Top 3 C# files by LOC:
    1118 kcp2k/kcp/Kcp.cs
     746 kcp2k/highlevel/KcpPeer.cs
     411 kcp2k/highlevel/KcpServer.cs
...
```
