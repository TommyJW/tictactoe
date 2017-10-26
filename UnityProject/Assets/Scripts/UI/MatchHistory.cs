using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
/*
Screen script for the Match History canvas
every public member is located on the UI
    */
public class MatchHistory : MonoBehaviour {

    public GameObject playerNameBtnPrefab;
    public GameObject myPlayerListGO;
    public GameObject myPlayerPanelGO;

    public GameObject matchRecordPrefab;
    public GameObject myRecordListGO;
    public GameObject myRecordPanelGO;

    private bool triggerOn = false;
    private bool wasOnLastFrame = false;
    private enum MY_SCREEN_MODES { SHOW_PLAYERS, SHOW_MATCH}
    private MY_SCREEN_MODES myScreenMode;
    private List<string> playerUIDs;
    private List<string> gameRecordUIDs;
    private List<GameObject> playerRecordGOs;
    private List<GameObject> gameRecordGOs;
    // Use this for initialization
    void Start () {
        myScreenMode = MY_SCREEN_MODES.SHOW_PLAYERS;
        gameRecordGOs = new List<GameObject>();
        playerRecordGOs = new List<GameObject>();
        gameRecordUIDs = new List<string>();
        playerUIDs = new List<string>();

    }
	
	// Update is called once per frame
	void Update () {
        // determine if this is an initial load if yes, then clear the game records and turn the trigger on
        if (UIManager.Singleton.matchHistory.enabled && !wasOnLastFrame)
        {
            triggerOn = true;
            playerUIDs.Clear();
            foreach (GameObject go in playerRecordGOs)
            {
                GameObject.Destroy(go);
            }
        }

        //one time fire that builds the game objects to be shown in the player list
        if (triggerOn)
        {
            foreach (PlayerProfile pp in GameManager.Singleton.myPlayerHistory.playerProfiles)
            {
                if (!playerUIDs.Contains(pp.UID))
                {
                    playerUIDs.Add(pp.UID);
                    GameObject temp = GameObject.Instantiate(playerNameBtnPrefab);
                    temp.transform.SetParent(myPlayerListGO.transform, false);
                    playerRecordGOs.Add(temp);
                    Text tempText = temp.GetComponentInChildren<Text>();
                    tempText.text = pp.playerName;
                    PlayerRecordBtn tempPB = temp.GetComponent<PlayerRecordBtn>();
                    tempPB.setPlayerProfile(pp);
                    PlayerRecordBtn tempPBPR = tempPB.GetComponent<PlayerRecordBtn>();
                    tempPBPR.BuildConnections();
                    pp.CalculateTotals();
                    int[] winlosstie = pp.getWinLossTie();
                    string pName = pp.playerName;
                    string pWins = winlosstie[0].ToString();
                    string pLosses = winlosstie[1].ToString();
                    string pTies = winlosstie[2].ToString();
                    tempPBPR.updateFields(pp.playerName, winlosstie[0].ToString(), winlosstie[1].ToString(), winlosstie[2].ToString());
                }
            }
            triggerOn = false;
        }
        wasOnLastFrame = UIManager.Singleton.matchHistory.enabled;
    }

    //user clicked on a profile, show all the matches for that player
    public void ClickedProfile(PlayerProfile pp)
    {
        //clear any old data
        foreach(GameObject go in gameRecordGOs)
        {
            GameObject.Destroy(go);
        }
        gameRecordGOs.Clear();
        gameRecordUIDs.Clear();

        //build the new list of gameobjects that hold the match history info and put them into the hidden list
        foreach(GameRecord gr in pp.getTheGameRecords())
        {
            if(!gameRecordUIDs.Contains(gr.UID))
            {
                gameRecordUIDs.Add(gr.UID);

                GameObject temp = GameObject.Instantiate(matchRecordPrefab);
                temp.transform.SetParent(myRecordListGO.transform, false);

                MatchRecordBtn tempMRB = temp.GetComponent<MatchRecordBtn>();
                tempMRB.BuildConnections();
                string p1Name = GameManager.Singleton.myPlayerHistory.retrievePlayerNameByUID(gr.player1UID);
                string p2Name = GameManager.Singleton.myPlayerHistory.retrievePlayerNameByUID(gr.player2UID);

                string theWinner = "";
                switch(gr.playerWinner)
                {
                    case GameRecord.WINNER_SELECT.PLAYER1:
                        theWinner = p1Name;
                        break;
                    case GameRecord.WINNER_SELECT.PLAYER2:
                        theWinner = p2Name;
                        break;
                    case GameRecord.WINNER_SELECT.TIE:
                        theWinner = "TIE";
                        break;
                }
                tempMRB.updateFields(theWinner, p1Name, p2Name);

                gameRecordGOs.Add(temp);
            }
        }

        //change which list is shown
        myPlayerPanelGO.SetActive(false);
        myRecordPanelGO.SetActive(true);
        myScreenMode = MY_SCREEN_MODES.SHOW_MATCH;
    }

    //user hit back button, if showing the match history, instead show players, if showing players, instead go back to main menu
    public void BackBtn()
    {
        switch (myScreenMode)
        {
            case MY_SCREEN_MODES.SHOW_PLAYERS:
                UIManager.Singleton.mainMenu.enabled = true;
                UIManager.Singleton.matchHistory.enabled = false;
                break;
            case MY_SCREEN_MODES.SHOW_MATCH:
                myPlayerPanelGO.SetActive(true);
                myRecordPanelGO.SetActive(false);
                myScreenMode = MY_SCREEN_MODES.SHOW_PLAYERS;
                break;
        }
    }
}
