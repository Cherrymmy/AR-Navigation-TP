using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Compass : MonoBehaviour
{
    public float magneticHeading;   // �ڱ���� �� ����
    public float trueHeaing;        // ��ġ�� ���� ���Ⱚ

    public GameObject compass;      // ��ħ�� �̹���
    private float _rotationSpeed = 10f;


    private void OnEnable()
    {
        Input.compass.enabled = true;
        Input.location.Start();         // ���� ������ ���Ⱚ ���
    }

    private void OnDisable()
    {
        Input.compass.enabled = false;
        Input.location.Stop();          // ���Ⱚ ��� ����
    }

    private void LateUpdate()
    {
        // ��ħ���� ��Ȯ���� ��ȿ���� Ȯ��
        if (Input.compass.headingAccuracy == 0 || Input.compass.headingAccuracy > 0)
        {
            magneticHeading = Input.compass.magneticHeading;    // �ڽ��� ������ ������ ���
            trueHeaing = Input.compass.trueHeading;             // ���� ������ ������ ������ ���
        }

        // �������� �������� 
        if (trueHeaing != 0)
        {
            compass.transform.rotation = Quaternion.Slerp(compass.transform.rotation, Quaternion.Euler(0f, 0f, -trueHeaing), Time.deltaTime * _rotationSpeed);  // ���� ���� ������ ����Ű���� ��
        }
    }
 }
