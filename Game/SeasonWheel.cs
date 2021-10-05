using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonWheel : MonoBehaviour
{
    public GameObject seasonToken, yearToken;

    public void ChangMonth(ref int month, int moveSpace)
    {
        int newMonth = month + moveSpace;
        if (newMonth > 12)
        {
            newMonth = newMonth - 12;
            ChangeYear(GameManager.GM.year);
        }
        int oldSeasonNo = (month - 1) / 3;
        int newSeasonNo = (newMonth - 1) / 3;
        if (oldSeasonNo != newSeasonNo)
        {
            ChangeSeason(GameManager.GM.season);
        }
        MoveSeasonToken(newMonth);
        month = newMonth;
    }

    public void ChangeSeason(GameManager.Season originSeason)
    {
        switch (originSeason)
        {
            case GameManager.Season.Winter:
                GameManager.GM.season = GameManager.Season.Spring;
                break;
            case GameManager.Season.Spring:
                GameManager.GM.season = GameManager.Season.Summer;
                break;
            case GameManager.Season.Summer:
                GameManager.GM.season = GameManager.Season.Fall;
                break;
            case GameManager.Season.Fall:
                GameManager.GM.season = GameManager.Season.Winter;
                break;
        }
    }

    public void ChangeYear(int originYear)
    {
        GameManager.GM.year++;
        
    }

    Vector3 roundCenter = new Vector3(-90f, 9.08f, 0f);
    float seasonDiceAngle;

    void MoveSeasonToken(int month)
    {
        switch (month)
        {
            case 1:
                seasonDiceAngle = 3 * 22.5f + 6;
                break;
            case 2:
                seasonDiceAngle = 2 * 22.5f;
                break;
            case 3:
                seasonDiceAngle = 22.5f - 6;
                break;
            case 4:
                seasonDiceAngle = 15 * 22.5f + 6;
                break;
            case 5:
                seasonDiceAngle = 14 * 22.5f;
                break;
            case 6:
                seasonDiceAngle = 13 * 22.5f - 6;
                break;
            case 7:
                seasonDiceAngle = 11 * 22.5f + 6;
                break;
            case 8:
                seasonDiceAngle = 10 * 22.5f;
                break;
            case 9:
                seasonDiceAngle = 9 * 22.5f - 6;
                break;
            case 10:
                seasonDiceAngle = 7 * 22.5f + 6;
                break;
            case 11:
                seasonDiceAngle = 6 * 22.5f;
                break;
            case 12:
                seasonDiceAngle = 5 * 22.5f - 6;
                break;
            default:
                Debug.LogError("Error in SeasonDiceGo month");
                break;
        }
        seasonToken.transform.localPosition = roundCenter + new Vector3(5.3f * Mathf.Cos(seasonDiceAngle * Mathf.Deg2Rad), 0, 5.3f * Mathf.Sin(seasonDiceAngle * Mathf.Deg2Rad));
    }
}
