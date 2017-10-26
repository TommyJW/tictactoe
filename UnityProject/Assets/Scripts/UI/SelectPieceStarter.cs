using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/*
Screen script for the Game play canvas
every public member is located on the UI
    */
public class SelectPieceStarter : MonoBehaviour {

    public Text playerOneTitleNameHolder;
    public Text playerTwoTitleNameHolder;
    public Text playerOneBtnNameHolder;
    public Text playerTwoBtnNameHolder;

    public enum MY_SCREEN_MODES { ONE_PLAYER, TWO_PLAYER }
    public MY_SCREEN_MODES myMode;

    private bool triggerOn = false;
    private bool wasOnLastFrame = false;


    // Update is called once per frame
    /*
    Determine if we are loading the screen and on it's first frame
    If so, then process the holder text fields
        */
    void Update () {
        if (UIManager.Singleton.selectPiece.enabled && !wasOnLastFrame)
        {
            triggerOn = true;
        }
        if (triggerOn)
        {
            if (GameManager.Singleton.myPlayer1Profile != null)
                playerOneTitleNameHolder.text = GameManager.Singleton.myPlayer1Profile.playerName;
            else
                playerOneTitleNameHolder.text = "Player 1";
            if (GameManager.Singleton.myPlayer2Profile != null)
                playerTwoTitleNameHolder.text = GameManager.Singleton.myPlayer2Profile.playerName;
            else
                playerTwoTitleNameHolder.text = "Player 2";
            playerOneBtnNameHolder.text = playerOneTitleNameHolder.text;
            playerTwoBtnNameHolder.text = playerTwoTitleNameHolder.text;
            triggerOn = false;
        }
        wasOnLastFrame = UIManager.Singleton.playerHistory.enabled;
    }

    /*
    PlayerOneStartBtn - 
    Triggered by the event system
    Set the GameManager Start Settings
    */
    public void PlayerOneStartBtn(){
        GameManager.Singleton.P1_turnFlag = true;
    }
    /*
    PlayerTwoStartBtn - 
    Triggered by the event system
    Set the GameManager Start Settings
    */
    public void PlayerTwoStartBtn()
    {
        GameManager.Singleton.P1_turnFlag = false;
    }
    /*
    XStartBtn - 
    Triggered by the event system
    Set the GameManager Start Settings
    */
    public void XStartBtn()
    {
        GameManager.Singleton.myPieceSettings = GameManager.START_ROUND_SETTINGS_PIECE.X;
    }
    /*
    OStartBtn - 
    Triggered by the event system
    Set the GameManager Start Settings
    */
    public void OStartBtn()
    {
        GameManager.Singleton.myPieceSettings = GameManager.START_ROUND_SETTINGS_PIECE.O;
    }
    /*
    PlayBtn - 
    Triggered by the event system
    Request GameManager to start a new round
    Request UI Manger to move to next screen
    */
    public void PlayBtn()
    {
        GameManager.Singleton.configureGamePieces();
        GameManager.Singleton.currentGameState = GameManager.GAME_STATES.RUNROUND;
        UIManager.Singleton.selectPiece.enabled = false;
        UIManager.Singleton.gameplay.enabled = true;
        CameraScript.Singleton.MoveToGame();
    }

    //user pressed quit, go back to player history
    public void QuitBtn()
    {
        UIManager.Singleton.selectPiece.enabled = false;
        UIManager.Singleton.playerHistory.enabled = true;
    }

}
