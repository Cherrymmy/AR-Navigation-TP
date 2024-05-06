using UnityEngine;


public abstract class IUimenu : MonoBehaviour
{
    [Header("메뉴 선택")]
    [Tooltip("현재 메뉴")]
    public UIManager.MenuType CurrentMenu;
    [Tooltip("바꾸고 싶은 메뉴")]
    public UIManager.MenuType TargetMenuType;
    

    public abstract void TargetSwitch();
    public abstract void Open();
    public abstract void Close();
}
