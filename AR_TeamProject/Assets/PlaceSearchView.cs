using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlaceSearchView : IUimenu
{
    [Tooltip("바꾸고 싶은 메뉴2")]
    public UIManager.MenuType TargetMeun2Type;

    public TMP_InputField searchText;
    private void Start()
    {
        //searchText.onValueChanged.AddListener();
        searchText.onSelect.AddListener(delegate { Select();});
    }

    public override void Close()
    {

    }

    public override void Open()
    {

    }

    public override void TargetSwitch()
    {
        UIManager.Instance.Switch(MenuType, TargetMenuType);
    }

    public void TargetSwitch2Meun()
    {
        UIManager.Instance.Switch(MenuType, TargetMeun2Type);
    }

    public void Select()
    {
        TargetSwitch();
    }
}
