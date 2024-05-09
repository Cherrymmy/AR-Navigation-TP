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
    private new ParticleSystem particleSystem;
    private Coroutine balloonCoroutine;

    // 랜덤 텍스트 
    private string[] randomMessages = new string[]
    {
        "안녕하세요!",
        "길 안내를 해줄게요!",
        "저는 길 안내 Pet 이에요!",
        "반가워요!"
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

        // 오브젝트가 존재하면 항상 위치를 업데이트
        if (placeObjectInstance != null)
        {
            UpdateObjectMovement(screenCenter);
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                AttemptToPlaceObject(touch.position);
            }
        }
    }

    void AttemptToPlaceObject(Vector2 touchPosition)
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
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

    void UpdateObjectMovement(Vector2 screenCenter)
    {
        if (raycastManager.Raycast(screenCenter, raycastHits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = raycastHits[0].pose;
            ARPlane plane = raycastHits[0].trackable as ARPlane;
            if (plane != null)
            {
                UpdateObjectPositionAndFollowUser(hitPose.position, plane.center.y);
            }
        }
    }


    void SetupBalloonComponents()
    {
        Canvas canvas = placeObjectInstance.GetComponentInChildren<Canvas>(true);
        if (canvas)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            balloonImage = canvas.GetComponentInChildren<Image>(true);
            balloonText = canvas.GetComponentInChildren<TextMeshProUGUI>(true);
            particleSystem = canvas.GetComponentInChildren<ParticleSystem>(true);
        }

        if (balloonImage == null || balloonText == null)
        {
            Debug.LogError("Balloon components not found.");
            return;
        }

        balloonImage.gameObject.SetActive(false); // 초기 상태는 비활성화
        particleSystem.Stop();
    }

    void ShowBalloon()
    {
        int randomIndex = Random.Range(0, randomMessages.Length);
        balloonText.text = randomMessages[randomIndex];
        balloonImage.gameObject.SetActive(true);
        particleSystem.Play(); // 파티클 시스템 활성화

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
        particleSystem.Stop(); // 파티클 시스템 비활성화
    }

    void UpdateObjectPositionAndFollowUser(Vector3 targetPosition, float planeHeight)
    {
        if (raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), raycastHits, TrackableType.PlaneWithinPolygon))
        {
            ARRaycastHit hit = raycastHits[0];
            Vector3 newPosition = new Vector3(targetPosition.x, hit.pose.position.y + 0.1f, targetPosition.z);

            // 객체의 위치를 부드럽게 조정
            placeObjectInstance.transform.position = Vector3.Lerp(placeObjectInstance.transform.position, newPosition, Time.deltaTime * 2);

            // 스케일 조정
            AdjustScaleBasedOnDistance(placeObjectInstance.transform.position, Camera.main.transform.position);

            // 이동 여부에 따라 애니메이션 상태 업데이트
            bool isMoving = Vector3.Distance(placeObjectInstance.transform.position, newPosition) > 0.05f;
            placeObjectAnimator.SetBool("isWalking", isMoving);

            Quaternion targetRotation = Quaternion.LookRotation(arCameraManager.transform.forward);
            // placeObjectInstance의 회전을 현재 회전에서 targetRotation으로 시간에 따라 부드럽게 조정합니다.
            placeObjectInstance.transform.rotation = Quaternion.Slerp(placeObjectInstance.transform.rotation, targetRotation, Time.deltaTime * 5);

        }
        else
        {
            // 평면 감지에 실패한 경우, 이동 및 회전 중지
            placeObjectAnimator.SetBool("isWalking", false);
        }
    }

    void AdjustScaleBasedOnDistance(Vector3 objectPosition, Vector3 cameraPosition)
    {
        float distance = Vector3.Distance(cameraPosition, objectPosition);
        float scale = Mathf.Clamp(distance / 5.0f, 0.1f, 2.0f); // 거리에 따른 스케일 조정. '5.0f'는 조정 계수입니다.
        placeObjectInstance.transform.localScale = new Vector3(scale, scale, scale);
    }

}
