using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    public Text cardName, description, costPointText, cardType, cardPointText;
    public Image[] CostArea;


    public void SetCardView(Card card)
    {
        Player player = GameManager.GM.myPlayer;
        cardName.text = card.cardName;
        description.text = card.description;
        if (card.whichType == Card.CardType.Familiar)
        {
            cardType.text = "神僕";
        }
        else
        {
            cardType.text = "魔器";
        }
        cardPointText.text = card.costPoint.ToString();
        cardPointText.text = card.cardPoint.ToString();
        int countCost = 0;
        if (card.costWater != 0)
        {
            for (int i = 0; i < card.costWater; i++)
            {
                CostArea[countCost].gameObject.SetActive(true);
                CostArea[countCost].sprite = player.waterTokenSprite;
                countCost++;
            }
        }
        if (card.costEarth != 0)
        {
            for (int i = 0; i < card.costEarth; i++)
            {
                CostArea[countCost].gameObject.SetActive(true);
                CostArea[countCost].sprite = player.earthTokenSprite;
                countCost++;
            }
        }
        if (card.costFire != 0)
        {
            for (int i = 0; i < card.costFire; i++)
            {
                CostArea[countCost].gameObject.SetActive(true);
                CostArea[countCost].sprite = player.fireTokenSprite;
                countCost++;
            }
        }
        if (card.costAir != 0)
        {
            for (int i = 0; i < card.costAir; i++)
            {
                CostArea[countCost].gameObject.SetActive(true);
                CostArea[countCost].sprite = player.airTokenSprite;
                countCost++;
            }
        }
    }

}
