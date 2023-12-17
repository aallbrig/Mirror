using System;
using Mirror.Hosting.Container.Common.Tests.Fakes;
using Mirror.Hosting.Container.Runtime.Controllers;
using Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes;
using Mirror.Hosting.Container.Runtime.Tests.EditMode.Fakes;
using NUnit.Framework;
using UnityEngine.UIElements;

namespace Mirror.Hosting.Container.Runtime.Tests.EditMode.Controllers
{
    public class RuntimeSummaryControllerTests
    {
        private VisualElement FakeView()
        {
            VisualElement view = new VisualElement();
            view.Add(new Label { name = "user-info_container-runtime-status" });
            view.Add(new Label { name = "user-info_container-runtime-version" });
            return view;
        }
        private static VisualElement CreateVisualElementWithChildren<T>(params string[] childNames)
            where T : VisualElement, new()
        {
            VisualElement view = new VisualElement();
            foreach (string childName in childNames)
                view.Add(new T { name = childName });
            return view;
        }

        static VisualElement[] failingViews = new VisualElement[]
        {
            new VisualElement(),
            CreateVisualElementWithChildren<Label>(new string[] { "user-info_container-runtime-status" }),
            CreateVisualElementWithChildren<Label>(new string[] { "user-info_container-runtime-version" })
        };
        [Test]
        public void RuntimeSummaryController_ComplainsIfUnableToFindElementsOnView([ValueSource(nameof(failingViews))] VisualElement failingView)
        {
            try
            {
                FakeEventBroker fakeEventBroker = new FakeEventBroker();
                FakeRuntimeClient fakeRuntimeClient = new FakeRuntimeClient(fakeEventBroker);
                ContainerRuntimeRepository containerRuntimeRepository = new ContainerRuntimeRepository(fakeRuntimeClient);
                ContainerRuntimeService containerRuntimeService = new ContainerRuntimeService(containerRuntimeRepository);

                RuntimeSummaryController sut = new RuntimeSummaryController(failingView, containerRuntimeService, fakeEventBroker);

                Assert.Fail();
            }
            catch (NullReferenceException ex)
            {
                Assert.Pass($"expected null reference exception detected:\n{ex.Message}");
            }
            catch (Exception ex)
            {
                // NUnity.Framework.SuccessException is thrown when Assert.Pass() is called (?!)
                if (ex is SuccessException) return;

                Assert.Fail($"unexpected exception detected:\n{ex}");
            }
        }
    }
}
