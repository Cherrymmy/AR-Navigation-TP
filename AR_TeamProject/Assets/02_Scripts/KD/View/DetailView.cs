using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;
using System;

namespace AR
{
    public class DetailView : IUimenu
    {
        [Tooltip("바꾸고 싶은 메뉴2")]
        public UIManager.MenuType TargetMeun2Type;
        [Space]
        [Header("UI")]
        public Button exitButton;
        public Button nextButton;
        [Header("장소")]
        public TMP_Text placename;
        public TMP_Text placeAddress;
        [Header("사진")]
        public Image currentImage;
        public TMP_Text photoLength;
        [Header("전화")]
        public Button callButton;
        [Header("리뷰")]
        public float rating;
        public TMP_Text totalRiviews;
        public GameObject[] stars;

        private DestinationNotify _destinationNotify;

        private void Start()
        {
            _destinationNotify = FindAnyObjectByType<DestinationNotify>();
            /* Event */
            //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            exitButton.onClick.AddListener(Close);
            nextButton.onClick.AddListener(delegate { Open(); OnCurumi(); });
            //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
        }
        #region Swich 
        public override void Close()
        {
            TargetSwitch2Meun();
            UIManager.Instance.LoadingSet = false;
        }

        public override void Open()
        {
            TargetSwitch();
            UIManager.Instance.LoadingSet = false;


        }

        public override void TargetSwitch()
        {
            UIManager.Instance.Switch(CurrentMenu, TargetMenuType);
        }

        public void TargetSwitch2Meun()
        {
            UIManager.Instance.Switch(CurrentMenu, TargetMeun2Type);
        }
        #endregion

        public void UpdateStarRating(float rating)
        {
            int roundedRating = (int)Math.Floor(rating); // 평점 반내림

            for (int i = 0; i < stars.Length; i++)
            {
                
                if (i < roundedRating)
                {
                    stars[i].gameObject.SetActive(true);
                }
                else
                {
                    stars[i].gameObject.SetActive(false);
                }
            }
        }

        public void OnCurumi()
        {
            _destinationNotify.AddDestination();
            
        }
    }

}
