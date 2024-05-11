using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    private Slider slider;
    private float time;
    private string SceneName = "StaticMap";

    void Start()
    {
        slider = GameObject.Find("LoadingBar").GetComponent<Slider>();
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        // AsyncOperation을 통해서 비동기 작업 진행, 여기서는 비동기적으로 씬 로드
        AsyncOperation async = SceneManager.LoadSceneAsync(1);

        // 씬 인덱스가 아니라 씬 이름으로 할 시 이 줄을 쓰고 SceneName만 수정해주면 됨
        // AsyncOperation async = SceneManager.LoadSceneAsync(SceneName); 


        // 씬 비활성화
        async.allowSceneActivation = false;

        // 비동기화 작업이 완료되지 않는 동안
        while (!async.isDone)
        {
            yield return null;

            time += Time.deltaTime;

            // 비동기 작업이 90% 진행되기 전까지
            if (async.progress < 0.9f)
            {
                // 슬라이더의 값을 진행도까지 시간에 따라 선형보간 작업
                slider.value = Mathf.Lerp(slider.value, async.progress, time);

                // 90%이상 작업되었다면
                if (slider.value >= async.progress)
                {
                    // 시간값 초기화
                    time = 0.0f;
                }
            }
            else
            {
                // 슬라이더의 값을 100%까지 시간에 따라 선형보간 작업
                slider.value = Mathf.Lerp(slider.value, 1f, time);

                // 작업이 100% 완료 되었다면
                if (slider.value == 1f)
                {
                    // 씬 활성화
                    async.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
