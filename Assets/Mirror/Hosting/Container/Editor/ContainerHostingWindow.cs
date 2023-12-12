using System.IO;
using Mirror.Hosting.Container.Runtime.Controllers;
using Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes;
using Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes.Docker;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mirror.Hosting.Container.Editor
{
    public class ContainerHostingWindow : EditorWindow
    {
        static ContainerRuntimeClient GetContainerRuntimeClient(ContainerRuntimeEventBroker eventBroker)
        {
            // todo: add support for other container runtimes
            return new DockerRuntimeClient(eventBroker);
        }
        VisualTreeAsset visualTree;
        VisualTreeAsset containerBuildTargetItem;
        ContainerWindowController containerWindowController;
        ContainerRuntimeStatusController containerRuntimeStatusController;
        ContainerRuntimeClient client;
        ContainerRuntimeRepository repository;
        ContainerRuntimeService service;
        private ContainerRuntimeEventBroker eventBroker;

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
            containerRuntimeStatusController = new ContainerRuntimeStatusController(rootVisualElement, service, eventBroker);
        }

        void OnEnable()
        {
            visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{StylesheetPath}/uxml/ContainerWindow.uxml");
            containerBuildTargetItem =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{StylesheetPath}/uxml/ContainerBuildTargetInputItem.uxml");
            eventBroker = new ContainerRuntimeEventBroker();
            client = GetContainerRuntimeClient(eventBroker);
            repository = new ContainerRuntimeRepository(client);
            service = new ContainerRuntimeService(repository);
        }
    }
}
