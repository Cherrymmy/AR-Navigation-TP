using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Compass : MonoBehaviour
{
    public float magneticHeading;   // 자기방향 값 저장
    public float trueHeaing;        // 장치의 실제 방향값

    public GameObject compass;      // 나침반 이미지
    private float _rotationSpeed = 10f;


    private void OnEnable()
    {
        Input.compass.enabled = true;
        Input.location.Start();         // 실제 세계의 방향값 얻기
    }

    private void OnDisable()
    {
        Input.compass.enabled = false;
        Input.location.Stop();          // 방향값 얻기 끄기
    }

    private void LateUpdate()
    {
        // 나침반의 정확도가 유효한지 확인
        if (Input.compass.headingAccuracy == 0 || Input.compass.headingAccuracy > 0)
        {
            magneticHeading = Input.compass.magneticHeading;    // 자신의 방향을 변수로 담기
            trueHeaing = Input.compass.trueHeading;             // 실제 세계의 방향을 변수로 담기
        }

        // 움직임이 있을때만 
        if (trueHeaing != 0)
        {
            compass.transform.rotation = Quaternion.Slerp(compass.transform.rotation, Quaternion.Euler(0f, 0f, -trueHeaing), Time.deltaTime * _rotationSpeed);  // 실제 세계 방향을 가리키도록 함
        }
    }
 }
