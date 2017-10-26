using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
/*
Screen script for the Game play canvas
every public member is located on the UI
    */
public class SelectPlayer : MonoBehaviour {

    public Text myTitle;
    public Text myEnteredPlayerName;
    public GameObject myListGO;
    public GameObject playerNameBtnPrefab;
    public enum MY_SCREEN_MODES { ONE_PLAYER, TWO_PLAYER}
    public MY_SCREEN_MODES myMode;

    private List<string> playerUIDs;

    private bool triggerOn = false;
    private bool wasOnLastFrame = false;
    private enum MY_SCREEN_STATE { AWAIT_P1, AWAIT_P2, READY_FOR_PLAY };
    private MY_SCREEN_STATE myScreenState = MY_SCREEN_STATE.AWAIT_P1;
    private string myP1Title = "Select or Enter Player 1";
    private string myP2Title = "Select or Enter Player 2";

	// Use this for initialization
    /*
        Will need to populate the GUI list when all the profiles are loaded.
    */
	void Start () {
        playerUIDs = new List<string>();
	}
	
	// Update is called once per frame
    //check if screen was active last frame, if not then fire the trigger once
	void Update () {
        if(UIManager.Singleton.playerHistory.enabled && !wasOnLastFrame)
        {
            triggerOn = true;
        }

        // show all the players in a list, built by the game manager player profiles list
        if (triggerOn)
        {
            foreach (PlayerProfile pp in GameManager.Singleton.myPlayerHistory.playerProfiles)
            {
                if (!playerUIDs.Contains(pp.UID))
                {
                    playerUIDs.Add(pp.UID);
                    GameObject temp = GameObject.Instantiate(playerNameBtnPrefab);
                    temp.transform.SetParent(myListGO.transform, false);
                    Text tempText = temp.GetComponentInChildren<Text>();
                    tempText.text = pp.playerName;
                    PlayerBtn tempPB = temp.GetComponent<PlayerBtn>();
                    tempPB.setPlayerProfile(pp);
                }
            }
            myScreenState = MY_SCREEN_STATE.AWAIT_P1;
            myTitle.text = myP1Title;
            triggerOn = false;
        }
        wasOnLastFrame = UIManager.Singleton.playerHistory.enabled;
	}

    /*
    When a profile is clicked, it needs to be loaded into the GameManager, this screen has 3 main states:
    Waiting on P1 selection
    Waiting on P2 selection (if 2 player)
    Waiting for play button

    first and second states are straightforward, however, if the user has selected a player, they may select a different 
    player before moving to play button, so we need to override the currently loaded profile in the GameManger
        */
    public void ClickedProfile(PlayerProfile pp)
    {
        switch(myScreenState)
        {
            case MY_SCREEN_STATE.AWAIT_P1:
                GameManager.Singleton.myPlayer1Profile = pp;
                if (GameManager.Singleton.myNumberPlayers == GameManager.NUMBER_PLAYERS.ONE_PLAYER)
                {
                    myScreenState = MY_SCREEN_STATE.READY_FOR_PLAY;
                }else
                {
                    myTitle.text = myP2Title;
                    myScreenState = MY_SCREEN_STATE.AWAIT_P2;
                }
                break;
            case MY_SCREEN_STATE.AWAIT_P2:
                GameManager.Singleton.myPlayer2Profile = pp;
                myScreenState = MY_SCREEN_STATE.READY_FOR_PLAY;
                break;
            case MY_SCREEN_STATE.READY_FOR_PLAY:
                if(GameManager.Singleton.myNumberPlayers == GameManager.NUMBER_PLAYERS.ONE_PLAYER)
                {
                    GameManager.Singleton.myPlayer1Profile = pp;
                }else
                {
                    GameManager.Singleton.myPlayer2Profile = pp;
                }
                break;
        }
    }

    /*
    PlayBtn - 
    Triggered by the event system
    Will test that a player is selected
    if ONE PLAYER
        Will swap screen to select piece and set player 2 UID to -1 for computer
    if TWO PLAYER
        Will change title to allow player two to select profile
    Request UI Manger to move to next screen
    */
    public void PlayBtn()
    {
        if(myScreenState == MY_SCREEN_STATE.READY_FOR_PLAY)
        {
            if(GameManager.Singleton.myNumberPlayers == GameManager.NUMBER_PLAYERS.ONE_PLAYER)
            {
                GameManager.Singleton.myPlayer2Profile = GameManager.Singleton.myPlayerHistory.computerProfile;
            }
            UIManager.Singleton.playerHistory.enabled = false;
            UIManager.Singleton.selectPiece.enabled = true;
        }
    }
    /*
    BackBtn - 
    Triggered by the event system
    Will take the player back one screen
    if ONE PLAYER
        Will swap screen to Difficulty screen
    if TWO PLAYER
        IF Still selecting player one
            Will swap back to Main Menu
        If selecting Player two
            Will change title to allow player one to select profile
    Request UI Manger to move to next screen
    */
    public void BackBtn()
    {
        if (GameManager.Singleton.myNumberPlayers == GameManager.NUMBER_PLAYERS.ONE_PLAYER)
        {
            UIManager.Singleton.playerHistory.enabled = false;
            UIManager.Singleton.setDifficulty.enabled = true;
        }
        else
        {
            switch(myScreenState)
            {
                case MY_SCREEN_STATE.AWAIT_P1:
                    UIManager.Singleton.playerHistory.enabled = false;
                    UIManager.Singleton.mainMenu.enabled = true;
                    break;
                case MY_SCREEN_STATE.AWAIT_P2:
                    myTitle.text = myP1Title;
                    myScreenState = MY_SCREEN_STATE.AWAIT_P1;
                    break;
                case MY_SCREEN_STATE.READY_FOR_PLAY:
                    myTitle.text = myP1Title;
                    myScreenState = MY_SCREEN_STATE.AWAIT_P1;
                    break;
            }
        }
    }

    /*
    Input field, take the name and create a new player profile from it, then add it to the list
    */
    public void EnterName()
    {

        Debug.Log("Name entered and it was " + myEnteredPlayerName.text);
        PlayerProfile ppTemp = new PlayerProfile();
        ppTemp.playerName = myEnteredPlayerName.text;
        ppTemp.UID = GameManager.Singleton.myPlayerHistory.nextPlayerProfileUID.ToString();
        GameManager.Singleton.myPlayerHistory.nextPlayerProfileUID++;
        GameManager.Singleton.myPlayerHistory.playerProfiles.Add(ppTemp);


        playerUIDs.Add(ppTemp.UID);
        GameObject temp = GameObject.Instantiate(playerNameBtnPrefab);
        temp.transform.SetParent(myListGO.transform, false);
        Text tempText = temp.GetComponentInChildren<Text>();
        tempText.text = ppTemp.playerName;
        PlayerBtn tempPB = temp.GetComponent<PlayerBtn>();
        tempPB.setPlayerProfile(ppTemp);
    }
    
}
