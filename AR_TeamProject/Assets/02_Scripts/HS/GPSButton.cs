using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GPSButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private GoogleMap _googleMap;
    private Image[] _markers = new Image[2];

    private void Start()
    {
        _markers[0] = GameObject.Find("RawImage - StaticMap").transform.Find("Image - Marker").GetComponent<Image>();
        _markers[1] = GameObject.Find("RawImage - NaviMap").transform.Find("Image - Marker").GetComponent<Image>();
        _googleMap = GameObject.Find("GoogleMap").GetComponent<GoogleMap>();   
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("Touch on GPS Button Down");
        _googleMap.IsGPSOn = true;

        foreach (var marker in _markers)
        {
            marker.rectTransform.anchoredPosition = _googleMap.markerInitPosition;
        }
        
        //_googleMap.markerPosition = _googleMap.markerInitPosition;
        _googleMap.IsGPSButtonClick = true;
        _googleMap.DragInitGPSLat = 0f;
        _googleMap.DragInitGPSLon = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("Touch on GPS Button Up");
        _googleMap.IsGPSOn = true;

        foreach (var marker in _markers)
        {
            marker.rectTransform.anchoredPosition = _googleMap.markerInitPosition;
        }
        //_googleMap.markerPosition = _googleMap.markerInitPosition;
        _googleMap.IsGPSButtonClick = false;
    }
}
