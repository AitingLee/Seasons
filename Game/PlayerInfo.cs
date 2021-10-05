using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    public int playerNo; // 0~3
    public string playerName;
    public int reserveSpace, waterToken, earthToken, fireToken, airToken, crystal, handCardCount, bonusCost, gaugeCount;    
    public List<int> handCardIndex, summonCardIndex, tempDrawingIndex;
    public bool finishAndWait, hasChosenDice;
    public SeasonDice chosenDice;
}
