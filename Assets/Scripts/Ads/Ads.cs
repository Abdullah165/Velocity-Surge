using GoogleMobileAds.Api;
using UnityEngine;

public class Ads : MonoBehaviour
{
    //const string rewardedAdId = "ca-app-pub-5782906284382102/7591152813";
    const string rewardedAdId = "ca-app-pub-3940256099942544/5224354917";

    private RewardedAd _rewardedAd;

    private BannerView bannerView;
    private const string bannerID = "ca-app-pub-3940256099942544/6300978111";

    private void Start()
    {
        MobileAds.Initialize(initStatus => { });
        //RequestRewarededAd();
        RequestBanner();
    }

    void RequestRewarededAd()
    {
        _rewardedAd = new RewardedAd(rewardedAdId);
        AdRequest adRequest = new AdRequest.Builder().Build();
        _rewardedAd.LoadAd(adRequest);
    }

    public void ShowRewardedAd()
    {
        _rewardedAd.Show();
        RequestRewarededAd();
    }

    void RequestBanner()
    {
        bannerView = new BannerView(bannerID, AdSize.Banner, AdPosition.BottomLeft);
        AdRequest adRequest = new AdRequest.Builder().Build();
        bannerView.LoadAd(adRequest);
    }
}
