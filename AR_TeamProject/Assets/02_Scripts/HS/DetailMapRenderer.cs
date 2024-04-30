using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailMapRenderer : MonoBehaviour, IStaticMapObserver
{
    private GoogleMap _mapData;


    private void OnEnable()
    {
        _mapData.ResisterStaticMapObserver(this);
    }

    private void OnDisable()
    {
        _mapData.RemoveStaticMapObserver(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateData(float lat, float lon, int zoom)
    {
        throw new System.NotImplementedException();
    }
}
