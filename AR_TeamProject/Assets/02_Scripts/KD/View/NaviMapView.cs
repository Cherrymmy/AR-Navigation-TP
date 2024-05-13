using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NaviMapView : IUimenu
{
    [Tooltip("바꾸고 싶은 메뉴2")]
    public UIManager.MenuType TargetMeun2Type;
    [Space]
    public Button exitButton;
    public Button ARChange;

    #region Swich 
    public override void Close()
    {
        TargetSwitch2Meun();
        UIManager.Instance.LoadingSet = false;
    }

    public override void Open()
    {
        TargetSwitch();
        UIManager.Instance.LoadingSet = false;

        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            StartCoroutine(CameraAR());
        else
            SceneManager.LoadSceneAsync(2);
    }

    public override void TargetSwitch()
    {
        UIManager.Instance.Switch(CurrentMenu, TargetMenuType);
    }

    public void TargetSwitch2Meun()
    {
        UIManager.Instance.Switch(CurrentMenu, TargetMeun2Type);
    }
    #endregion

    private void Start()
    {
        exitButton.onClick.AddListener(Close);
        // AR 씬 로드 하는거 여기에 
        ARChange.onClick.AddListener(Open);
        ARChange.onClick.AddListener(OnDragDisableButton);
    }

    IEnumerator CameraAR()
    {
        // 카메라 허가를 받지 못했다면, 권한 허가 팝업을 띄움
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
            yield return null;

            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Permission.RequestUserPermission(Permission.Camera);
#if UNITY_ANDROID
                //OpenAppSettings();
#endif
            }
        }
    }

#if UNITY_ANDROID
    void OpenAppSettings()
    {
        // 설정 화면으로 유도하는 대화상자 표시
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager");
            AndroidJavaObject launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", Application.identifier);
            currentActivity.Call("startActivity", launchIntent);

            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", Application.identifier, null);
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject);

            intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
            intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
            currentActivity.Call("startActivity", intentObject);
        }
    }
#endif

    private void OnDragDisableButton()
    {
        GoogleMap googleMap = GameObject.Find("GoogleMap").GetComponent<GoogleMap>();

        if (googleMap != null)
        {
            googleMap.IsDragZoomDisable = true;
        }
    }
}