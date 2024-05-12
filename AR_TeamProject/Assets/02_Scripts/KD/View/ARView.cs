using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ARView : IUimenu
{
    public Button naviChange;

    #region Swich 
    public override void Close()
    {
        
    }

    public override void Open()
    {
        TargetSwitch();
        UIManager.Instance.LoadingSet = false;
        OnDragAbleButton();
        SceneManager.LoadScene(1);
    }

    public override void TargetSwitch()
    {
        UIManager.Instance.Switch(CurrentMenu, TargetMenuType);
    }
    
    #endregion


    void Start()
    {
        naviChange.onClick.AddListener(Open);
    }

    private void OnDragAbleButton()
    {
        GoogleMap googleMap = GameObject.Find("GoogleMap").GetComponent<GoogleMap>();

        if (googleMap != null)
        {
            googleMap.IsDragZoomDisable = false;
        }
    }
}
