using System;
using System.Collections.Generic;
using AR;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DestinationNotify : MonoBehaviour
{
    // data manager, 위치정보를 가지고 있는 싱글톤
    public DataManager dataManager;
    // 도착 팝업 이미지, 인스펙터창에서 넣어줘도 되고 그냥 find 해도 됨 맘에드는 이미지로 연결
    public GameObject arrival_popup;
    public bool isFirst = false;

    // 도착지 위도
    private float lats;
    // 도착지 경도
    private float lons;

    private void Start()
    {
        dataManager = DataManager.Instance;

        if (dataManager == null)
        {
            Debug.LogError("DataManager 인스턴스를 찾을 수 없습니다.");
        }
    }

    void Update()
    {       
        // GPS 연결되어있을 때 
        if (Input.location.status == LocationServiceStatus.Running)
        {
            // 도착지 위도, 경도가 정해져있을 때 (0일 때는 포함 x)
            if (lats != 0 && lons != 0)
            {
                //현재 내 위치 위도, 경도 업데이트문에서 받아오기
                double myLat = Input.location.lastData.latitude;
                double myLong = Input.location.lastData.longitude;

                // 도착까지 남은 거리 계산
                double remainDistance = distance(myLat, myLong, lats, lons);

                if (remainDistance <= 15f) // 20m 반경 이내에 도착지가 있다면 
                {
                    if (!isFirst)
                    {
                        // 도착을 표시할 불값
                        isFirst = true;
                        // 팝업 표시
                        arrival_popup.SetActive(true);
                    }
                }
            }
        }
    }

    // 도착지 위도, 경도 입력 메서드
    public void AddDestination()
    {
        // 도착지 위도 경도 받아와서 넣어주기
        lats = dataManager.detailResponse.result.geometry.location.lat;
        lons = dataManager.detailResponse.result.geometry.location.lng;

        //lats = 37.71432696f;
        //lons = 126.74255351f;
    }

    // 지표면 거리 계산 공식(하버사인 공식) 남은 거리 계산 공식.
    private double distance(double lat1, double lon1, double lat2, double lon2)
    {
        double theta = lon1 - lon2;

        double dist = Math.Sin(Deg2Rad(lat1)) * Math.Sin(Deg2Rad(lat2)) + Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) * Math.Cos(Deg2Rad(theta));

        dist = Math.Acos(dist);

        dist = Rad2Deg(dist);

        dist = dist * 60 * 1.1515;

        dist = dist * 1609.344; // 미터 변환

        return dist;
    }

    private double Deg2Rad(double deg)
    {
        return (deg * Mathf.PI / 180.0f);
    }

    private double Rad2Deg(double rad)
    {
        return (rad * 180.0f / Mathf.PI);
    }
}
