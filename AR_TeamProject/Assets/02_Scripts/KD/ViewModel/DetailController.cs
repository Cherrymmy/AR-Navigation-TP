using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        }

        private void ViewChange()
        {
            _view.placename.text = _model.placeDetailsResponse.result.name;
            _view.placeAddress.text = _model.placeDetailsResponse.result.vicinity;
            //_view.totalRiviews.text = _model.placeDetailsResponse.result.
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
        }
        
    }
}
