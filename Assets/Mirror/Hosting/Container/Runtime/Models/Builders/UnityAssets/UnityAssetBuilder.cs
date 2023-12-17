using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Mirror.Hosting.Container.Runtime.Models.Enums;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Mirror.Hosting.Container.Runtime.Models.Builders.UnityAssets
{

    public class UnityAssetBuilder
    {
        bool IsArmCPU() =>
            RuntimeInformation.ProcessArchitecture == Architecture.Arm ||
            RuntimeInformation.ProcessArchitecture == Architecture.Arm64;

        private readonly ContainerBuildTypes containerBuildType;

        public UnityAssetBuilder(ContainerBuildTypes containerBuildType)
        {
            this.containerBuildType = containerBuildType;
        }

        public BuildReport BuildUnityPlayer()
        {
            IEnumerable<string> scenes = EditorBuildSettings.scenes.Select(s=>s.path);
            BuildPlayerOptions buildPlayerOptions = GetBuildPlayerOptions(scenes);
            return BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        BuildPlayerOptions GetBuildPlayerOptions(IEnumerable<string> scenes)
        {
            if (containerBuildType == ContainerBuildTypes.LinuxGameServer)
            {
                BuildPlayerOptions options = new BuildPlayerOptions
                {
                    scenes = scenes.ToArray(),
                    target = BuildTarget.StandaloneLinux64,
#if UNITY_2021_3_OR_NEWER
                    subtarget = (int)StandaloneBuildSubtarget.Server,
#else
                    options = BuildOptions.EnableHeadlessMode,
#endif
                    locationPathName = "Builds/ContainerHosting/LinuxGameServerBuild"
                };

                if (IsArmCPU())
                    // https://forum.unity.com/threads/build-headless-linux-for-docker-on-mac-m2.1402429/
                    options.options = BuildOptions.Development;

                return options;
            }

            Debug.LogError("Unsupported container build type");
            return new BuildPlayerOptions();
        }
    }

}
