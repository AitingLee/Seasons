using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;
using Photon.Pun;

public class CardManager : MonoBehaviour
{
    public List<int> drawDeck;
    public List<int> discardDeck;
    public List<Card> wholeCards;

    private static GameManager GM;
    private static CardManager cm;
    public static CardManager CM
    {
        get
        {
            if (cm == null)
            {
                cm = FindObjectOfType(typeof(CardManager)) as CardManager;
                if (cm == null)
                {
                    GameObject go = new GameObject("cm");
                    cm = go.AddComponent<CardManager>();
                }
            }
            return cm;
        }
    }
    public void ReadWholeCards()
    {
        var cardReader = new StreamReader(File.OpenRead(@Application.dataPath + "\\Resources\\Card\\Cards.csv"), System.Text.Encoding.GetEncoding("Big5"));
        List<string> listCardNo = new List<string>();
        List<string> listCardType = new List<string>();
        List<string> listCardName = new List<string>();
        List<string> listCardPoint = new List<string>();
        List<string> listCostPoint2 = new List<string>();   //二人遊戲時牌的消耗資源
        List<string> listCostWater2 = new List<string>();
        List<string> listCostEarth2 = new List<string>();
        List<string> listCostFire2 = new List<string>();
        List<string> listCostAir2 = new List<string>();
        List<string> listCostPoint3 = new List<string>();   //三人遊戲時牌的消耗資源
        List<string> listCostWater3 = new List<string>();
        List<string> listCostEarth3 = new List<string>();
        List<string> listCostFire3 = new List<string>();
        List<string> listCostAir3 = new List<string>();
        List<string> listCostPoint4 = new List<string>();   //四人遊戲時牌的消耗資源
        List<string> listCostWater4 = new List<string>();
        List<string> listCostEarth4 = new List<string>();
        List<string> listCostFire4 = new List<string>();
        List<string> listCostAir4 = new List<string>();
        List<string> listDescription = new List<string>();

        while (!cardReader.EndOfStream)
        {
            var line = cardReader.ReadLine();
            var values = line.Split(',');

            listCardNo.Add(values[0]);
            listCardType.Add(values[1]);
            listCardName.Add(values[2]);
            listCardPoint.Add(values[3]);
            listCostPoint2.Add(values[4]);
            listCostWater2.Add(values[5]);
            listCostEarth2.Add(values[6]);
            listCostFire2.Add(values[7]);
            listCostAir2.Add(values[8]);
            listCostPoint3.Add(values[9]);
            listCostWater3.Add(values[10]);
            listCostEarth3.Add(values[11]);
            listCostFire3.Add(values[12]);
            listCostAir3.Add(values[13]);
            listCostPoint4.Add(values[14]);
            listCostWater4.Add(values[15]);
            listCostEarth4.Add(values[16]);
            listCostFire4.Add(values[17]);
            listCostAir4.Add(values[18]);
            listDescription.Add(values[19]);
        }

        for (int i = 0; i < listCardNo.Count; i++)
        {
            Card card = new Card();
            card.cardName = listCardName[i];
            card.description = listDescription[i];

            if (listCardType[i] == "T")
            {
                card.whichType = Card.CardType.MagicItem;
            }
            else
            {
                card.whichType = Card.CardType.Familiar;
            }

            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                try
                {
                    card.cardID = Convert.ToInt32(listCardNo[i]);
                    card.cardPoint = Convert.ToInt32(listCardPoint[i]);
                    card.costPoint = Convert.ToInt32(listCostPoint2[i]);
                    card.costWater = Convert.ToInt32(listCostWater2[i]);
                    card.costEarth = Convert.ToInt32(listCostEarth2[i]);
                    card.costFire = Convert.ToInt32(listCostFire2[i]);
                    card.costAir = Convert.ToInt32(listCostAir2[i]);
                }
                catch
                {
                    Debug.LogError("Error in Card's Constructing");
                }
            }
            else if (PhotonNetwork.CurrentRoom.PlayerCount == 3)
            {
                try
                {
                    card.cardID = Convert.ToInt32(listCardNo[i]);
                    card.cardPoint = Convert.ToInt32(listCardPoint[i]);
                    card.costPoint = Convert.ToInt32(listCostPoint3[i]);
                    card.costWater = Convert.ToInt32(listCostWater3[i]);
                    card.costEarth = Convert.ToInt32(listCostEarth3[i]);
                    card.costFire = Convert.ToInt32(listCostFire3[i]);
                    card.costAir = Convert.ToInt32(listCostAir3[i]);
                }
                catch
                {
                    Debug.LogError("Error in Card's Constructing");
                }
            }
            else if (PhotonNetwork.CurrentRoom.PlayerCount == 4)
            {
                try
                {
                    card.cardID = Convert.ToInt32(listCardNo[i]);
                    card.cardPoint = Convert.ToInt32(listCardPoint[i]);
                    card.costPoint = Convert.ToInt32(listCostPoint4[i]);
                    card.costWater = Convert.ToInt32(listCostWater4[i]);
                    card.costEarth = Convert.ToInt32(listCostEarth4[i]);
                    card.costFire = Convert.ToInt32(listCostFire4[i]);
                    card.costAir = Convert.ToInt32(listCostAir4[i]);
                }
                catch
                {
                    Debug.LogError("Error in Card's Constructing");
                }
            }
            else
            {
                Debug.LogError("Error in Player numbers");
            }
            wholeCards.Add(card);
        }
    }

    public int DrawCardIndex(int drawDeckIndex)      //draw card from draw deck[index] , won't remove from draw deck
    {
        if (drawDeckIndex<drawDeck.Count)
        {
            int cardNo = drawDeck[drawDeckIndex];
            return wholeCards[cardNo - 1].cardID;
        }
        else
        {
            Debug.LogError($"in Draw Card Index : Index {drawDeckIndex} > count {drawDeck.Count}");
            return -1;  //出錯
        }

    }

    public void RemoveFirstFromDrawDeck()    // remove means drwan by other, not sent to discard Deck
    {
        drawDeck.RemoveAt(0);
    }

    //public void DiscardCardByIndex   >> Discard from draw Deck to discard Deck
}
