using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SeasonDice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameManager.Season diceSeason;
    public int diceNo;
    public int sideValue;

    GameObject diceViewPrefab, diceView;
    private bool isToSetView, hasSetView;
    private float timer, waitView;

    private static DiceManager dm;

    void Awake()
    {
        diceViewPrefab = Resources.Load<GameObject>($"Dice\\Prefab\\DiceView");
        waitView = 1;
        dm = DiceManager.DM;
    }

    void Update()
    {
        if (sideValue != 0 && isToSetView && Time.time - timer > waitView)
        {
            if (gameObject.GetComponent<SeasonDiceGO>() == null)       //若SeasonDice是UI Image
            {
                Vector3 objectPos = gameObject.transform.localPosition;
                Vector3 adjustPos = new Vector3(130, -170, 0);
                diceView = Instantiate(diceViewPrefab, new Vector3(0, 0, 0), Quaternion.identity, gameObject.transform.parent);
                diceView.transform.localPosition = objectPos + adjustPos;
            }
            else           //若SeasonDice是GameObject
            {
                diceView = dm.diceViewDefault;
            }
            diceView.GetComponent<DiceView>().SetDiceView(this);
            hasSetView = true;
            isToSetView = false;
        }
    }

    // 若SeasonDice是GameObject用 On Mouse Method
    public void OnMouseEnter()
    {
        Debug.Log($"Mouse Enter{this.name}");
        if (hasSetView)
        {
            diceView.SetActive(false);
            hasSetView = false;
        }
        timer = Time.time;
        isToSetView = true;
    }

    public void OnMouseExit()
    {
        if (hasSetView)
        {
            diceView.SetActive(false);
            hasSetView = false;
        }
        isToSetView = false;
    }

    // 若SeasonDice是UI Image用 Pointer Handler
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"Pointer Enter{this.name}");
        timer = Time.time;
        isToSetView = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hasSetView)
        {
            Destroy(diceView);
        }
        isToSetView = false;
    }
}
