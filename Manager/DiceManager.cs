using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DiceManager : MonoBehaviour
{
	private int[] winterDicesNo, springDicesNo, summerDicesNo, fallDicesNo;
    int playerCountInRoom;
    public Transform rollPos, CubePos, myChosenDicePos;
    public GameObject diceViewDefault;
    private static DiceManager dm;
    private static GameManager gm;

    public GameObject[] rollingDices;
    private Transform leftDice;
    private Player player;

    public static DiceManager DM
    {
        get
        {
            if (dm == null)
            {
                dm = FindObjectOfType(typeof(DiceManager)) as DiceManager;
                if (dm == null)
                {
                    GameObject go = new GameObject("dm");
                    dm = go.AddComponent<DiceManager>();
                }
            }
            return dm;
        }
    }

    private void Awake()
    {
        playerCountInRoom = PhotonNetwork.CurrentRoom.PlayerCount;
        rollingDices = new GameObject[playerCountInRoom + 1];
        gm = GameManager.GM;
        player = gm.myPlayer;
    }

    int[] InitSeasonDiceNo()
    {
        int[] addArray = new int[playerCountInRoom + 1];
        for (int i = 0; i < addArray.Length;)
        {
            bool uniqueNum = true;
            int randomNum = UnityEngine.Random.Range(1, 6);
            for (int j = 0; j < i; j++)
            {
                if (randomNum == addArray[j])
                {
                    uniqueNum = false;
                    break;
                }
            }
            if (uniqueNum)
            {
                addArray[i] = randomNum;
                i++;
            }
        }
        return addArray;
    }
    public void InitSeasonDice()
    {
        winterDicesNo = InitSeasonDiceNo();
        springDicesNo = InitSeasonDiceNo();
        summerDicesNo = InitSeasonDiceNo();
        fallDicesNo = InitSeasonDiceNo();
    }

    public bool rolling;

	public void Roll(GameManager.Season season)
	{
        rolling = true;
        GameObject currentDice;
        int count = 0;
        switch (season)
        {
            case GameManager.Season.Winter:
                foreach (int i in winterDicesNo)
                {
                    string prefabName = $"SeasonDice{season.ToString()}{i}";
                    currentDice = PhotonNetwork.Instantiate(prefabName, rollPos.position, Quaternion.identity);
                    //測試用
                    //currentDice.transform.SetParent(rollPos);
                    //currentDice.transform.localPosition = Vector3.zero;
                    currentDice.GetComponent<Rigidbody>().AddForce(new Vector3(UnityEngine.Random.Range(-20, 20), UnityEngine.Random.Range(-20, 20), UnityEngine.Random.Range(0, 30)));
                    currentDice.GetComponent<SeasonDiceGO>().dicePV.RPC("RPC_SetRollingNo", RpcTarget.All, count);
                    count++;
                }
                break;
            case GameManager.Season.Spring:
                foreach (int i in springDicesNo)
                {
                    string prefabName = $"SeasonDice{season.ToString()}{i}";
                    currentDice = PhotonNetwork.Instantiate(prefabName, rollPos.position, Quaternion.identity) as GameObject;
                    currentDice.GetComponent<Rigidbody>().AddForce(new Vector3(UnityEngine.Random.Range(-20, 20), UnityEngine.Random.Range(-20, 20), UnityEngine.Random.Range(0, 30)));
                    currentDice.GetComponent<SeasonDiceGO>().dicePV.RPC("RPC_SetRollingNo", RpcTarget.All, count);
                    count++;
                }
                break;
            case GameManager.Season.Summer:
                foreach (int i in summerDicesNo)
                {
                    string prefabName = $"SeasonDice{season.ToString()}{i}";
                    currentDice = PhotonNetwork.Instantiate(prefabName, rollPos.position, Quaternion.identity) as GameObject;
                    currentDice.GetComponent<Rigidbody>().AddForce(new Vector3(UnityEngine.Random.Range(-20, 20), UnityEngine.Random.Range(-20, 20), UnityEngine.Random.Range(0, 30)));
                    currentDice.GetComponent<SeasonDiceGO>().dicePV.RPC("RPC_SetRollingNo", RpcTarget.All, count);
                    count++;
                }
                break;
            case GameManager.Season.Fall:

                foreach (int i in fallDicesNo)
                {
                    string prefabName = $"SeasonDice{season.ToString()}{i}";
                    currentDice = PhotonNetwork.Instantiate(prefabName, rollPos.position, Quaternion.identity) as GameObject;
                    currentDice.GetComponent<Rigidbody>().AddForce(new Vector3(UnityEngine.Random.Range(-20, 20), UnityEngine.Random.Range(-20, 20), UnityEngine.Random.Range(0, 30)));
                    currentDice.GetComponent<SeasonDiceGO>().dicePV.RPC("RPC_SetRollingNo", RpcTarget.All, count);
                    count++;
                }
                break;
        }
        gm.hasRolled = true;
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && rolling)
        {
            if (CheckDicesStop())
            {
                rolling = false;
                player.pv.RPC("RPC_MasterCallDiceHasRolled", RpcTarget.All);
                Debug.Log("Dices has stopped");
            }
        }
    }
    bool CheckDicesStop()
    {
        foreach (GameObject dice in rollingDices)
        {
            if (!dice.GetComponent<SeasonDiceGO>().hasValue)
            {
                return false;
            }
        }
        return true;
    }
    public SeasonDiceSideInfo GetDiceSideInfo(GameManager.Season season, int diceNo, int sideNo)
    {
        SeasonDiceSideInfo sideInfo = new SeasonDiceSideInfo();
        switch (season)
        {
            case GameManager.Season.Winter:
                switch (diceNo)
                {
                    case 1: //冬骰1
                        switch (sideNo)
                        {
                            case 1:
                                sideInfo.gauge = true;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 1;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 2:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 2;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 3:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 2;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 3;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 4:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 2;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 5:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = true;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 6:
                                sideInfo.gauge = false;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 1;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 1;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                        }
                        break;
                    case 2: //冬骰2
                        switch (sideNo)
                        {
                            case 1:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 2;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 2:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 2;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 1;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 3:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 2;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 4:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = true;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 5:
                                sideInfo.gauge = false;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 1;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 1;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 6:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 1;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                        }
                        break;
                    case 3: //冬骰3
                        switch (sideNo)
                        {
                            case 1:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 2;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 2:
                                sideInfo.gauge = true;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 1;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 3:
                                sideInfo.gauge = false;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 1;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 1;
                                sideInfo.crystals = 3;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 4:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = true;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 5:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 1;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 6:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 2;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 3;
                                sideInfo.moveSpaces = 3;
                                break;
                        }
                        break;
                    case 4: //冬骰4
                        switch (sideNo)
                        {
                            case 1:
                                sideInfo.gauge = false;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 1;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 1;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 2:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 2;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 2;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 3:
                                sideInfo.gauge = true;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 1;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 4:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 2;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 5:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 6;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 6:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 1;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                        }
                        break;
                    case 5: //冬骰5
                        switch (sideNo)
                        {
                            case 1:
                                sideInfo.gauge = false;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 2;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 2:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 6;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 3:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 1;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 4:
                                sideInfo.gauge = true;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 1;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 5:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 2;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 6:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 2;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 1;
                                sideInfo.moveSpaces = 2;
                                break;
                        }
                        break;
                    default:
                        Debug.LogError("Set Dice Ifno Wrong");
                        break;
                }
                break;
            case GameManager.Season.Spring:
                switch (diceNo)
                {
                    case 1: //春骰1
                        switch (sideNo)
                        {
                            case 1:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = true;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 2:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 2;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 3:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 2;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 3;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 4:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 2;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 5:
                                sideInfo.gauge = true;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 1;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 6:
                                sideInfo.gauge = false;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 1;
                                sideInfo.getEarth = 1;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                        }
                        break;
                    case 2: //春骰2
                        switch (sideNo)
                        {
                            case 1:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 2;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 2:
                                sideInfo.gauge = true;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 1;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 3:
                                sideInfo.gauge = false;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 1;
                                sideInfo.getEarth = 1;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 4:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = true;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 5:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 1;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 6:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 2;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 3;
                                sideInfo.moveSpaces = 3;
                                break;
                        }
                        break;
                    case 3: //春骰3
                        switch (sideNo)
                        {
                            case 1:
                                sideInfo.gauge = false;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 1;
                                sideInfo.getEarth = 1;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 2:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 2;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 2;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 3:
                                sideInfo.gauge = true;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 1;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 4:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 2;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 5:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 6;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 6:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 1;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                        }
                        break;
                    case 4: //春骰4
                        switch (sideNo)
                        {
                            case 1:
                                sideInfo.gauge = true;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 1;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 2:
                                sideInfo.gauge = false;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 2;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 3:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 6;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 4:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 1;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 5:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 2;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 1;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 6:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 2;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                        }
                        break;
                    case 5: //春骰5
                        switch (sideNo)
                        {
                            case 1:
                                sideInfo.gauge = true;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 1;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 2:
                                sideInfo.gauge = false;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 1;
                                sideInfo.getEarth = 1;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 3:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = true;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 4:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 2;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 5:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 2;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 1;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 6:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 2;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                        }
                        break;
                    default:
                        Debug.LogError("Set Dice Ifno Wrong");
                        break;
                }
                break;
            case GameManager.Season.Summer:
                switch (diceNo)
                {
                    case 1: //夏骰1
                        switch (sideNo)
                        {
                            case 1:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = true;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 2:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 2;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 3:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 2;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 3;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 4:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 2;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 5:
                                sideInfo.gauge = true;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 1;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 6:
                                sideInfo.gauge = false;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 1;
                                sideInfo.getFire = 1;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                        }
                        break;
                    case 2: //夏骰2
                        switch (sideNo)
                        {
                            case 1:
                                sideInfo.gauge = false;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 1;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 1;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 2:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = true;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 3:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 2;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 4:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 2;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 1;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 5:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 1;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 2;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 6:
                                sideInfo.gauge = true;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 1;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                        }
                        break;
                    case 3: //夏骰3
                        switch (sideNo)
                        {
                            case 1:
                                sideInfo.gauge = true;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 1;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 2:
                                sideInfo.gauge = false;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 1;
                                sideInfo.getFire = 1;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 3:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = true;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 4:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 1;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 5:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 2;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 3;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 6:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 2;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                        }
                        break;
                    case 4: //夏骰4
                        switch (sideNo)
                        {
                            case 1:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 2;
                                sideInfo.crystals = 2;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 2:
                                sideInfo.gauge = true;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 1;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 3:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 2;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 4:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 6;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 5:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 1;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 6:
                                sideInfo.gauge = false;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 1;
                                sideInfo.getFire = 1;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                        }
                        break;
                    case 5: //夏骰5
                        switch (sideNo)
                        {
                            case 1:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 2;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 2:
                                sideInfo.gauge = true;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 1;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 3:
                                sideInfo.gauge = false;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 2;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 4:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 6;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 5:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 1;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 6:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 2;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 1;
                                sideInfo.moveSpaces = 2;
                                break;
                        }
                        break;
                    default:
                        Debug.LogError("Set Dice Ifno Wrong");
                        break;
                }
                break;
            case GameManager.Season.Fall:
                switch (diceNo)
                {
                    case 1: //秋骰1
                        switch (sideNo)
                        {
                            case 1:
                                sideInfo.gauge = false;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 1;
                                sideInfo.getAir = 1;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 2:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = true;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 3:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 2;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces =3;
                                break;
                            case 4:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 2;
                                sideInfo.crystals = 3;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 5:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 2;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 6:
                                sideInfo.gauge = true;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 1;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                        }
                        break;
                    case 2: //秋骰2
                        switch (sideNo)
                        {
                            case 1:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 2;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 2:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 1;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 3:
                                sideInfo.gauge = false;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 1;
                                sideInfo.getAir = 1;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 4:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = true;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 5:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 1;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 6:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 2;
                                sideInfo.crystals = 3;
                                sideInfo.moveSpaces = 3;
                                break;
                        }
                        break;
                    case 3: //秋骰3
                        switch (sideNo)
                        {
                            case 1:
                                sideInfo.gauge = false;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 1;
                                sideInfo.getAir = 1;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 2:
                                sideInfo.gauge = false;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 2;
                                sideInfo.crystals = 2;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 3:
                                sideInfo.gauge = true;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 1;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 4:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 2;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 5:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 6;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 6:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 1;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                        }
                        break;
                    case 4: //秋骰4
                        switch (sideNo)
                        {
                            case 1:
                                sideInfo.gauge = false;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 2;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 2:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 6;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 3:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 1;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 4:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 2;
                                sideInfo.crystals = 1;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 5:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 2;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 6:
                                sideInfo.gauge = true;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 1;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                        }
                        break;
                    case 5: //秋骰5
                        switch (sideNo)
                        {
                            case 1:
                                sideInfo.gauge = true;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 1;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 2:
                                sideInfo.gauge = false;
                                sideInfo.transmute = true;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 1;
                                sideInfo.getAir = 1;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 3:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = true;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                            case 4:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 2;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 0;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 1;
                                break;
                            case 5:
                                sideInfo.gauge = false;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 1;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 2;
                                break;
                            case 6:
                                sideInfo.gauge = true;
                                sideInfo.transmute = false;
                                sideInfo.drawCard = false;
                                sideInfo.getWater = 0;
                                sideInfo.getEarth = 0;
                                sideInfo.getFire = 0;
                                sideInfo.getAir = 2;
                                sideInfo.crystals = 0;
                                sideInfo.moveSpaces = 3;
                                break;
                        }
                        break;
                    default:
                        Debug.LogError("Set Dice Ifno Wrong");
                        break;
                }
                break;
        }
        return sideInfo;
    }
    public void SetRolledDicePos()
    {
        int count = 0;
        foreach (GameObject dice in rollingDices)
        {
            dice.transform.SetParent(CubePos.transform.GetChild(count));
            switch (dice.GetComponent<SeasonDice>().sideValue)
            {
                case 1:
                    dice.transform.rotation = Quaternion.Euler(-90f, 0, 180f);
                    break;
                case 2:
                    dice.transform.rotation = Quaternion.Euler(180f, 90f, 0);
                    break;
                case 3:
                    dice.transform.rotation = Quaternion.Euler(0, 180f, -90f);
                    break;
                case 4:
                    dice.transform.rotation = Quaternion.Euler(0, 180f, 90f);
                    break;
                case 5:
                    dice.transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case 6:
                    dice.transform.rotation = Quaternion.Euler(90, 0, -90);
                    break;
                default:
                    Debug.LogError("Wrong in setting dice rotation");
                    break;
            }
            dice.transform.localPosition = Vector3.zero;
            dice.GetComponent<SeasonDiceGO>().rollingNo = count;
            count++;
        }
    }
    public void SetLeftDicePos()
    {
        foreach (Transform pos in CubePos)
        {
            if (pos.childCount != 0)
            {
                leftDice = pos.GetChild(0);
                leftDice.SetParent(CubePos.GetChild(4));
                leftDice.localPosition = Vector3.zero;
            }
        }
    }
    public int UseLeftDiceAndMove()
    {
        SeasonDice sdice = leftDice.GetComponent<SeasonDice>();
        int moveSpace = GetDiceSideInfo(sdice.diceSeason, sdice.diceNo, sdice.sideValue).moveSpaces;
        leftDice.GetComponent<SeasonDiceGO>().dicePV.RPC("RPC_DestroyThisDice", RpcTarget.All);
        player.pv.RPC("RPC_ResetChosenDice", RpcTarget.All);
        return moveSpace;
    }
}
