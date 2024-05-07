using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GPSButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private GoogleMap _googleMap;


    private void Start()
    {
        _googleMap = GameObject.Find("GoogleMap").GetComponent<GoogleMap>();   
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("Touch on GPS Button Down");
        _googleMap.IsGPSOn = true;
        _googleMap.markerPosition = _googleMap.markerInitPosition;
        _googleMap.IsGPSButtonClick = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("Touch on GPS Button Up");
        _googleMap.IsGPSOn = true;
        _googleMap.markerPosition = _googleMap.markerInitPosition;
        _googleMap.IsGPSButtonClick = false;
    }
}
