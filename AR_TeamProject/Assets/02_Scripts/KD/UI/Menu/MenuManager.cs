using System.Collections.Generic;
using System.Reflection;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// 게임 내 다양한 메뉴를 관리하는 클래스로, 메뉴의 생성, 열기, 닫기를 담당합니다.
    /// </summary>
    public class MenuManager : MonoBehaviour
    {
        [SerializeField]
        private Menu[] menuScreens; // 관리할 모든 메뉴 스크린을 저장하는 배열입니다.

        public Menu[] MenuScreens
        {
            get { return menuScreens; }
            set { menuScreens = value; }
        }

        [SerializeField]
        private int startScreen = 0; // 게임 시작 시 표시할 초기 화면의 인덱스입니다.

        public int StartScreen
        {
            get { return startScreen; }
            set { startScreen = value; }
        }

        // 메뉴를 LIFO (후입선출) 순서로 관리하기 위해 사용되는 스택입니다.
        private Stack<Menu> menuStack = new Stack<Menu>();

        public static MenuManager Instance { get; set; } // MenuManager의 싱글톤 인스턴스입니다.

        private void Start()
        {
            Instance = this;
            // 게임 시작 시 시작 메뉴를 엽니다.
            if (MenuScreens.Length > StartScreen)
            {
                var startMenu = CreateInstance(MenuScreens[StartScreen].name);
                OpenMenu(startMenu.GetMenu());
            }
            else
            {
                Debug.LogError("구성된 메뉴 스크린이 충분하지 않습니다.");
            }
        }

        private void OnDestroy()
        {
            Instance = null; // 인스턴스를 null로 초기화하여 다른 오브젝트와의 참조를 끊습니다.
        }

        /// <summary>
        /// 메뉴 이름을 기반으로 새 메뉴 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="MenuName">생성할 메뉴의 이름입니다.</param>
        /// <returns>생성된 게임 오브젝트입니다.</returns>
        public GameObject CreateInstance(string MenuName)
        {
            var prefab = GetPrefab(MenuName); // 이름에 해당하는 프리팹을 찾습니다.

            return Instantiate(prefab, transform); // 프리팹을 인스턴스화하여 반환합니다.
        }

        public void CreateInstance(string MenuName, out GameObject menuInstance)
        {
            var prefab = GetPrefab(MenuName);

            menuInstance = Instantiate(prefab, transform); // 프리팹을 인스턴스화하여 외부 변수에 할당합니다.
        }

        public void OpenMenu(Menu menuInstance)
        {
            // 가장 상위 메뉴를 비활성화합니다.
            if (menuStack.Count > 0)
            {
                if (menuInstance.DisableMenusUnderneath)
                {
                    foreach (var menu in menuStack)
                    {
                        menu.gameObject.SetActive(false); // 메뉴를 숨깁니다.

                        if (menu.DisableMenusUnderneath)
                            break; // 추가 메뉴 숨기기를 중단합니다.
                    }
                }

                Canvas topCanvas = menuInstance.GetComponent<Canvas>();
                if (topCanvas != null)
                {
                    Canvas previousCanvas = menuStack.Peek().GetComponent<Canvas>();

                    if (previousCanvas != null)
                    {
                        topCanvas.sortingOrder = previousCanvas.sortingOrder + 1; // 캔버스 순서를 조정합니다.
                    }
                }

            }

            menuStack.Push(menuInstance); // 스택에 새 메뉴를 추가합니다.
        }

        private GameObject GetPrefab(string PrefabName)
        {
            for (int i = 0; i < MenuScreens.Length; i++)
            {
                if (MenuScreens[i].name == PrefabName)
                {
                    return MenuScreens[i].gameObject; // 이름에 해당하는 게임 오브젝트를 반환합니다.
                }
            }
            throw new MissingReferenceException("해당 이름의 프리팹을 찾을 수 없습니다: " + PrefabName);
        }

        public void CloseMenu(Menu menu)
        {
            if (menuStack.Count == 0)
            {
                Debug.LogErrorFormat(menu, "{0} 메뉴를 닫을 수 없습니다. 메뉴 스택이 비어 있습니다.", menu.GetType());
                return;
            }

            if (menuStack.Peek() != menu)
            {
                Debug.LogErrorFormat(menu, "{0} 메뉴를 닫을 수 없습니다. 메뉴가 스택의 최상위에 있지 않습니다.", menu.GetType());
                return;
            }

            CloseTopMenu(); // 최상위 메뉴를 닫습니다.
        }

        public void CloseTopMenu()
        {
            var menuInstance = menuStack.Pop(); // 스택에서 메뉴 인스턴스를 제거합니다.

            if (menuInstance.DestroyWhenClosed)
                Destroy(menuInstance.gameObject); // 메뉴를 파괴합니다.
            else
                menuInstance.gameObject.SetActive(false); // 메뉴를 비활성화합니다.

            // 상위 메뉴를 재활성화합니다.
            foreach (var menu in menuStack)
            {
                menu.gameObject.SetActive(true); // 메뉴를 활성화합니다.

                if (menu.DisableMenusUnderneath)
                    break; // 추가 메뉴 활성화를 중단합니다.
            }
        }

        private void Update()
        {
            // Android에서 뒤로 가기 버튼이 Esc로 전송됩니다.
            if (UIExtensionsInputManager.GetKeyDown(KeyCode.Escape) && menuStack.Count > 0)
            {
                menuStack.Peek().OnBackPressed(); // 스택의 최상위 메뉴의 뒤로 가기 이벤트를 처리합니다.
            }
        }
    }

    public static class MenuExtensions
    {
        /// <summary>
        /// 게임 오브젝트에서 Menu 컴포넌트를 추출합니다.
        /// </summary>
        /// <param name="go">Menu 컴포넌트를 추출할 게임 오브젝트입니다.</param>
        /// <returns>추출된 Menu 컴포넌트입니다.</returns>
        public static Menu GetMenu(this GameObject go)
        {
            return go.GetComponent<Menu>();
        }
    }

}
