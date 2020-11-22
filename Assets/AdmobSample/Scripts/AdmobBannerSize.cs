using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kameffee
{
    public class AdmobBannerSize : MonoBehaviour
    {
        [SerializeField]
        private AdPosition adPosition = AdPosition.Top;

        [SerializeField]
        private AdSize.Type type = AdSize.Type.AnchoredAdaptive;

        [SerializeField]
        private UnityEvent<float> onHeightChanged;

        private DeviceOrientation _currentOrientation = DeviceOrientation.Unknown;

        private void Start()
        {
            UpdateBannerSize();
        }

        public void SetAdSizeType(int sizeType)
        {
            SetAdSizeType((AdSize.Type) sizeType);
        }

        public void SetAdSizeType(AdSize.Type sizeType)
        {
            type = sizeType;
            UpdateBannerSize();
        }

        private void Update()
        {
            if (Input.deviceOrientation != _currentOrientation)
            {
                UpdateBannerSize();
                _currentOrientation = Input.deviceOrientation;
            }
        }

        private void UpdateBannerSize()
        {
            AdSize adSize = GetAdSize();
            var bannerView = new BannerView("", adSize, adPosition);
            Debug.Log($"w:{bannerView.GetWidthInPixels()} h:{bannerView.GetHeightInPixels()}");

            float height = bannerView.GetHeightInPixels();
            var canvas = transform.root as RectTransform;
            height = height / canvas.localScale.x;

            onHeightChanged.Invoke(height);
        }

        private AdSize GetAdSize()
        {
            switch (type)
            {
                case AdSize.Type.Standard:
                    return AdSize.Banner;
                case AdSize.Type.SmartBanner:
                    return AdSize.SmartBanner;
                case AdSize.Type.AnchoredAdaptive:
                    return AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}