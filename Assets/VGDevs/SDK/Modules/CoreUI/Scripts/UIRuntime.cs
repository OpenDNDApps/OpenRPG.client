using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VGDevs
{
    using static GameResources;
    using static GameUIResourcesCollection;
    
    public class UIRuntime : MonoSingletonSelfGenerated<UIRuntime>, IGameDependency
    {
		[SerializeField] private List<UIScreenPanelContainer> m_canvases = new List<UIScreenPanelContainer>();
        [SerializeField] private Camera m_camera;
        
		public Camera Camera => m_camera == null ? m_camera = Camera.main : m_camera;

        public event Action<UIWindow> OnWindowRemoved;
        private Dictionary<UISectionType, List<UIWindow>> m_windowHistory = new Dictionary<UISectionType, List<UIWindow>>();

		protected override void OnSingletonAwake()
        {
            base.OnSingletonAwake();
            SceneManager.activeSceneChanged += OnActiveSceneChange;
            Initialize();
        }

        private void OnActiveSceneChange(Scene current, Scene next)
        {
            m_camera = Camera.main;
            Initialize();
        }

        private void Initialize()
        {
            foreach (var container in m_canvases)
            {
                container.Canvas.worldCamera = m_camera;
                if (!Settings.UI.Sorting.Exists(sort => sort.Type.Equals(container.Type)))
                    continue;

                UISortingKeyPair sort = Settings.UI.Sorting.Find(sort => sort.Type.Equals(container.Type));
                container.Canvas.sortingOrder = sort.SortOrder;
                container.Canvas.planeDistance = sort.PlaneDistance;
                container.Canvas.sortingLayerName = sort.Type.ToString();
            }

            m_windowHistory.Clear();
            foreach (var sectionType in (UISectionType[]) Enum.GetValues(typeof(UISectionType)))
            {
                m_windowHistory.Add(sectionType, new List<UIWindow>());
            }

            IsReady = true;
        }

        public static Canvas GetCanvasOfType(UISectionType type)
        {
            var found = GameResources.UIRuntime.m_canvases.Find(canvas => canvas.Type == type);
            if (found.Canvas == null)
            {
                Debug.LogError($"Canvas of type '{type}' does not exist in UI Controller.");
            }
            return found.Canvas;
        }

        public void ShowTransition(Action onComplete = null, string transitionKey = null, bool addToList = false)
        {
            var found = UI.TryGetUIItem(transitionKey, out UIItem prefab);
            if (!found)
            {
                Debug.LogWarning($"[UIRuntime] Transition id '{transitionKey}' was not found, failsafe to default transition.");
                var foundDefault = UI.TryGetUIItem(Settings.UI.Default.Transition.name, out prefab);
                if (!foundDefault)
                {
                    Debug.LogError($"[UIRuntime] There's no default transitions, defaulting to OnComplete");
                    onComplete?.Invoke();
                    return;
                }
            }
        
            ShowTransition(prefab, onComplete, addToList);
        }

		public void ShowTransition(UIItem transitionPrefab, Action callback = null, bool addToList = false)
        {
            if (transitionPrefab == null)
            {
                Debug.LogError($"[UIRuntime] Missing Transition prefab, defaulting to OnComplete");
                callback?.Invoke();
                return;
            }
            var transitionItem = InstantiateUI(transitionPrefab, addToList);
            transitionItem.OnShow += callback;
            transitionItem.AnimatedShow();
		}

        private T InstantiateUI<T>(T prefab, bool addToList = false) where T : MonoBehaviour
        {
            UISectionType sectionType = UISectionType.Undefined;
            if (prefab is UIWindow window)
                sectionType = window.SectionType;

            if (sectionType == UISectionType.Undefined)
                return null;
            
            var isContained = m_canvases.TryGetUISectionByType(sectionType, out var rootContainer);
            var newItem = Instantiate(prefab, isContained ? rootContainer.Canvas.transform : null);

            if (newItem is UIWindow newWindow && addToList)
                m_windowHistory[newWindow.SectionType].Add(newWindow);
            
            return newItem;
        }

        private void CleanUISection<T>(T prefab) where T : MonoBehaviour
        {
            UISectionType sectionType = UISectionType.Undefined;
            if (prefab is UIWindow window)
                sectionType = window.SectionType;

            CleanUISection(sectionType);
        }

        private static void CleanUISection(UISectionType sectionType)
        {
            if (!GameResources.UIRuntime.m_windowHistory.ContainsKey(sectionType))
                return;
            
            for (var i = GameResources.UIRuntime.m_windowHistory[sectionType].Count - 1; i >= 0; i--)
            {
                RemoveWindow(GameResources.UIRuntime.m_windowHistory[sectionType][i]);
            }
        }

        public static void RemoveWindow<T>(T window) where T : UIWindow
        {
            window.OnHide += () =>
            {
                GameResources.UIRuntime.OnWindowRemoved?.Invoke(window);
                GameResources.UIRuntime.SafeDestroy(window.gameObject);
            };
            window.Hide();
        }

        public static bool TryShowWindow<T>(string windowName, out T spawnedWindow, bool addToList = false, Action onFailed = null) where T : UIWindow
        {
            spawnedWindow = null;
            var found = UI.TryGetUIWindow(windowName, out UIWindow window);
            if (!found)
            {
                Debug.LogError($"[UIRuntime] There's no windows of name '{windowName}'");
                onFailed?.Invoke();
                return false;
            }

            spawnedWindow = GameResources.UIRuntime.InstantiateUI(window as T, true);

            if (addToList)
                GameResources.UIRuntime.m_windowHistory[window.SectionType].Add(window);
            
            return true;
        }

        public static bool TryShowGenericError(string errorMessage)
        {
            var valid = TryShowWindow("UIAlert - Error", out UIAlertPopup alertPopup);
            alertPopup.Build(errorMessage);
            alertPopup.AnimatedShow();
            
            return valid;
        }

        public static void NotifyWindowDestroy(UIWindow uiWindow)
        {
            var removed = GameResources.UIRuntime.m_windowHistory[uiWindow.SectionType].Remove(uiWindow);
            if(removed) GameResources.UIRuntime.OnWindowRemoved?.Invoke(uiWindow);
        }

        public bool TryShowGenericMessagePopup(out UIGenericTextMessagePopup messagePopup)
        {
            return TryShowWindow("UIGenericTextMessagePopup", out messagePopup);
        }

        public bool IsReady { get; set; }
    }

	public static class UIRuntimeExtensions
	{
		public static bool TryGetUISectionByType(this List<UIScreenPanelContainer> list, UISectionType type, out UIScreenPanelContainer container)
		{
			container = default;
			foreach (var item in list)
			{
				if (item.Type != type) continue;
				
				container = item;
				return true;
			}

			return false;
		}
	}

	[System.Serializable]
	public struct UIScreenPanelContainer
	{
		public UISectionType Type;
		public Canvas Canvas;
	}

	public enum UISectionType
	{
		Undefined = -1, Background, Default, Popup, Overlay
	}
}