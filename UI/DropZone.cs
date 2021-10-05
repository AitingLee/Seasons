using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZone : MonoBehaviour, IDropHandler
{
    public Card droppedCard;
    GameManager gm;
    CardManager cm;
    Player player;

    public void Awake()
    {
        gm = GameManager.GM;
        cm = CardManager.CM;
        player = gm.myPlayer;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggingCard = eventData.pointerDrag;
        Debug.Log($"{eventData.pointerDrag.name} was dropped on {gameObject.name}");
        Draggable drag = draggingCard.GetComponent<Draggable>();
        CardInstance cardInstance = draggingCard.transform.GetComponent<CardInstance>();
        if (drag != null)
        {
            Debug.Log("drag!=null");
            if (gameObject.name == "DropCardZone(Clone)")
            {
                if (player.PlayCard(cardInstance))
                {
                    drag.parnetToReturnTo = this.transform;
                    Debug.Log("set card instance");
                }
                else
                {
                    Debug.Log("Summon failed.");
                }
            }
            else if (gameObject.name == "HandCardZone" && player.isDrawingCard)
            {
                drag.parnetToReturnTo = this.transform;
                player.DrawTempCard(cardInstance, ref player.myInfo.tempDrawingIndex);
                player.isDrawingCard = false;
                Debug.Log($"Set myself {player.myInfo.playerName} is drawing card {player.isDrawingCard}");
                if (gm.inFirstDrawing)
                {
                    player.myInfo.finishAndWait = true;
                    player.UpdateMyInfo();
                    player.pv.RPC("RPC_MasterCheckChange", RpcTarget.MasterClient);
                }
            }
            else if (gm.inConstructingDeck && !player.myInfo.finishAndWait)
            {
                if (gameObject.name == "HandCardZone")
                {
                    drag.parnetToReturnTo = this.transform;
                }
                else if (gameObject.name == "FirstYearZone" || gameObject.name == "SecondYearZone" || gameObject.name == "ThirdYearZone")
                {
                    Debug.Log($"Drop on zone {gameObject.name} child count = {transform.childCount}");
                    if (transform.childCount < 3)
                    {
                        drag.parnetToReturnTo = this.transform;
                    }
                }
            }

        }
        else
        {
            Debug.Log("You Can't Drag This Object.");
        }

    }
}
