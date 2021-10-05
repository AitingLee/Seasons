using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{ 
    public int cardID;
    public string cardName, description;
    public enum CardType { Familiar, MagicItem };
    public CardType whichType;
    public int cardPoint;
    public int costPoint, costWater, costEarth, costFire, costAir;

}
