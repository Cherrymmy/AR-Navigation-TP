using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TouchManager : MonoBehaviour
{
    [SerializeField] private GameObject placeObjectPrefab; // AR에 배치할 객체의 프리팹
    private GameObject placeObjectInstance; // 배치된 객체의 인스턴스
    private Animator placeObjectAnimator; // 배치된 객체의 애니메이터 컴포넌트
    private ARRaycastManager raycastManager; // AR Raycast 매니저
    private List<ARRaycastHit> raycastHits = new List<ARRaycastHit>(); // Raycast 충돌 정보를 담을 리스트
    private ARCameraManager arCameraManager; // AR 카메라 관리를 위한 컴포넌트

    private Image balloonImage; // UI 말풍선 이미지
    private TextMeshProUGUI balloonText; // 말풍선 내의 텍스트 컴포넌트
    private Coroutine balloonCoroutine;

    // 랜덤 텍스트 
    private string[] randomMessages = new string[]
    {
        "안녕하세요!",
        "저를 따라오세요!",
        "잘 오고있나요?",
        "거의 다 왔어요!!",
        "도착!"
    };

    void Start()
    {
        arCameraManager = FindObjectOfType<ARCameraManager>();
        raycastManager = FindObjectOfType<ARRaycastManager>();
        if (arCameraManager == null || raycastManager == null)
        {
            Debug.LogError("AR 관련 컴포넌트를 찾을 수 없습니다.");
        }
    }

    void Update()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;
                    if (placeObjectInstance == null)
                    {
                        placeObjectInstance = Instantiate(placeObjectPrefab, hitPose.position, Quaternion.identity);
                        placeObjectInstance.AddComponent<BoxCollider>();
                        placeObjectAnimator = placeObjectInstance.GetComponent<Animator>();
                        SetupBalloonComponents();
                    }
                    else
                    {
                        placeObjectAnimator.SetTrigger("jump");
                        ShowBalloon();
                    }
                }
            }
        }

        if (placeObjectInstance != null && raycastManager.Raycast(screenCenter, raycastHits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = raycastHits[0].pose;
            UpdateObjectPositionAndFollowUser(hitPose.position);
        }
    }

    /// <summary>
    /// Canvas children 가져옴 
    /// </summary>
    void SetupBalloonComponents()
    {
        Canvas canvas = placeObjectInstance.GetComponentInChildren<Canvas>(true);
        if (canvas)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            balloonImage = canvas.GetComponentInChildren<Image>(true);
            balloonText = canvas.GetComponentInChildren<TextMeshProUGUI>(true);
        }

        if (balloonImage == null || balloonText == null)
        {
            Debug.LogError("Balloon components not found.");
            return;
        }

        balloonImage.gameObject.SetActive(false); // 초기 상태는 비활성화
    }

    /// <summary>
    /// 말풍선에 랜덤 텍스트 띄우고 3초 뒤 사라지게 함
    /// </summary>
    void ShowBalloon()
    {
        int randomIndex = Random.Range(0, randomMessages.Length);
        balloonText.text = randomMessages[randomIndex];
        balloonImage.gameObject.SetActive(true);

        if (balloonCoroutine != null)
        {
            StopCoroutine(balloonCoroutine);
        }
        balloonCoroutine = StartCoroutine(HideBalloonAfterSeconds(3.0f));
    }

    IEnumerator HideBalloonAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        balloonImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// object가 사용자를 따라갈 때의 위치랑 방향조정하고 이동 여부로 애니메이션 상태 설정함
    /// </summary>
    /// <param name="targetPosition"></param>
    void UpdateObjectPositionAndFollowUser(Vector3 targetPosition)
    {
        if (!raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), raycastHits, TrackableType.PlaneWithinPolygon))
        {
            return;
        }

        Vector3 newPosition = new Vector3(targetPosition.x, targetPosition.y + 0.1f, targetPosition.z);
        placeObjectInstance.transform.position = Vector3.Lerp(placeObjectInstance.transform.position, newPosition, Time.deltaTime * 2);

        bool isMoving = Vector3.Distance(placeObjectInstance.transform.position, newPosition) > 0.05f;
        placeObjectAnimator.SetBool("isWalking", isMoving);

        Quaternion targetRotation = Quaternion.LookRotation(arCameraManager.transform.forward);
        placeObjectInstance.transform.rotation = Quaternion.Slerp(placeObjectInstance.transform.rotation, targetRotation, Time.deltaTime * 5);
    }
}
