using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoInstance : MonoBehaviour
{
    public Text OppName, CrystalText, HandCardText, BonusText, GaugeText;
    public GameObject Hourglass, RoundStartCube;
    public GameObject[] ElementSlots;
    public GameObject chosenDice;
    [HideInInspector]
    public int thisOppNo;
    [HideInInspector]
    public bool hasSetPlayer;
}
