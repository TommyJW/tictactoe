using UnityEngine;
using System.Collections;
/*
Screen script for the MainMenu canvas
every member is located on the UI
    */
public class MainMenu : MonoBehaviour {


    /*
        OnePlayerBtn - 
        Triggered by the event system
        Will setup the game for a one player match
        Set Game Manager to One Player Mode
        Request UI Manger to move to next screen
    */
    public void OnePlayerBtn(){
        GameManager.Singleton.myNumberPlayers = GameManager.NUMBER_PLAYERS.ONE_PLAYER;
        UIManager.Singleton.mainMenu.enabled = false;
        UIManager.Singleton.setDifficulty.enabled = true;

    }
    /*
        TwoPlayerBtn - 
        Triggered by the event system
        Will setup the game for a two player match
        Set Game Manager to Two Player Mode
        Request UI Manger to move to next screen
    */
    public void TwoPlayerBtn() {
        GameManager.Singleton.myNumberPlayers = GameManager.NUMBER_PLAYERS.TWO_PLAYER;
        UIManager.Singleton.mainMenu.enabled = false;
        UIManager.Singleton.playerHistory.enabled = true;
    }
    /*
    MatchHistoryBtn - 
    Request UI Manger to move to next screen
    */
    public void MatchHistoryBtn() {
        UIManager.Singleton.mainMenu.enabled = false;
        UIManager.Singleton.matchHistory.enabled = true;
    }
    /*
    SettingsBtn-
    Currently not used
    */
    public void SettingsBtn(){

    }
    /*
    QuitBtn - 
    Perform one last save of match history (this should have already been done)
    then call unity engine to close the app.
    */
    public void QuitBtn(){
        Application.Quit();
    }
}
