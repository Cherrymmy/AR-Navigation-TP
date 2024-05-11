using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace AR
{
    public class DetailController : MonoBehaviour
    {

        //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
        private DetailModel _model;
        private DetailView _view;
        //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-



        private void Start()
        {
            _view = FindAnyObjectByType<DetailView>();
            _model = FindAnyObjectByType<DetailModel>();
            _model.OnDetailSearchComplete.AddListener(ViewChange);
            _view.nextButton.onClick.AddListener(_model.OnDragAbleButton);
        }

        private void ViewChange()
        {
            _view.placename.text = _model.placeDetailsResponse.result.name;
            _view.placeAddress.text = _model.placeDetailsResponse.result.vicinity;
            _view.totalRiviews.text = _model.placeDetailsResponse.result.user_ratings_total.ToString();
            _view.callButton.onClick.AddListener(_model.OnClickCall);               // 버튼 클릭 시 전화
            _view.rating = _model.placeDetailsResponse.result.rating;
            _view.photoLength.text = _model.placeDetailsResponse.result.photos.Length.ToString();

            Debug.Log(_model.texture);
            if (_model.texture != null)
            {
                // 동적으로 텍스처의 크기에 맞는 Rect 생성
                Rect rect = new Rect(0, 0, _model.texture.width, _model.texture.height);
                _view.currentImage.sprite = Sprite.Create(_model.texture, rect, new Vector2(0.5f, 0.5f));
            }
            else
            {
                Debug.LogError("Texture not loaded yet");
            }

            _view.UpdateStarRating(_model.placeDetailsResponse.result.rating);

            UIManager.Instance.LoadingSet = false;
        }
    }
        
}
