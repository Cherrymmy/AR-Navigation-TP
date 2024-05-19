using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ARView : IUimenu
{
    public Button naviChange;

    private GoogleMap _googleMap;
    private NaviMapRenderer _naviMapRenderer;
    private MiniMapRenderer _miniMapRenderer;

    #region Swich 
    public override void Close()
    {
        
    }

    public override void Open()
    {
        TargetSwitch();
        OnDragAbleButton();
        SceneManager.LoadSceneAsync(1);
        UIManager.Instance.LoadingSet = false;
    }

    public override void TargetSwitch()
    {
        UIManager.Instance.Switch(CurrentMenu, TargetMenuType);
    }
    
    #endregion


    void Start()
    {
        _googleMap = GameObject.Find("GoogleMap").GetComponent<GoogleMap>();
        _naviMapRenderer = GameObject.Find("RawImage - NaviMap").GetComponent<NaviMapRenderer>();
        _miniMapRenderer = GameObject.Find("RawImage - MiniMap").GetComponent<MiniMapRenderer>();


        naviChange.onClick.AddListener(Open);
    }

    private void OnDragAbleButton()
    {
        _googleMap.IsDragZoomDisable = false;
        _naviMapRenderer.enabled = true;
        _miniMapRenderer.enabled = false;
    }
}
