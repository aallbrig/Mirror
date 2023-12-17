using Mirror.Hosting.Container.Common.Tests.Fakes;
using Mirror.Hosting.Container.Runtime.Controllers;
using Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes;
using Mirror.Hosting.Container.Runtime.Tests.EditMode.Fakes;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mirror.Hosting.Container.Runtime.Tests.PlayMode.Controllers
{
    public class RuntimeSummaryControllerTests
    {
        private VisualElement NewTestView()
        {
            VisualElement visualElem = new VisualElement();
            visualElem.Add(new Label { name = "user-info_container-runtime-status" });
            visualElem.Add(new Label { name = "user-info_container-runtime-version" });
            visualElem.Add(new Button { name = "foobar" });
            return visualElem;
        }
        private GameObject NewViewRunner(VisualElement view)
        {
            GameObject gameObject = new GameObject();
            UIDocument uiDoc = gameObject.AddComponent<UIDocument>();
            uiDoc.panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
            uiDoc.rootVisualElement.Add(view);
            return gameObject;
        }
        [Test]
        public void RuntimeSummaryController_()
        {
            GameObject viewRunner = NewViewRunner(NewTestView());
            FakeEventBroker fakeEventBroker = new FakeEventBroker();
            ContainerRuntimeRepository repository = new ContainerRuntimeRepository(new FakeRuntimeClient(fakeEventBroker));
            ContainerRuntimeService service = new ContainerRuntimeService(repository);
            RuntimeSummaryController sut = new RuntimeSummaryController(viewRunner.GetComponent<UIDocument>().rootVisualElement, service, fakeEventBroker);

            Assert.IsNotNull(sut);
        }
        [Test]
        public void ButtonReceivesClickEvent_Test()
        {
            bool wasClicked = false;
            Button btn = new Button { name = "foobar" };
            btn.RegisterCallback<ClickEvent>(_ => wasClicked = true);
            GameObject hostGameObject = new GameObject();
            UIDocument uiDoc = hostGameObject.AddComponent<UIDocument>();
            uiDoc.panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
            uiDoc.rootVisualElement.Add(btn);

            using (ClickEvent clkEvt = ClickEvent.GetPooled())
            {
                clkEvt.target = btn;
                btn.SendEvent(clkEvt);
            }

            Assert.IsTrue(wasClicked);
        }
    }
}
