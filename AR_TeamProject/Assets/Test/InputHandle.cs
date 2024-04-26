using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputHandle : MonoBehaviour
{
    Vector2 touchMark;
    TMP_Text touchMarkLog;

    private void Awake()
    {
        touchMarkLog = transform.Find("Panel/Text (TMP)").GetComponent<TMP_Text>();
    }
    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            Debug.Log(touchMark - (Vector2)Input.mousePosition);
            touchMark = Input.mousePosition;
            touchMarkLog.text = touchMark.ToString();
        }
#else
        if (Input.touchCount > 0)
        {
            Debug.Log(touchMark - Input.GetTouch(0).position);
            touchMark = Input.GetTouch(0).position;
            touchMarkLog.text = touchMark.ToString();
        }
#endif
    }
}
