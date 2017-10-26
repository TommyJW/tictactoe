using UnityEngine;
using System.Collections;
/*
Entity class containing all of the available screens (canvas) 
    */
public class UIManager : MonoBehaviour {

    public enum SCREENS_LIST { MAIN_MENU, SET_DIFFICULTY, PLAYER_HISTORY, SELECT_PIECE, GAME_PLAY, MATCH_HISTORY}
    public SCREENS_LIST requestedNextScreen;

    public static UIManager Singleton;

    public Canvas mainMenu;
    public Canvas setDifficulty;
    public Canvas playerHistory;
    public Canvas selectPiece;
    public Canvas gameplay;
    public Canvas matchHistory;

    public GameObject GamePlayGO;

    private SCREENS_LIST currentScreen;
    private GamePlay myGamePlayScript;


	// Use this for initialization
	void Start () {
        Singleton = this;
        myGamePlayScript = GamePlayGO.GetComponent<GamePlay>();

	}

    public void SetupGamePlayScreen()
    {
        myGamePlayScript.SetupScreen(GameManager.Singleton.myPlayer1Profile.playerName, GameManager.Singleton.myPlayer2Profile.playerName);
    }
    public void GameScreenShowWinner(string Name)
    {
        myGamePlayScript.ShowWinner(Name);
    }

}
