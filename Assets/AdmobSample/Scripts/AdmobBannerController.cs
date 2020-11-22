using System;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Events;

namespace Kameffee
{
    public class AdmobBannerController : MonoBehaviour
    {
        public AdSize.Type type = AdSize.Type.AnchoredAdaptive;

        public AdPosition adPosition = AdPosition.Top;

        public void SetAdSizeType(int type)
        {
            this.type = (AdSize.Type) type;
            // Update
            if (bannerView != null)
                RequestBannerAd();
        }

        #region Events

        [Header("Events")]
        public UnityEvent OnAdLoadedEvent;

        public UnityEvent OnAdFailedToLoadEvent;
        public UnityEvent OnAdOpeningEvent;
        public UnityEvent OnAdClosedEvent;
        public UnityEvent OnAdLeavingApplicationEvent;

        #endregion
        
        private DeviceOrientation _currentOrientation = DeviceOrientation.Unknown;


        private BannerView bannerView;

        private void Update()
        {
            if (Input.deviceOrientation != _currentOrientation)
            {
                // Update
                if (bannerView != null)
                    RequestBannerAd();
                _currentOrientation = Input.deviceOrientation;
            }
        }

        public void RequestBannerAdForTop()
        {
            RequestBannerAd(AdPosition.Top);
        }

        public void RequestBannerAdForBottom()
        {
            RequestBannerAd(AdPosition.Bottom);
        }

        public void RequestBannerAd()
        {
            RequestBannerAd(adPosition);
        }

        public void RequestBannerAd(AdPosition adPosition)
        {
#if UNITY_EDITOR
            string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
        string adUnitId = "unexpected_platform";
#endif
            // Clean up banner before reusing
            bannerView?.Destroy();

            this.adPosition = adPosition;

            AdSize adSize = GetAdSize();
            bannerView = new BannerView(adUnitId, adSize, this.adPosition);

            // Add Event Handlers
            bannerView.OnAdLoaded += (sender, args) => OnAdLoadedEvent.Invoke();
            bannerView.OnAdFailedToLoad += (sender, args) => OnAdFailedToLoadEvent.Invoke();
            bannerView.OnAdOpening += (sender, args) => OnAdOpeningEvent.Invoke();
            bannerView.OnAdClosed += (sender, args) => OnAdClosedEvent.Invoke();
            bannerView.OnAdLeavingApplication += (sender, args) => OnAdLeavingApplicationEvent.Invoke();

            // Load a banner ad
            bannerView.LoadAd(CreateAdRequest());
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

        private AdRequest CreateAdRequest()
        {
            return new AdRequest.Builder()
                .AddTestDevice(AdRequest.TestDeviceSimulator)
                .AddTestDevice("0123456789ABCDEF0123456789ABCDEF")
                .AddKeyword("unity-admob-sample")
                .TagForChildDirectedTreatment(false)
                .AddExtra("color_bg", "9B30FF")
                .Build();
        }

        public void BannerHide()
        {
            bannerView?.Hide();
        }

        public void DestroyBannerAd()
        {
            bannerView?.Destroy();
            bannerView = null;
        }
    }
}