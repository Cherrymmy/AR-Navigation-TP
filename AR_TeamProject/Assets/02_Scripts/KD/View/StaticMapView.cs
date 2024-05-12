using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaticMapView : IUimenu
{
    [Space]
    [Header("UI")]
    [Tooltip("Canvas 바꾸기")]
    public Button joinButton;

    void Start()
    {
        joinButton.onClick.AddListener(delegate { Open(); OnDragDisableButton(); });
    }

    public override void Close()
    {
        throw new System.NotImplementedException();
    }

    public override void Open()
    {
        TargetSwitch();
    }

    public override void TargetSwitch()
    {
        UIManager.Instance.Switch(CurrentMenu, TargetMenuType);
        UIManager.Instance.LoadingSet = false;
    }

    private void OnDragDisableButton()
    {
        GoogleMap googleMap = GameObject.Find("GoogleMap").GetComponent<GoogleMap>();

        if (googleMap != null)
        {
            googleMap.IsDragZoomDisable = true;
        }
    }
}
