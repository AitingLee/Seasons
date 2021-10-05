using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Linq;
using System;

public class Player : MonoBehaviourPunCallbacks
{
    public PlayerInfo myInfo;
    public PlayerInfo[] allInfos;
    public Text drawDeckCount;
    public GameObject RollButton, FinishActionButton;
    public GameObject UICardPrefab, DrawCardZone, DropCardZone, HandCardZone;
    public GameObject DrawCheck, Warning;
    public Button[] bonusButtons;   // 0 trade 1 transmute 2 gauge 3 draw
    public AsWishDecision asWishDecision;
    public ReserveDecision reserveDecision;

    [SerializeField]
    public GameObject[] summonGauges;
    public GameObject[] summonCards;
    public GameObject[] cardViews;
    public GameObject[] oppInfoInstances;
    public Text myInfoCrystalText, myInfoBonusText, myInfoNameText;
    public GameObject constructingDeck, constructingFirst, constructingSecond, constructingThird, constructingDeckCheckButton;
    public Sprite waterTokenSprite, earthTokenSprite, fireTokenSprite, airTokenSprite, emptyTokenSprite;
    public GameObject waterTokenPrefab, earthTokenPrefab, fireTokenPrefab, airTokenPrefab, tokenSets;

    [HideInInspector]
    public bool isDrawingCard, myAction, tempBlockAction, canTransmute;
    [HideInInspector]
    public PhotonView pv;

    int playerCountInRoom;
    private static CardManager cm;
    private static GameManager gm;
    private static DiceManager dm;

    private bool showWarning;
    private float warningTimer;

    int[] firstYearDeck, secondYearDeck, thirdYearDeck;   

    public void Awake()
    {
        cm = CardManager.CM;
        gm = GameManager.GM;
        dm = DiceManager.DM;

        playerCountInRoom = PhotonNetwork.CurrentRoom.PlayerCount;
        pv = PhotonView.Get(this);
        allInfos = new PlayerInfo[playerCountInRoom];
        for (int i = 0; i < playerCountInRoom; i++)
        {
            allInfos[i] = new PlayerInfo();
        }
        myInfo = new PlayerInfo { playerName = PhotonNetwork.LocalPlayer.NickName};
        
        myInfo.handCardIndex = new List<int>();
        myInfo.summonCardIndex = new List<int>();
        myInfo.tempDrawingIndex = new List<int>();
        myInfo.chosenDice = dm.myChosenDicePos.GetComponent<SeasonDice>();  //set defaullt dice

        firstYearDeck = new int[3];   //第一年牌庫
        secondYearDeck = new int[3];  //第二年牌庫
        thirdYearDeck = new int[3];   //第三年牌庫

        cm.wholeCards = new List<Card>();      //整套牌各一張
        cm.drawDeck = new List<int>();    //每張牌會有兩張在drawDeck
        cm.discardDeck = new List<int>();
        cm.ReadWholeCards();
        if (PhotonNetwork.IsMasterClient)
        {
            System.Random random = new System.Random();
            for (int i = 1; i < cm.wholeCards.Count + 1; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    cm.drawDeck.Insert(random.Next(cm.drawDeck.Count), i);     //洗牌，每張牌置入兩次
                }
            }
        }

    }
    private void Update()
    {
        if (showWarning)
        {
            CheckWarningClose();
        }
    }
    private void Start()
    {
        myInfo.reserveSpace = 7;
        if (PhotonNetwork.IsMasterClient)
        {
            pv.RPC("RPC_MasterInitAllDrawingDeck", RpcTarget.Others, cm.drawDeck.ToArray());
            int count = 0;
            foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                pv.RPC("RPC_MasterSetPlayerName", RpcTarget.All, count, player.NickName);
                count++;
            }
            pv.RPC("RPC_MasterCallFirstDraw", RpcTarget.All);
        }
    }


    public void IncreaseGauge()
    {
        int gaugesCount = 0;
        foreach (GameObject child in summonGauges)
        {
            if (child.activeSelf)
            {
                gaugesCount++;
            }
        }
        if (gaugesCount < 15)
        {
            summonGauges[gaugesCount].SetActive(true);
        }
        else
        {
            LogWarning($"召喚格超出上限15格，無法再增加");
        }
    }
    public void LogWarning(string content)
    {
        warningTimer = Time.time;
        Warning.transform.GetChild(1).GetComponent<Text>().text = content;
        Warning.SetActive(true);
        showWarning = true;
    }
    public void LogGameLog(string content)
    {
        //GameLog content;
    }
    void CheckWarningClose()
    {
        if (Time.time > warningTimer + 3)
        {
            Warning.SetActive(false);
            showWarning = false;
        }
    }
    public bool PlayCard(CardInstance cardInstance)
    {
        bool playCard = false;
        if (tempBlockAction)
        {
            return false;
        }
        if (! (myInfo.waterToken > cardInstance.thisCard.costWater)
            || !(myInfo.earthToken > cardInstance.thisCard.costEarth)
            || !(myInfo.airToken > cardInstance.thisCard.costAir)
            || !(myInfo.fireToken > cardInstance.thisCard.costFire)
            || !(myInfo.crystal > cardInstance.thisCard.costPoint))
        {
        return false;
        }
        else
        {
            myInfo.waterToken = myInfo.waterToken - cardInstance.thisCard.costWater;
            myInfo.earthToken = myInfo.earthToken - cardInstance.thisCard.costEarth;
            myInfo.airToken = myInfo.airToken - cardInstance.thisCard.costAir;
            myInfo.fireToken = myInfo.fireToken - cardInstance.thisCard.costFire;
            myInfo.crystal = myInfo.crystal - cardInstance.thisCard.costPoint;
        }
        for (int i = 0; i < 19; i++)
        {
            if (summonGauges[i].activeSelf == true)
            {
                if (summonCards[i].activeSelf == false)
                {
                    summonCards[i].SetActive(true);
                    var sprite = Resources.Load<Sprite>($"Card\\Image\\{cardInstance.thisCard.cardID}");
                    summonCards[i].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = sprite;
                    playCard = true;
                    break;
                }
            }
        }
        UpdateMyInfo();
        return playCard;
    }
    public void IndexToCardInstance(int index, Transform cardZone)
    {
        Card card = cm.wholeCards[index - 1];
        GameObject go = Instantiate(UICardPrefab, cardZone);
        go.GetComponent<CardInstance>().thisCard = card;
        var sprite = Resources.Load<Sprite>($"Card\\Image\\{card.cardID}");
        go.transform.GetChild(1).GetComponent<Image>().sprite = sprite;
    }
    public int[] CardInstanceToIndexs(Transform cardZone)
    {
        int count = cardZone.childCount;
        int[] indexs = new int[count];
        for (int i = 0; i < count; i++)
        {
            indexs[i] = cardZone.GetChild(i).GetComponent<CardInstance>().thisCard.cardID;
        }
        return indexs;
    }
    public void DrawTempCard(CardInstance cardInstance, ref List<int> tempDrawingIndex)
    {
        int removeID = cardInstance.thisCard.cardID;
        tempDrawingIndex.Remove(removeID);
    }
    public void DrawCard()
    {
        int index = cm.DrawCardIndex(0);
        myInfo.handCardIndex.Add(index);
        pv.RPC("RPC_EveryoneRemoveCard", RpcTarget.All, 1);
        IndexToCardInstance(index, HandCardZone.transform);
    }
    public void UpdateMyInfo()
    {
        int diceSeason = 0;
        switch (myInfo.chosenDice.diceSeason)
        {
            case GameManager.Season.Winter:
                diceSeason = 1;
                break;
            case GameManager.Season.Spring:
                diceSeason = 2;
                break;
            case GameManager.Season.Summer:
                diceSeason = 3;
                break;
            case GameManager.Season.Fall:
                diceSeason = 4;
                break;
        }
        pv.RPC("RPC_EveryoneUpdateMyInfo", RpcTarget.All, myInfo.playerNo, myInfo.reserveSpace, myInfo.waterToken,
            myInfo.earthToken, myInfo.fireToken, myInfo.airToken, myInfo.crystal, myInfo.handCardCount,
            myInfo.bonusCost,myInfo.gaugeCount, myInfo.handCardIndex.ToArray(), myInfo.summonCardIndex.ToArray(), 
            myInfo.tempDrawingIndex.ToArray(), myInfo.finishAndWait, 
            myInfo.hasChosenDice, diceSeason, myInfo.chosenDice.diceNo, myInfo.chosenDice.sideValue);
    }
    public bool AllFinished()
    {
        foreach (PlayerInfo info in allInfos)
        {
            if (!info.finishAndWait)
            {
                return false;
            }
        }
        return true;
    }
    public void FinishConstructing()
    {
        if (HandCardZone.transform.childCount == 0 && constructingFirst.transform.GetChild(0).childCount == 3 &&
            constructingSecond.transform.GetChild(0).childCount == 3 && constructingThird.transform.GetChild(0).childCount == 3)
        {
            Text buttontext = constructingDeckCheckButton.transform.GetChild(0).GetComponent<Text>();
            buttontext.color = Color.green;
            buttontext.text = "已完成";
            firstYearDeck = CardInstanceToIndexs(constructingFirst.transform.GetChild(0));
            secondYearDeck = CardInstanceToIndexs(constructingSecond.transform.GetChild(0));
            thirdYearDeck = CardInstanceToIndexs(constructingThird.transform.GetChild(0));
            myInfo.finishAndWait = true;
            UpdateMyInfo();
            pv.RPC("RPC_MasterCheckStart", RpcTarget.MasterClient);
            gm.ProgressText.text = "等待其他玩家完成建構牌組。";
        }
    }
    public void IncreaseReserveSpace()
    {
        if (myInfo.reserveSpace < 10)
        {
            myInfo.reserveSpace++;
            GameObject emptySlot = tokenSets.transform.GetChild(myInfo.reserveSpace).GetChild(0).gameObject;
            emptySlot.SetActive(true);
        }
        else
        {
            LogWarning("元素儲存格已達最大上限10格");
        }
    }
    public void GetEnergyAndCheck(int getAsWish, int getWater, int getEarth, int getFire, int getAir)
    {
        int totalGet = getWater + getEarth + getFire + getAir + getAsWish;
        if (CheckHasEnoughSlot(totalGet))
        {
            if (getAsWish > 0)
            {
                tempBlockAction = true;
                asWishDecision.SetChooseWish(getAsWish, getWater, getEarth, getFire, getAir);
            }
            else
            {
                GetTokenAfterCheck(getWater, getEarth, getFire, getAir);
            }
        }
        else
        {
            tempBlockAction = true;
            int originHas = myInfo.waterToken + myInfo.earthToken + myInfo.fireToken + myInfo.airToken;
            reserveDecision.SetOriginGetElement(originHas, getAsWish, getWater, getEarth, getFire, getAir);
        }
    }
    bool CheckHasEnoughSlot(int getAmount)
    {
        int emptySlot = myInfo.reserveSpace - (myInfo.waterToken + myInfo.earthToken + myInfo.fireToken + myInfo.airToken);
        if (emptySlot > getAmount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void GetTokenAfterCheck(int getWater, int getEarth, int getFire, int getAir)
    {
        if (getWater > 0)
        {
            for (int i = 0; i < getWater; i++)
            {
                InstantiateToken(waterTokenPrefab);
                myInfo.waterToken++;
            }
        }
        if (getEarth > 0)
        {
            for (int i = 0; i < getEarth; i++)
            {
                InstantiateToken(earthTokenPrefab);
                myInfo.earthToken++;
            }
        }
        if (getFire > 0)
        {
            for (int i = 0; i < getFire; i++)
            {
                InstantiateToken(fireTokenPrefab);
                myInfo.fireToken++;
            }
        }
        if (getAir > 0)
        {
            for (int i = 0; i < getAir; i++)
            {
                InstantiateToken(airTokenPrefab);
                myInfo.airToken++;
            }
        }
    }
    public void InstantiateToken(GameObject tokenPrefab)
    {
        bool foundEmpty = false;
        for (int i = 0; i < 10; i++)
        {
            if (tokenSets.transform.GetChild(i).GetChild(0).gameObject.activeSelf)
            {
                Transform parent = tokenSets.transform.GetChild(i);
                GameObject emptySlot = parent.GetChild(0).gameObject;
                Instantiate(tokenPrefab, parent);
                emptySlot.SetActive(false);
                foundEmpty = true;
                break;
            }
        }
        if (!foundEmpty)
        {
            Debug.LogError("Active Empty Slot Not Found");
        }
    }
    public void UseOrDiscardEnergy(int element, int amount)
    {
        string s = "";
        switch (element)
        {
            case 1:
                s = "Water";
                break;
            case 2:
                s = "Earth";
                break;
            case 3:
                s = "Fire";
                break;
            case 4:
                s = "Air";
                break;
            default:
                Debug.LogError("Wrong element Index in Use Or Discard Energy");
                break;
        }
        for (int i = 0; i < amount; i++)
        {
            GameObject tbd = GameObject.FindGameObjectWithTag($"{s}Token");
            GameObject empty = tbd.transform.parent.GetChild(0).gameObject;
            empty.SetActive(true);
            Destroy(tbd);
        }
    }
    public void GetCrystal (int amount)
    {
        myInfo.crystal += amount;
    }
    public void ChooseDice(SeasonDice dice)
    {
        var rotation = dice.transform.rotation;
        myInfo.chosenDice = dice;
        myInfo.hasChosenDice = true;
        var prefab = Resources.Load<GameObject>($"SeasonDice{dice.diceSeason.ToString()}{dice.diceNo}");
        GameObject myChosenDiceGo = Instantiate(prefab, Vector3.zero, Quaternion.identity, dm.myChosenDicePos);
        myChosenDiceGo.transform.localPosition = Vector3.zero;
        myChosenDiceGo.transform.localRotation = rotation;
        myChosenDiceGo.GetComponent<SeasonDice>().diceSeason = dice.diceSeason;
        myChosenDiceGo.GetComponent<SeasonDice>().diceNo = dice.diceNo;
        myChosenDiceGo.GetComponent<SeasonDice>().sideValue = dice.sideValue;
        myChosenDiceGo.GetComponent<SeasonDiceGO>().thisDice = myChosenDiceGo.GetComponent<SeasonDice>();

        var tbdPV = myChosenDiceGo.GetComponent<PhotonView>();
        Destroy(tbdPV);
        var tbdPTV = myChosenDiceGo.GetComponent<PhotonTransformView>();
        Destroy(tbdPTV);
        //測試用
        var sd = myChosenDiceGo.GetComponent<SeasonDiceGO>().thisDice;
        Debug.Log($"My chosen dice info : season {sd.diceSeason.ToString()} no {sd.diceNo} value {sd.sideValue} ");
        UpdateMyInfo();
        pv.RPC("RPC_ChangeTurn", RpcTarget.All);
    }

    private SeasonDiceSideInfo chosenDiceInfo;
    public void CallDiceFunction(SeasonDice chosenDice)
    {
        chosenDiceInfo = dm.GetDiceSideInfo(chosenDice.diceSeason, chosenDice.diceNo, chosenDice.sideValue);
        if (chosenDiceInfo.gauge)
        {
            gm.performDice += DiceFunctionGauge;
        }
        if (chosenDiceInfo.transmute)
        {
            gm.performDice += DiceFunctionTransmute;
        }
        if (chosenDiceInfo.drawCard)
        {
            gm.performDice += DiceFunctionDraw;
        }
        if (chosenDiceInfo.getWater + chosenDiceInfo.getEarth + chosenDiceInfo.getFire + chosenDiceInfo.getAir != 0)
        {
            gm.performDice += DiceFunctionGetEnergy;
        }
        if (chosenDiceInfo.crystals != 0)
        {
            gm.performDice += DiceFunctionGetCrystal;
        }
    }
    public void DiceFunctionGauge()
    {
        IncreaseGauge();
    }
    public void DiceFunctionTransmute()
    {
        canTransmute = true;
        Debug.Log("Dice Function Can Transmute");
        //待修改: if can transmute > token shold be selectable;
    }
    public void DiceFunctionDraw()
    {
        DrawCheck.SetActive(true);
        bonusButtons[3].interactable = true;
    }
    public void DiceFunctionGetEnergy()
    {
        GetEnergyAndCheck(0, chosenDiceInfo.getWater, chosenDiceInfo.getEarth, chosenDiceInfo.getFire, chosenDiceInfo.getAir);
    }
    public void DiceFunctionGetCrystal()
    {
        GetCrystal(chosenDiceInfo.crystals);
    }
    public void BonusDrawTwoCard()
    {
        //待修改 : 抽兩張牌
        //加入choosing card zone (卡片效果抽牌也可以用，最多放四張)
    }
    public void FinishAction()
    {
        FinishActionButton.SetActive(false);
        canTransmute = false;
        myAction = false;
        pv.RPC("RPC_ChangeTurn", RpcTarget.All);
    }


    [PunRPC]
    public void RPC_ChangeTurn() 
    {
        gm.ChangeTurn();
    }
    [PunRPC]
    public void RPC_ChangeFirstDrawDeck()      //輪抽換抽牌堆
    {
        gm.firstDrawRound++;
        myInfo.finishAndWait = false; //完成抽牌狀態設為否

        foreach(PlayerInfo info in allInfos)    //每個玩家的抽牌狀態為否
        {
            info.finishAndWait = false;
        }

        for (int i = 0; i < DrawCardZone.transform.childCount; i++)      //刪除現有抽牌區的子物件(待抽牌)
        {
            GameObject go = DrawCardZone.transform.GetChild(i).gameObject;
            Destroy(go);
        }

        if (gm.firstDrawRound <= 9)
        {
            if (myInfo.playerNo < playerCountInRoom - 1)
            {
                myInfo.tempDrawingIndex = allInfos[myInfo.playerNo + 1].tempDrawingIndex;
            }
            else
            {
                myInfo.tempDrawingIndex = allInfos[0].tempDrawingIndex;
            }
            foreach (int i in myInfo.tempDrawingIndex)
            {
                IndexToCardInstance(i, DrawCardZone.transform);
            }
            isDrawingCard = true;        //玩家回復可抽牌狀態
        }
        else
        {
            isDrawingCard = false;
            DrawCardZone.SetActive(false);
            gm.inFirstDrawing = false;
            Debug.Log("finish first drawing");
            if (PhotonNetwork.IsMasterClient)
            {
                pv.RPC("RPC_MasterCallConstructingDeck", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    void RPC_MasterInitAllDrawingDeck(int[] indexArray)
    {
        cm.drawDeck = indexArray.ToList();
    }

    [PunRPC]
    void RPC_MasterSetPlayerName(int playerNo, string playerName)
    {
        allInfos[playerNo].playerNo = playerNo;
        allInfos[playerNo].playerName = playerName;
        if (playerName == myInfo.playerName)
        {
            myInfo.playerNo = playerNo;
            myInfoNameText.text = playerName;
        }
        else
        {
            foreach (GameObject info in oppInfoInstances)
            {
                InfoInstance infoInstance = info.GetComponent<InfoInstance>();
                if (!infoInstance.hasSetPlayer)
                {
                    infoInstance.hasSetPlayer = true;
                    infoInstance.OppName.text = playerName;
                    infoInstance.thisOppNo = playerNo;
                    info.SetActive(true);
                    return;
                }
            }
        }
    }

    [PunRPC]
    void RPC_MasterCallFirstDraw()
    {
        Debug.Log("Start First Draw");
        gm.ChangeProgressText(true, "FirstDraw");
        gm.firstDrawRound = 1;
        gm.inFirstDrawing = true;
        isDrawingCard = true;
        myInfo.finishAndWait = false;
        DrawCardZone.SetActive(true);
        int startIndex = myInfo.playerNo * 9;
        for (int i = startIndex; i < startIndex + 9; i++)   //一號玩家抽0~8號牌;二號抽9~17以此類推
        {
            myInfo.tempDrawingIndex.Add(cm.DrawCardIndex(i));
        }
        for (int i = 0; i < playerCountInRoom * 9; i++)     //刪除所有被抽取的牌
        {
            cm.RemoveFirstFromDrawDeck();
        }
        drawDeckCount.text = cm.drawDeck.Count.ToString();
        foreach (int i in myInfo.tempDrawingIndex)
        {
            IndexToCardInstance(i, DrawCardZone.transform);
        }
    }

    [PunRPC]
    void RPC_EveryoneUpdateMyInfo(int playerNo, int reserveSpace, int waterToken, int earthToken, int fireToken,
        int airToken, int crystal,int handCardCount,int bonusCost, int gaugeCount,
        int[] handCardIndex, int[] summonCardIndex, int[] tempDrawingDeckIndex,
        bool finishPickedAndWait,bool hasChosenDice,
        int chosenDiceSeason, int chosenDiceNo, int chosenDiceSideValue)
    {
        var playerInfo = allInfos[playerNo];
        playerInfo.reserveSpace = reserveSpace;
        playerInfo.waterToken = waterToken;
        playerInfo.earthToken = earthToken;
        playerInfo.fireToken = fireToken;
        playerInfo.airToken = airToken;
        playerInfo.crystal = crystal;
        playerInfo.handCardCount = handCardCount;
        playerInfo.bonusCost = bonusCost;
        playerInfo.gaugeCount = gaugeCount;
        playerInfo.handCardIndex = handCardIndex.ToList();
        playerInfo.summonCardIndex = summonCardIndex.ToList();
        playerInfo.tempDrawingIndex = tempDrawingDeckIndex.ToList();
        playerInfo.finishAndWait = finishPickedAndWait;

        GameManager.Season season = GameManager.Season.Winter;
        switch(chosenDiceSeason)
        {
            case 1:
                season = GameManager.Season.Winter;
                break;
            case 2:
                season = GameManager.Season.Spring;
                break;
            case 3:
                season = GameManager.Season.Summer;
                break;
            case 4:
                season = GameManager.Season.Fall;
                break;
        }

        playerInfo.hasChosenDice = hasChosenDice;
        

        if (!(playerNo == myInfo.playerNo))
        {
            foreach (GameObject info in oppInfoInstances)
            {
                InfoInstance infoInstance = info.GetComponent<InfoInstance>();
                if (infoInstance.thisOppNo == playerNo)
                {
                    infoInstance.CrystalText.text = crystal.ToString();
                    infoInstance.BonusText.text = bonusCost.ToString();
                    infoInstance.HandCardText.text = handCardCount.ToString();
                    infoInstance.GaugeText.text = gaugeCount.ToString();

                    if (playerInfo.hasChosenDice)
                    {
                        SeasonDice sDice = infoInstance.chosenDice.GetComponent<SeasonDice>();
                        playerInfo.chosenDice = sDice;
                        playerInfo.chosenDice.diceSeason = season;
                        playerInfo.chosenDice.diceNo = chosenDiceNo;
                        playerInfo.chosenDice.sideValue = chosenDiceSideValue;
                        sDice = playerInfo.chosenDice;
                        string fileName = $"{sDice.diceSeason.ToString().ToLower()}{sDice.diceNo}_{sDice.sideValue}";
                        infoInstance.chosenDice.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Dice\\Textures\\Upward\\{fileName}");
                        infoInstance.chosenDice.SetActive(true);
                    }
                    else
                    {
                        infoInstance.chosenDice.SetActive(false);
                    }
                    
                    if (!playerInfo.finishAndWait)
                    {
                        infoInstance.Hourglass.SetActive(true);
                    }
                    else
                    {
                        infoInstance.Hourglass.SetActive(false);
                    }
                    if (gm.roundStartFrom == playerNo)
                    {
                        infoInstance.RoundStartCube.SetActive(true);
                    }
                    else
                    {
                        infoInstance.RoundStartCube.SetActive(false);
                    }
                    for (int i = 0; i < reserveSpace; i++)
                    {
                        infoInstance.ElementSlots[i].SetActive(true);
                        infoInstance.ElementSlots[i].GetComponent<Image>().sprite = emptyTokenSprite;
                    }
                    int elementCount = 0;
                    for (int i = 0; i < waterToken; i++)
                    {
                        infoInstance.ElementSlots[elementCount].GetComponent<Image>().sprite = waterTokenSprite;
                        elementCount++;
                    }
                    for (int i = 0; i < earthToken; i++)
                    {
                        infoInstance.ElementSlots[elementCount].GetComponent<Image>().sprite = earthTokenSprite;
                        elementCount++;
                    }
                    for (int i = 0; i < fireToken; i++)
                    {
                        infoInstance.ElementSlots[elementCount].GetComponent<Image>().sprite = fireTokenSprite;
                        elementCount++;
                    }
                    for (int i = 0; i < airToken; i++)
                    {
                        infoInstance.ElementSlots[elementCount].GetComponent<Image>().sprite = airTokenSprite;
                        elementCount++;
                    }
                }
            }
        }
        else
        {
            myInfoBonusText.text = myInfo.bonusCost.ToString();
            myInfoCrystalText.text = myInfo.crystal.ToString();
        }
    }

    [PunRPC]
    void RPC_EveryoneRemoveCard(int howMany)
    {
        for (int i = 0; i < howMany; i++)
        {
            cm.RemoveFirstFromDrawDeck();
        }
        drawDeckCount.text = cm.drawDeck.Count.ToString();
    }

    [PunRPC]
    void RPC_MasterCheckChange()
    {
        if (AllFinished())
        {
            Debug.Log("All picked");
            pv.RPC("RPC_ChangeFirstDrawDeck", RpcTarget.All);
        }
    }

    [PunRPC]
    void RPC_MasterCheckStart()
    {
        if (AllFinished())
        {
            Debug.Log("所有玩家已完成建構牌組");
            int rs = UnityEngine.Random.Range(0, playerCountInRoom);
            pv.RPC("RPC_MasterCallFinishConstructingDeck", RpcTarget.All, rs);
        }
    }

    [PunRPC]
    void RPC_MasterCallConstructingDeck()
    {
        gm.inConstructingDeck = true;
        gm.ChangeProgressText(true, "Constructing");
        constructingDeck.SetActive(true);
    }

    [PunRPC]
    void RPC_MasterCallFinishConstructingDeck(int roundStartfrom)
    {
        gm.inConstructingDeck = false;
        constructingDeck.SetActive(false);
        foreach (int i in firstYearDeck)
        {
            IndexToCardInstance(i, HandCardZone.transform);
        }
        Array.Clear(firstYearDeck, 0, firstYearDeck.Length);
        UpdateMyInfo();
        gm.startRound = true;
        gm.inRollPhase = true;
        gm.roundStartFrom = roundStartfrom;
        gm.whoseTurn = roundStartfrom;
        gm.roundCount = 1;
    }

    [PunRPC]
    void RPC_MasterRollDice()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            dm.Roll(gm.season);
        }
    }
    [PunRPC]
    void RPC_MasterCallDiceHasRolled()
    {
        dm.SetRolledDicePos();
    }
    [PunRPC]
    void RPC_ResetChosenDice()
    {
        GameObject chosenDice = dm.myChosenDicePos.GetChild(0).gameObject;
        Destroy(chosenDice);
        myInfo.hasChosenDice = false;
        UpdateMyInfo();
    }


}
