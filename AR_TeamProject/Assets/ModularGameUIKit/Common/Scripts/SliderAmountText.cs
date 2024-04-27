﻿// Copyright (C) 2023 ricimi. All rights reserved.
// This code can only be used under the standard Unity Asset Store EULA,
// a copy of which is available at https://unity.com/legal/as-terms.

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ricimi
{
    // This component is associated to a slider's amount text and is in charge
    // of keeping it updated with regards to the slider's current value.
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class SliderAmountText : MonoBehaviour
    {
    #pragma warning disable 649
        [SerializeField]
        private Slider slider;
    #pragma warning restore 649

        public string Suffix;
        public bool WholeNumber = true;

        private TextMeshProUGUI text;

        private void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
            SetAmountText(slider.value);
        }

        private void OnDestroy()
        {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }

        private void OnSliderValueChanged(float value)
        {
            SetAmountText(value);
        } 

        private void SetAmountText(float value)
        {
            if (WholeNumber)
                text.text = $"{(int)value}{Suffix}";
            else
                text.text = $"{Math.Round(value, 2)}{Suffix}";
        }
    }
}