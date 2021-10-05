using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform parnetToReturnTo = null;
    Player player;

    void Awake()
    {
        player = GameManager.GM.myPlayer;
    }

    public void OnBeginDrag (PointerEventData eventData)
    {
        Debug.Log("On Begin Drag");

        if (player.myAction && !player.isDrawingCard && !player.tempBlockAction)
        {
            player.DropCardZone.SetActive(true);
        }
        parnetToReturnTo = this.transform.parent;
        this.transform.SetParent(this.transform.parent.parent);     //開始拖曳時將物件移出parent HandCardZone

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("On End Drag");
        this.transform.SetParent(parnetToReturnTo);
        if (player.DropCardZone.activeSelf)
        {
            player.DropCardZone.SetActive(false);
        }
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
