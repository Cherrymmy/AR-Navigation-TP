using Google.XR.ARCoreExtensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GeospatialManager : MonoBehaviour
{
    [Header("Core Features")]
    [SerializeField]
    private TextMeshProUGUI geospatialStatusText;

    [SerializeField]
    private AREarthManager earthManager;

    [SerializeField]
    private ARCoreExtensions arCoreExtensions;

    private bool waitingForLocationService = false;

    private Coroutine locationServiceLauncher;


    ARGeospatialAnchor anchor;
    

    private void Awake()
    {
        Application.targetFrameRate = 60;
        
    }

    void Update()
    {
        Debug.Log("begin update");
        if(!Debug.isDebugBuild || earthManager == null)
        {
            Debug.Log("!Debug.isDebugBuild || earthManager == null");
            return;
        }
          

        if(ARSession.state != ARSessionState.SessionInitializing && ARSession.state != ARSessionState.SessionTracking)
        {
            Debug.Log("ARSession.state != ARSessionState.SessionInitializing && ARSession.state != ARSessionState.SessionTracking");
            return;
        }
            
        var featureSupport = earthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);

        switch (featureSupport)
        {
            case FeatureSupported.Unknown:
                break;
            case FeatureSupported.Unsupported:
                break;
            case FeatureSupported.Supported:
                if(arCoreExtensions.ARCoreExtensionsConfig.GeospatialMode == GeospatialMode.Disabled)
                {
                    arCoreExtensions.ARCoreExtensionsConfig.GeospatialMode = GeospatialMode.Enabled;

                    arCoreExtensions.ARCoreExtensionsConfig.StreetscapeGeometryMode = StreetscapeGeometryMode.Enabled;
                }
                break;

            default:
                break;
        }

        var pose = earthManager.EarthState == EarthState.Enabled &&
            earthManager.EarthTrackingState == TrackingState.Tracking ? earthManager.CameraGeospatialPose : new GeospatialPose();

        var supported = earthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);

        if(geospatialStatusText != null)
        {
            geospatialStatusText.text =
                $"SessionState : {ARSession.state}\n" +
                $"LocationServiceStatus : {Input.location.status}\n" +
                $"FeatureSupported : {supported}\n" +
                $"EarthState : {earthManager.EarthState}\n" +
                $"EarthTrackingState : {earthManager.EarthTrackingState}\n" +
                $"LAT/LNG : {pose.Latitude:F6}, {pose.Longitude:F6}\n" +
                $"HorizontalAcc : {pose.HorizontalAccuracy:F6}\n" +
                $"ALT : {pose.Altitude:F2}\n" +
                $"VerticalAcc : {pose.VerticalAccuracy:F2}\n" +
                $"EunRotation : {pose.EunRotation:F2}\n" +
                $"OrientalYawAcc : {pose.OrientationYawAccuracy:F2}\n";
        }

        Debug.Log("end update");
    }
    private void OnEnable()
    {
        locationServiceLauncher = StartCoroutine(StartLocationService());
    }

    private void OnDisable()
    {
        if (locationServiceLauncher != null)
            StopCoroutine(locationServiceLauncher);

        locationServiceLauncher = null;
        Input.location.Stop();
    }

    private IEnumerator StartLocationService()
    {
        waitingForLocationService = true;

#if UNITY_ANDROID
        if(!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            yield return new WaitForSeconds(3f);
        }
#endif

        if(!Input.location.isEnabledByUser)
        {
            Debug.Log("!Input.location.isEnabledByUser");
            waitingForLocationService = false;
            yield break;
        }

        Input.location.Start();

        while(Input.location.status == LocationServiceStatus.Initializing)
        {
            yield return null;
        }

        waitingForLocationService = false;

        if(Input.location.status != LocationServiceStatus.Running)
        {
            Input.location.Stop();
        }
    }
}
