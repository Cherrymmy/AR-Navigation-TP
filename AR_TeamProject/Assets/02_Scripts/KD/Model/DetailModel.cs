using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using static AR.DataManager;


namespace AR
{

    public class DetailModel : MonoBehaviour
    {

        string apiKey = "AIzaSyCsyqqXiR26jn_xlk5UTmDdKdKqLoHyw1U";
        public PlaceDetailsResponse placeDetailsResponse { get; set; }

        public UnityEvent OnDetailSearchComplete;
        public Texture2D texture;


        public void Toss(string name)
        {
            Details(name);
        }
        
        public void ReToss(string name)
        {

            ReDetails(name);
        }

        // search 값에 있으면 여기서 돌고
        private void Details(string placesId)
        {
                // 검색기록 저장하기
                string fields = "name,photos,geometry,vicinity,editorial_summary,reviews,user_ratings_total,type,formatted_phone_number";
                string placeDetailsUrl = $"https://maps.googleapis.com/maps/api/place/details/json?placeid={placesId}&fields={fields}&key={apiKey}&language=ko";
                // Place Detail 경도 위도 받기
                StartCoroutine(PlaceDetails(placeDetailsUrl));
        }

        // 검색 기록에 있으면 여기 실행함 (DataMager)
        private void ReDetails(string name)
        { 

            foreach (var data in DataManager.Instance.jsonDatas.datas)
            {
                if (data.Name == name)
                {
                    Debug.Log("ReDetails" + name);
                    //              이름 , 사진 , 경도위도 , 주소 , 상세 설명 ,리뷰 , 총리뷰 , 장소 타입 , 전화번호
                    string fields = "name,photos,geometry,vicinity,editorial_summary,reviews,user_ratings_total,type,formatted_phone_number";
                    string placeDetailsUrl = $"https://maps.googleapis.com/maps/api/place/details/json?placeid={data.PlaceId}&fields={fields}&key={apiKey}&language=ko";
                    StartCoroutine(PlaceDetails(placeDetailsUrl));
                    break;
                }
            }
        }

        private IEnumerator PlaceDetails(string url)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.LogError("상세 정보를 가져오는 중 오류 발생: " + request.error);
                }
                else
                {
                    Debug.Log("응답: " + request.downloadHandler.text);
                    placeDetailsResponse = JsonUtility.FromJson<PlaceDetailsResponse>(request.downloadHandler.text);

                    //Debug.Log(placeDetailsResponse.result.photos[0].photo_reference);
                    StartCoroutine(LoadImageFromPhoto(placeDetailsResponse.result.photos[0]));
                    OnDetailSearchComplete.Invoke();
                }
            }
        }

        public IEnumerator LoadImageFromPhoto(Photo photo)
        {
            string imageUrl = GetImageUrl(photo.photo_reference);  // 이미지 URL 구성 함수, 구현 필요
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("이미지 로드 실패: " + request.error);
                    UIManager.Instance.LoadingSet = false;
                }
                else
                {
                    texture = DownloadHandlerTexture.GetContent(request);
                    // uiImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    UIManager.Instance.LoadingSet = false;
                }
            }
        }

        private string GetImageUrl(string photoReference)
        {
            return $"https://maps.googleapis.com/maps/api/place/photo?maxwidth=200&photoreference={photoReference}&key={apiKey}";
        }

        public void OnClickCall()
        {
            string phoneNumber = placeDetailsResponse.result.formattedPhoneNumber;
            string url = "tel:" + phoneNumber;

            Application.OpenURL(url);
        }
    }
}

