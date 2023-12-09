using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mirror.Hosting.Container.Runtime.Controllers
{
    public class ContainerWindowController
    {
        // User Input
        TextField buildOutputRootTextField;
        // User Actions
        readonly Button documentationButton;
        readonly Button addContainerBuildTargetButton;
        readonly Button buildAndRunButton;
        // Containers
        VisualElement containerBuildTargetsContainer;

        public ContainerWindowController(VisualElement view)
        {
            // User Actions
            documentationButton = view.Q<Button>("user-action_open-documentation");
            if (documentationButton == null)
                throw new NullReferenceException("cannot find button with query 'user-action_open-documentation'");
            buildAndRunButton = view.Q<Button>("user-action_build-and-run-container");
            if (buildAndRunButton == null)
                throw new NullReferenceException("cannot find button with query 'user-action_build-and-run-container'");
            addContainerBuildTargetButton = view.Q<Button>("user-action_add-container-build-target");
            if (addContainerBuildTargetButton == null)
                throw new NullReferenceException("cannot find button with query 'user-action_add-container-build-target'");
            // Containers
            containerBuildTargetsContainer = view.Q<VisualElement>("container_container-build-targets");
            if (containerBuildTargetsContainer == null)
                throw new NullReferenceException("cannot find container with query 'container_container-build-targets'");

            documentationButton.clickable.clicked += OpenDocumentationCallback;
            buildAndRunButton.clickable.clicked += BuildAndRunContainers;
            addContainerBuildTargetButton.clickable.clicked += AddContainerBuildTarget;
        }

        void AddContainerBuildTarget()
        {
            Debug.Log("Container Hosting | Add Container Build Target");
        }

        void BuildAndRunContainers()
        {
            Debug.Log("Container Hosting | Building");
        }

        void OpenDocumentationCallback()
        {
            // TODO: write this documentation in the MirrorDocs repo
            Application.OpenURL("https://mirror-networking.gitbook.io/docs/hosting/container-hosting-plugin-guide");
        }
    }
}
