using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AsWishDecision : MonoBehaviour
{
    public Player player;

    public Text caption;
    public Dropdown waterOption, earthOption, fireOption, airOption;


    int m_reserveSpace, m_canGetWish, m_oWater, m_oEarth, m_oFire, m_oAir;

    public void SetWaterOption(int maxValue)
    {
        SetOptionString(waterOption, maxValue);
    }
    public void SetEarthOption(int maxValue)
    {
        SetOptionString(earthOption, maxValue);
    }
    public void SetFireOption(int maxValue)
    {
        SetOptionString(fireOption, maxValue);
    }
    public void SetAirOption(int maxValue)
    {
        SetOptionString(airOption, maxValue);
    }
    public void SetCanGetWish(int canGetHowMuch)
    {
        m_canGetWish = canGetHowMuch;
    }
    public void SetOriginGetElement(int getWater, int getEarth, int getFire, int getAir)
    {
        m_oWater = getWater;
        m_oEarth = getEarth;
        m_oFire = getFire;
        m_oAir = getAir;
    }

    public void SetChooseWish(int asWishAmount, int getWater, int getEarth, int getFire, int getAir)
    {
        caption.text = $"可獲得{asWishAmount}個任意元素，\n請選擇您要獲取的元素數量。";
        SetWaterOption(asWishAmount);
        SetEarthOption(asWishAmount);
        SetFireOption(asWishAmount);
        SetAirOption(asWishAmount);
        SetCanGetWish(asWishAmount);
        SetOriginGetElement(getWater, getEarth, getFire, getAir);
        gameObject.SetActive(true);
    }

    public void CheckWishButtonOnClick()
    {
        if (waterOption.value + earthOption.value + fireOption.value + airOption.value == m_canGetWish)
        {
            int getWater = m_oWater + waterOption.value;
            int getEarth = m_oEarth + earthOption.value;
            int getFire = m_oFire + fireOption.value;
            int getAit = m_oAir + airOption.value;
            player.GetTokenAfterCheck(getWater, getEarth, getFire, getAit);
            player.tempBlockAction = false;
            gameObject.SetActive(false);
        }
        else
        {
            player.LogWarning($"需要恰好選擇獲取{m_canGetWish}個元素");
        }
    }

    public void SetOptionString(Dropdown dropdown, int maxValue)
    {
        dropdown.ClearOptions();
        List<string> options = new List<string>();
        for (int i = 0; i < maxValue + 1; i++)
        {
            options.Add(i.ToString());
        }
        dropdown.AddOptions(options);
    }

    //public void CheckReserveButtonOnClick()
    //{
    //    int reserveWater = waterOption.value + 1;
    //    int reserveEarth = earthOption.value + 1;
    //    int reserveFire = fireOption.value + 1;
    //    int reserveAir = airOption.value + 1;
    //    if (reserveWater + reserveEarth + reserveAir + reserveFire == m_reserveSpace)
    //    {
    //        // To Destroy : reserve < origin
    //        if (ToDestroy(player.myInfo.waterToken, reserveWater))
    //        {
    //            DiscardToken("Water", player.myInfo.waterToken - reserveWater);
    //        }
    //        if (ToDestroy(player.myInfo.earthToken, reserveEarth))
    //        {
    //            DiscardToken("Earth", player.myInfo.earthToken - reserveEarth);
    //        }
    //        if (ToDestroy(player.myInfo.fireToken, reserveFire))
    //        {
    //            DiscardToken("Fire", player.myInfo.fireToken - reserveFire);
    //        }
    //        if (ToDestroy(player.myInfo.airToken, reserveAir))
    //        {
    //            DiscardToken("Air", player.myInfo.airToken - reserveAir);
    //        }
    //        // To Instantiate : reserve > origin
    //        if (!ToDestroy(player.myInfo.waterToken, reserveWater))
    //        {
    //            for (int i = 0; i < reserveWater - player.myInfo.waterToken; i++)
    //            {
    //                player.InstantiateToken(player.waterTokenPrefab);
    //            }
    //        }
    //        if (!ToDestroy(player.myInfo.earthToken, reserveEarth))
    //        {
    //            for (int i = 0; i < reserveWater - player.myInfo.waterToken; i++)
    //            {
    //                player.InstantiateToken(player.earthTokenPrefab);
    //            }
    //        }
    //        if (!ToDestroy(player.myInfo.fireToken, reserveFire))
    //        {
    //            for (int i = 0; i < reserveWater - player.myInfo.waterToken; i++)
    //            {
    //                player.InstantiateToken(player.fireTokenPrefab);
    //            }
    //        }
    //        if (!ToDestroy(player.myInfo.airToken, reserveAir))
    //        {
    //            for (int i = 0; i < reserveWater - player.myInfo.waterToken; i++)
    //            {
    //                player.InstantiateToken(player.airTokenPrefab);
    //            }
    //        }

    //        player.myInfo.waterToken = reserveWater;
    //        player.myInfo.earthToken = reserveEarth;
    //        player.myInfo.fireToken = reserveFire;
    //        player.myInfo.airToken = reserveAir;
    //        gameObject.SetActive(false);
    //        player.tempBlockAction = false;
    //    }
    //    else
    //    {
    //        player.LogWarning($"保留元素總數需恰好為{m_reserveSpace}個。");
    //    }

    //}


    //bool ToDestroy(int before, int after)
    //{
    //    if (before > after)
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}

    //void DiscardToken(string Element,int amount)
    //{
    //    for (int i = 0; i < amount; i++)
    //    {
    //        GameObject tbd = GameObject.FindGameObjectWithTag($"{Element}Token");
    //        Destroy(tbd);
    //    }
    //    string elementCh = "";
    //    switch (Element)
    //    {
    //        case "Water":
    //            elementCh = "水";
    //            break;
    //        case "Earth":
    //            elementCh = "土";
    //            break;
    //        case "Fire":
    //            elementCh = "火";
    //            break;
    //        case "Air":
    //            elementCh = "風";
    //            break;
    //    }
    //    player.LogGameLog($"丟棄 {amount} 個{elementCh}元素");
    //}
}
