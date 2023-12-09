using System.IO;
using Mirror.Hosting.Container.Runtime.Controllers;
using Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes.Docker;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mirror.Hosting.Container.Editor
{
    public class ContainerHostingWindow : EditorWindow
    {
        VisualTreeAsset visualTree;
        VisualTreeAsset containerBuildTargetItem;
        ContainerWindowController containerWindowController;
        ContainerRuntimeStatusController containerRuntimeStatusController;
        DockerRuntime dockerRuntime;

        string StylesheetPath =>
            Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this)));

        [MenuItem("Hosting/Container Hosting")]
        public static void ShowContainerWindow()
        {
            ContainerHostingWindow hostingWindow = GetWindow<ContainerHostingWindow>();
            hostingWindow.titleContent = new GUIContent("Container Hosting");
            hostingWindow.Show();
        }

        public void CreateGUI()
        {
            rootVisualElement.Clear();
            visualTree.CloneTree(rootVisualElement);
            containerWindowController = new ContainerWindowController(rootVisualElement);
            dockerRuntime = new DockerRuntime();
            containerRuntimeStatusController = new ContainerRuntimeStatusController(rootVisualElement, dockerRuntime);
        }

        void OnEnable()
        {
            visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{StylesheetPath}/uxml/ContainerWindow.uxml");
            containerBuildTargetItem =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{StylesheetPath}/uxml/ContainerBuildTargetInputItem.uxml");
        }
    }
}
