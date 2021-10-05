using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReserveDecision : MonoBehaviour
{
    public Player player;

    public Text caption;
    public Dropdown disWaterOption, disEarthOption, disFireOption, disAirOption;
    public Dropdown getWaterOption, getEarthOption, getFireOption, getAirOption;

    int m_reserveSpace, m_oGetWish, m_oGetWater, m_oGetEarth, m_oGetFire, m_oGetAir;
    int m_originHas;

    public void SetOriginGetElement(int originHas,int getAsWish, int getWater, int getEarth, int getFire, int getAir)
    {
        m_reserveSpace = player.myInfo.reserveSpace;
        m_originHas = originHas;
        m_oGetWish = getAsWish;
        m_oGetWater = getWater;
        m_oGetEarth = getEarth;
        m_oGetFire = getFire;
        m_oGetAir = getAir;
        SetReserveDecision();
    }
    public void SetReserveDecision()
    {
        int totalGet = m_oGetWater + m_oGetEarth + m_oGetFire + m_oGetAir + m_oGetWish;
        int canDiscard = totalGet + m_originHas - m_reserveSpace;
        caption.text = $"獲取元素後超出儲存格上限。\n您可選擇丟棄至多{canDiscard}個原有元素，\n以換取儲存格空間。";

        int getWaterMax = m_oGetWater + m_oGetWish;
        int getEarthMax = m_oGetEarth + m_oGetWish;
        int getFireMax = m_oGetFire + m_oGetWish;
        int getAirMax = m_oGetAir + m_oGetWish;

        int disWaterMax = 0;
        int disEarthMax = 0;
        int disFireMax = 0;
        int disAirMax = 0;

        if (player.myInfo.waterToken >= canDiscard)
        {
            disWaterMax = canDiscard;
        }
        else
        {
            disWaterMax = player.myInfo.waterToken;
        }
        if (player.myInfo.earthToken >= canDiscard)
        {
            disEarthMax = canDiscard;
        }
        else
        {
            disEarthMax = player.myInfo.earthToken;
        }
        if (player.myInfo.fireToken >= canDiscard)
        {
            disFireMax = canDiscard;
        }
        else
        {
            disFireMax = player.myInfo.fireToken;
        }
        if (player.myInfo.airToken >= canDiscard)
        {
            disAirMax = canDiscard;
        }
        else
        {
            disAirMax = player.myInfo.airToken;
        }

        SetGetOption(getWaterMax, getEarthMax, getFireMax, getAirMax);
        SetDiscardOption(disWaterMax, disEarthMax, disFireMax, disAirMax);
        gameObject.SetActive(true);
    }
    public void SetGetOption(int getWaterMax, int getEarthMax, int getFireMax, int getAirMax)
    {
        SetGetOptionString(getWaterOption, getWaterMax);
        SetGetOptionString(getEarthOption, getEarthMax);
        SetGetOptionString(getFireOption, getFireMax);
        SetGetOptionString(getAirOption, getAirMax);
    }
    public void SetGetOptionString(Dropdown dropdown, int maxValue)
    {
        dropdown.ClearOptions();
        List<string> options = new List<string>();
        for (int i = 0; i < maxValue + 1; i++)
        {
            options.Add(i.ToString());
        }
        dropdown.AddOptions(options);
    }
    public void SetDiscardOption(int disWaterMax, int disEarthMax, int disFireMax, int disAirMax)
    {
        SetDiscardOptionString(disWaterOption, disWaterMax);
        SetDiscardOptionString(disEarthOption, disEarthMax);
        SetDiscardOptionString(disFireOption, disFireMax);
        SetDiscardOptionString(disAirOption, disAirMax);
    }
    public void SetDiscardOptionString(Dropdown dropdown, int maxValue)
    {
        dropdown.ClearOptions();
        List<string> options = new List<string>();
        for (int i = 0; i < maxValue + 1; i++)
        {
            options.Add($"-{i.ToString()}");
        }
        dropdown.AddOptions(options);
    }

    public void CheckReserveButtonOnClick()
    {
        int getWater = getWaterOption.value;
        int getEarth = getEarthOption.value;
        int getFire = getFireOption.value;
        int getAir = getAirOption.value;
        int getTotal = getWater + getEarth + getFire + getAir;

        int asWishToWater = getWater - m_oGetWater;
        int asWishToEarth = getEarth - m_oGetEarth;
        int asWishToFire = getFire - m_oGetFire;
        int asWishToAir = getAir - m_oGetAir;
        int asWishTotal = asWishToWater + asWishToEarth + asWishToFire + asWishToAir;

        if (asWishTotal <= m_oGetWish)      //檢查可獲取的任意元素是否有超出上限
        {
            int disWater = disWaterOption.value;
            int disEarth = disEarthOption.value;
            int disFire = disFireOption.value;
            int disAir = disAirOption.value;
            int disTotal = disWater + disEarth + disFire + disAir;
            if (m_originHas - disTotal + getTotal == m_reserveSpace)
            {
                if (disWater > 0)
                {
                    player.UseOrDiscardEnergy(1, disWater);
                }
                if (disEarth > 0)
                {
                    player.UseOrDiscardEnergy(2, disEarth);
                }
                if (disFire > 0)
                {
                    player.UseOrDiscardEnergy(3, disFire);
                }
                if (disAir > 0)
                {
                    player.UseOrDiscardEnergy(4, disAir);
                }
                player.GetTokenAfterCheck(getWater, getEarth, getFire, getAir);
                player.tempBlockAction = false;
                gameObject.SetActive(false);
            }
            else
            {
                int emptySlot = m_reserveSpace - m_originHas;
                player.LogWarning($"所選擇的元素加總應恰好為{emptySlot}個");
            }
        }
        else
        {
            player.LogWarning($"所選擇的任意元素超出上限，上限為{m_oGetWish}個");
        }
    }
}
