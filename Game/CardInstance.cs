using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardInstance : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    public Card thisCard;
    private float timer, waitView = 1;
    private bool isToSetView, hasSetView;
    GameObject cardViewPrefab, cardView;
    Transform viewParent;
    private Player player;

    public void Awake()
    {
        cardViewPrefab = Resources.Load<GameObject>($"Card\\Prefab\\CardView");
        viewParent = GameObject.Find("GameCanvas").transform;
        player = GameManager.GM.myPlayer;
    }


    // 若CardInstace是Summoned Card用 On Mouse Method
    public void OnMouseEnter()
    {
        Debug.Log($"Mouse Enter{this.name}");
        for (int i = 0; i < 19; i++)
        {
            player.cardViews[i].SetActive(false);
        }
        timer = Time.time;
        hasSetView = false;
        isToSetView = true;
    }

    public void OnMouseExit()
    {
        if (hasSetView)
        {
            cardView.SetActive(false);
        }
        isToSetView = false;
    }

    // 若CardInstace是UI Card用 Pointer Handler
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"Mouse Enter{this.name}");
        GameObject toDestroy = GameObject.Find("CardView(Clone)");
        Destroy(toDestroy);
        timer = Time.time;
        hasSetView = false;
        isToSetView = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hasSetView)
        {
            Destroy(cardView);
        }
        isToSetView = false;
    }

    public void Update()
    {
        if (isToSetView && Time.time - timer > waitView)
        {
            if (gameObject.GetComponent<Draggable>() != null)       //若CardInstace是UI Card
            {
                Vector3 objectPos = gameObject.transform.localPosition;
                Vector3 adjustPos = new Vector3(-150, -150, 0);
                cardView = Instantiate(cardViewPrefab, new Vector3(0, 0, 0), Quaternion.identity, viewParent);
                cardView.transform.localPosition = objectPos + adjustPos;
            }
            else           //若CardInstace是Summoned Card
            {
                for (int i = 0; i<19; i++)
                {
                    if (gameObject == player.summonCards[i])
                    {
                        cardView = player.cardViews[i];
                        break;
                    }
                }
            }
            cardView.GetComponent<CardView>().SetCardView(thisCard);
            cardView.SetActive(true);
            hasSetView = true;
            isToSetView = false;
        }
    }
}
