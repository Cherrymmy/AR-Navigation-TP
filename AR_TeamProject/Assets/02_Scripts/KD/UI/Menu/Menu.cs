namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// 제네릭 메뉴 클래스. 특정 타입의 메뉴 인스턴스를 싱글턴으로 관리합니다.
    /// </summary>
    /// <typeparam name="T">메뉴 클래스 타입</typeparam>
    public abstract class Menu<T> : Menu where T : Menu<T>
    {
        /// <summary>
        /// 해당 메뉴 타입의 싱글턴 인스턴스입니다.
        /// </summary>
        public static T Instance { get; private set; }

        /// <summary>
        /// 게임 오브젝트가 활성화될 때 호출되며, 싱글턴 인스턴스를 초기화합니다.
        /// </summary>
        protected virtual void Awake()
        {
            Instance = (T)this;
        }

        /// <summary>
        /// 게임 오브젝트가 파괴될 때 호출되며, 싱글턴 인스턴스를 해제합니다.
        /// </summary>
        protected virtual void OnDestroy()
        {
            Instance = null;
        }

        /// <summary>
        /// 메뉴를 열어주는 정적 메서드입니다. 인스턴스가 없으면 새로 생성하고, 있으면 활성화합니다.
        /// </summary>
        protected static void Open()
        {
            GameObject clonedGameObject = null;
            if (Instance == null)
            {
                // 메뉴 인스턴스가 없는 경우, 메뉴 매니저를 통해 인스턴스를 생성하고 메뉴를 엽니다.
                MenuManager.Instance.CreateInstance(typeof(T).Name, out clonedGameObject);
                MenuManager.Instance.OpenMenu(clonedGameObject.GetMenu());
            }
            else
            {
                // 이미 인스턴스가 있는 경우, 해당 인스턴스를 활성화하고 메뉴를 엽니다.
                Instance.gameObject.SetActive(true);
                MenuManager.Instance.OpenMenu(Instance);
            }
        }

        /// <summary>
        /// 메뉴를 닫는 정적 메서드입니다. 인스턴스가 없으면 오류를 로그합니다.
        /// </summary>
        protected static void Close()
        {
            if (Instance == null)
            {
                Debug.LogErrorFormat("메뉴 {0}를 닫으려고 시도했으나 인스턴스가 null입니다.", typeof(T).Name);
                return;
            }

            MenuManager.Instance.CloseMenu(Instance);
        }

        /// <summary>
        /// 뒤로 가기 버튼을 눌렀을 때 메뉴를 닫습니다.
        /// </summary>
        public override void OnBackPressed()
        {
            Close();
        }
    }

    /// <summary>
    /// 모든 메뉴의 기본 클래스입니다. 메뉴 관련 기본 속성과 메서드를 정의합니다.
    /// </summary>
    public abstract class Menu : MonoBehaviour
    {
        [Tooltip("메뉴가 닫힐 때 게임 오브젝트를 파괴합니다 (메모리 사용량 감소)")]
        public bool DestroyWhenClosed = true;

        [Tooltip("이 메뉴 아래에 있는 메뉴들을 비활성화합니다")]
        public bool DisableMenusUnderneath = true;

        /// <summary>
        /// 뒤로 가기 이벤트 처리를 위한 추상 메서드입니다.
        /// </summary>
        public abstract void OnBackPressed();
    }
}
