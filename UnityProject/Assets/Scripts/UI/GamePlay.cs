using UnityEngine;
using System.Collections;
using UnityEngine.UI;


/*
Screen script for the Game play canvas
every member is located on the UI
    */
public class GamePlay : MonoBehaviour {

    public GameObject winnerBtn;
    public Text winnerTxt;

    public GameObject quitBtn;

    public Button P1AIButton;
    public Button P2AIButton;

    public Text P1Name;
    public Text P2Name;

    /*
        Callec by UI Manager on loading this screen
        sets up the names in the corners, and resets the visible buttons
    */
    public void SetupScreen(string p1Name, string p2Name)
    {
        P1Name.text = p1Name;
        P2Name.text = p2Name;
        winnerBtn.SetActive(false);
        quitBtn.SetActive(true);
        if (GameManager.Singleton.myNumberPlayers == GameManager.NUMBER_PLAYERS.ONE_PLAYER)
        {
            P1AIButton.gameObject.SetActive(false);
            P2AIButton.gameObject.SetActive(false);
        } 
        else
        {
            P1AIButton.gameObject.SetActive(true);
            P2AIButton.gameObject.SetActive(true);
        }
    }

    /*
    turns on the winner button
    */
    public void ShowWinner(string Name)
    {
        winnerTxt.text = Name;
        winnerBtn.SetActive(true);
        quitBtn.SetActive(false);
    }

    //turns off the winner button
    public void HideWinner()
    {
        winnerBtn.SetActive(false);
    }

    //turns on the AI for P1
    public void turnP1AI()
    {
        GameManager.Singleton.TurnOnP1AI();
    }

    //turns on the AI for P2
    public void turnP2AI()
    {
        GameManager.Singleton.TurnOnP2AI();
    }

    //winner button pressed - go back to menu reset the board
    public void winnerButton()
    {
        UIManager.Singleton.gameplay.enabled = false;
        UIManager.Singleton.mainMenu.enabled = true;
        CameraScript.Singleton.MoveToMenu();
        GameManager.Singleton.resetGameStates();
    }

    //quit button pressed - go back to main menu and inform GameManager of the quit
    public void quitButton()
    {
        UIManager.Singleton.gameplay.enabled = false;
        UIManager.Singleton.mainMenu.enabled = true;
        GameManager.Singleton.QuitMatch();
        CameraScript.Singleton.MoveToMenu();
        GameManager.Singleton.resetGameStates();
    }
}
