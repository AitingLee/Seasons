using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceView : MonoBehaviour
{
    public Text effectDescription, getPointText;
    public Image allSidesImage;
    private Player player;
    private DiceManager dm;
    private SeasonDiceSideInfo info;
    public Image[] getAreas;

    public void SetDiceView(SeasonDice dice)
    {
        getAreas[0].gameObject.SetActive(false);
        getAreas[1].gameObject.SetActive(false);

        Debug.Log($"Dice info : season {dice.diceSeason.ToString()} No {dice.diceNo} value {dice.sideValue}");
        dm = DiceManager.DM;
        info = new SeasonDiceSideInfo();
        player = GameManager.GM.myPlayer;
        effectDescription.text = "";
        info = dm.GetDiceSideInfo(dice.diceSeason, dice.diceNo, dice.sideValue);

        string fileName = $"{dice.diceSeason.ToString().ToLower()}{dice.diceNo}_0";
        var sprite = Resources.Load<Sprite>($"Dice\\Textures\\Upward\\{fileName}");
        allSidesImage.sprite = sprite;

        if (info.gauge)
        {
            effectDescription.text += "召喚等級提升1級。\n";
        }
        if (info.transmute)
        {
            effectDescription.text += "本回合可以轉化。\n";
        }
        if (info.drawCard)
        {
            effectDescription.text += "抽一張牌。\n";
        }
        effectDescription.text += $"如果這顆季節骰沒被選擇，\n季節指示物將前進{info.moveSpaces}步。";

        getPointText.text = info.crystals.ToString();

        Debug.Log($"dice view water {info.getWater} earth {info.getEarth} fire {info.getFire} air {info.getAir}");
        int countGet = 0;
        if (info.getWater > 0)
        {
            Debug.Log(info.getWater);
            for (int i = 0; i < info.getWater; i++)
            {
                getAreas[countGet].gameObject.SetActive(true);
                getAreas[countGet].sprite = player.waterTokenSprite;
                countGet++;
            }
        }
        if (info.getEarth > 0)
        {
            Debug.Log(info.getEarth);
            for (int i = 0; i < info.getEarth; i++)
            {
                getAreas[countGet].gameObject.SetActive(true);
                getAreas[countGet].sprite = player.earthTokenSprite;
                countGet++;
            }
        }
        if (info.getFire > 0)
        {
            Debug.Log(info.getFire);
            for (int i = 0; i < info.getFire; i++)
            {
                getAreas[countGet].gameObject.SetActive(true);
                getAreas[countGet].sprite = player.fireTokenSprite;
                countGet++;
            }
        }
        if (info.getAir > 0)
        {
            Debug.Log(info.getAir);
            for (int i = 0; i < info.getAir; i++)
            {
                getAreas[countGet].gameObject.SetActive(true);
                getAreas[countGet].sprite = player.airTokenSprite;
                countGet++;
            }
        }
        if (countGet > 2)
        {
            Debug.LogError($"diceInfo.countGet > 2");
        }
        gameObject.SetActive(true);
    }
}
