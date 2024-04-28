using UnityEngine;


public abstract class IUimenu : MonoBehaviour
{
    [Tooltip("메뉴")]
    public UIManager.MenuType MenuType;
    [Tooltip("바꾸고 싶은 메뉴")]
    public UIManager.MenuType TargetMenuType;

    public abstract void TargetSwitch();
    public abstract void Open();
    public abstract void Close();
}
