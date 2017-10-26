using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

/*
    GameManager Object - Main control class for turns and win/loss states. 
    Performs calls to other classes to load critical data

    Is a Singleton - no other GameManager Object should exist in the scene, this allows other objects to
    reference GameManager without usint the lookup table

    Public variables
    MainBoard - the main virtual board in the scene its state should be known by several classes at once

    Round configuration settings:
    myPlayerSettings, myPieceSettings, myNumberPlayers, myPlayer1Profile, myPlayer2Profile
    These are setup by the UI screens, and referenced to determine

    UI sets and checks the game round to reflex the UI

    Pools are set by the editor

    */
public class GameManager : MonoBehaviour
{

    public static GameManager Singleton;
    public Board mainBoard;

    public enum START_ROUND_SETTINGS_PLAYER { PLAYER_ONE, OTHER}
    public enum START_ROUND_SETTINGS_PIECE { X, O}
    public enum NUMBER_PLAYERS { ONE_PLAYER, TWO_PLAYER}

    public START_ROUND_SETTINGS_PLAYER myPlayerSettings;
    public START_ROUND_SETTINGS_PIECE myPieceSettings;
    public NUMBER_PLAYERS myNumberPlayers;

    public PlayerProfile myPlayer1Profile;
    public PlayerProfile myPlayer2Profile;

    public bool P1_turnFlag;

    public enum GAME_STATES { NONE, RUNROUND, VICTORY, DRAW }   
    public GAME_STATES currentGameState;

    public PlayerHistory myPlayerHistory;

    //editor
    public GameObject[] poolXGamePiece;
    public GameObject[] poolOGamePiece;

    private string path;

    private bool changeTurnsFlag;
    private GamePiece.PIECE_TYPES nextPiece;
    private GameRecord myCurrentGameRecord;
    private int poolXIndex = 0;
    private int poolOIndex = 0;

    private int gamePieceUIDCounter = 0;

    private bool newRoundFlag;
    private bool turnedOnAI;
    private List<Cell> winnerCells;
    void OnAwake()
    {
        Singleton = this;
    }

    /*
		Start- 	Initialize game state, 
				load the player profiles
				Link up the mainBoard
				Create an instance of the AI
                create a computer profile if none exists
	*/
    void Start()
    {
        Singleton = this;
        resetGameStates();

        path = Application.persistentDataPath + Path.DirectorySeparatorChar + "PlayerHistory.xml";
        myPlayerHistory = PlayerHistory.LoadSerXML(path);
        if(myPlayerHistory.nextPlayerProfileUID == 0)
        {
            myPlayerHistory.nextPlayerProfileUID = 1;
        }
        if (myPlayerHistory.nextGameRecordUID == 0)
        {
            myPlayerHistory.nextGameRecordUID = 1;
        }
        if(myPlayerHistory.computerProfile == null)
        {
            myPlayerHistory.computerProfile = new PlayerProfile();
            myPlayerHistory.computerProfile.playerName = "Computer";
            myPlayerHistory.computerProfile.UID = "-1";
        }

    }

    void Update()
    {
        GameState();
    }
    public void configureGamePieces()
    {
        if (myPieceSettings == START_ROUND_SETTINGS_PIECE.X)
            nextPiece = GamePiece.PIECE_TYPES.X;
        else
            nextPiece = GamePiece.PIECE_TYPES.O;
    }
    /*
    Determine if the board has a victor, if it does, pass back the value of the piece that won.
    This function is called by AIManager too.
    It does not need to check the entire board, only top two rows, and left two rows, then uses a downward right algorithm
    to determine if any other cells are solved
    */
    public GamePiece.PIECE_TYPES WhoSolvedBoard(Board theBoard, bool mainBoard)
    {
        for (int row = 0; row < 5; row++)
        {
            for (int col = 0; col < 5; col++)
            {

                if (row < 2 || col < 2)
                {
                    Cell testCell = theBoard.allCells[row, col];
                    GamePiece testGamePiece = testCell.myPiece;
                    if (testGamePiece != null)
                    {
                        for (int k = 6; k < 10; k++)
                        {
                            if (testCell.myPosArr[k] != null && testCell.myPosArr[k].myPiece != null && testCell.myPosArr[k].myPiece.myPieceType == testGamePiece.myPieceType)
                            {
                                if (testCell.myPosArr[k].myPosArr[k] != null && testCell.myPosArr[k].myPosArr[k].myPiece != null && testCell.myPosArr[k].myPosArr[k].myPiece.myPieceType == testGamePiece.myPieceType)
                                {
                                    if (testCell.myPosArr[k].myPosArr[k].myPosArr[k] != null && testCell.myPosArr[k].myPosArr[k].myPosArr[k].myPiece != null && testCell.myPosArr[k].myPosArr[k].myPosArr[k].myPiece.myPieceType == testGamePiece.myPieceType)
                                    {
                                        //if we are testing the main board, save the winning cells so a special effect can be run
                                        if(mainBoard)
                                        {
                                            winnerCells = new List<Cell>();
                                            winnerCells.Add(testCell);
                                            winnerCells.Add(testCell.myPosArr[k]);
                                            winnerCells.Add(testCell.myPosArr[k].myPosArr[k]);
                                            winnerCells.Add(testCell.myPosArr[k].myPosArr[k].myPosArr[k]);
                                        }
                                        return testGamePiece.myPieceType;
                                    }
                                }
                            }
                        }
                    }
                    
                }
            }

        }
        return GamePiece.PIECE_TYPES.NONE;
    }

    /*checks the state of game
      if there is not a game
      do nothing
      if there is a game
      initialize a new round if it hasn't already happened
      check if we ned to ask AI to calculate a piece only in one player mode
      swap turns if a piece was played last frame
      if someone has won
    */
    void GameState()
    {
        string winText = "";
        switch (currentGameState)
        {
            case GAME_STATES.NONE:
                break;
            case GAME_STATES.RUNROUND:
                if(newRoundFlag)
                {
                    myCurrentGameRecord = new GameRecord();
                    myCurrentGameRecord.UID = myPlayerHistory.nextGameRecordUID.ToString();
                    myPlayerHistory.nextGameRecordUID++;
                    myCurrentGameRecord.player1UID = myPlayer1Profile.UID;
                    myCurrentGameRecord.player2UID = myPlayer2Profile.UID;
                    newRoundFlag = false;
                    UIManager.Singleton.SetupGamePlayScreen();
                    turnedOnAI = false;
                }
                if(myNumberPlayers == NUMBER_PLAYERS.ONE_PLAYER)
                {
                    //determine if we need to ask for AI piece
                    if(
                        (myPlayerSettings == START_ROUND_SETTINGS_PLAYER.PLAYER_ONE && P1_turnFlag == false) ||
                        (myPlayerSettings == START_ROUND_SETTINGS_PLAYER.OTHER && P1_turnFlag == true))
                    {
   //                      Debug.Log("AI TURN");
                         AIManager.Singleton.placePiece(nextPiece);
                    }
                }
                //change turns happens when a piece is played (last frame)
                //swap the turns, and check for a vicotry state
                if (changeTurnsFlag)
                {
                    changeTurnsFlag = false;
                    P1_turnFlag = !P1_turnFlag;
                    GamePiece.PIECE_TYPES testSolved = WhoSolvedBoard(mainBoard, true);
                    if (testSolved != GamePiece.PIECE_TYPES.NONE)
                    {
                        currentGameState = GAME_STATES.VICTORY;
                    }
                    else if (testSolved == GamePiece.PIECE_TYPES.NONE && mainBoard.cellsWithoutPieces.Count == 0)
                    {
                        currentGameState = GAME_STATES.DRAW;
                    }
                    else
                    {
                        if (nextPiece == GamePiece.PIECE_TYPES.X)
                        {
                            nextPiece = GamePiece.PIECE_TYPES.O;
                        }
                        else
                        {
                            nextPiece = GamePiece.PIECE_TYPES.X;
                        }
                    }
                }
                break;
            case GAME_STATES.VICTORY:
                // somone won the game, determine who and show the victory bubble note the flag will be reversed 
                // because the turns change after a piece drops
                newRoundFlag = true;
                
                if (P1_turnFlag == false && myNumberPlayers == NUMBER_PLAYERS.TWO_PLAYER)
                {
                    winText = myPlayer1Profile.playerName;
                    myCurrentGameRecord.playerWinner = GameRecord.WINNER_SELECT.PLAYER1;
                }

                else if (P1_turnFlag == true && myNumberPlayers == NUMBER_PLAYERS.TWO_PLAYER)
                {
                    winText = myPlayer2Profile.playerName;
                    myCurrentGameRecord.playerWinner = GameRecord.WINNER_SELECT.PLAYER2;
                }
                else if (P1_turnFlag == false && myNumberPlayers == NUMBER_PLAYERS.ONE_PLAYER)
                {
                    winText = myPlayer1Profile.playerName;
                    myCurrentGameRecord.playerWinner = GameRecord.WINNER_SELECT.PLAYER1;
                }
                else
                {
                    winText = "Computer Wins!";
                    myCurrentGameRecord.playerWinner = GameRecord.WINNER_SELECT.PLAYER2;
                }
                //show the winning effects
                UIManager.Singleton.GameScreenShowWinner(winText);
                if(winnerCells!= null)
                {
                    foreach(Cell c in winnerCells)
                    {
                        c.myCellTrigger.winFire();
                    }
                }
                //save the game and exit runround
                if(!turnedOnAI)
                    myPlayerHistory.gameRecords.Add(myCurrentGameRecord);
                myPlayerHistory.SaveSERProfiles(path);
                myPlayerHistory = PlayerHistory.LoadSerXML(path);
                currentGameState = GAME_STATES.NONE;
                  break;
                  
                //draw game, show draw, and save the game
              case GAME_STATES.DRAW:
                winText = "Draw";
                UIManager.Singleton.GameScreenShowWinner(winText);
                myCurrentGameRecord.playerWinner = GameRecord.WINNER_SELECT.TIE;
                myPlayerHistory.gameRecords.Add(myCurrentGameRecord);
                myPlayerHistory.SaveSERProfiles(path);
                myPlayerHistory = PlayerHistory.LoadSerXML(path);
                currentGameState = GAME_STATES.NONE;
                break;
}
}
/*
* Returns the Gamepiece that is placed on the Cell
*/
    public GameObject PlacePiece(Cell theCell)
    {
        GamePiece.PIECE_TYPES dropPiece = nextPiece;
        //pull up a piece and pass it back
        GameObject returnable;
        if (dropPiece == GamePiece.PIECE_TYPES.X)
        {
            returnable = poolXGamePiece[poolXIndex];
            poolXIndex++;
        }
        else
        {
            returnable = poolOGamePiece[poolOIndex];
            poolOIndex++;
        }
        changeTurnsFlag = true;
        return returnable;
    }

    //establish UIDs for game pieces
    public int getGamePieceNextUID()
    {
        gamePieceUIDCounter++;
        return gamePieceUIDCounter;
    }

    // reinitialize a game round, occurs after the player returns to the main menu from a game
   public void resetGameStates()
    {
        P1_turnFlag = true;
        changeTurnsFlag = false;
        newRoundFlag = true;
        poolXIndex = 0;
        poolOIndex = 0;
        gamePieceUIDCounter = 0;
        winnerCells = null;
        mainBoard.resetBoard();
    }

    /*
        Quit button was pressed in UI, end the round and set the victor
    */
    public void QuitMatch()
    {
        newRoundFlag = true;
        string debugText = "";
        newRoundFlag = true;

        if (P1_turnFlag == false && myNumberPlayers == NUMBER_PLAYERS.TWO_PLAYER)
        {
            debugText = myPlayer1Profile.playerName;
            myCurrentGameRecord.playerWinner = GameRecord.WINNER_SELECT.PLAYER1;
        }

        else if (P1_turnFlag == true && myNumberPlayers == NUMBER_PLAYERS.TWO_PLAYER)
        {
            debugText = myPlayer2Profile.playerName;
            myCurrentGameRecord.playerWinner = GameRecord.WINNER_SELECT.PLAYER2;
        }
        else if (P1_turnFlag == false && myNumberPlayers == NUMBER_PLAYERS.ONE_PLAYER)
        {
            debugText = myPlayer1Profile.playerName;
            myCurrentGameRecord.playerWinner = GameRecord.WINNER_SELECT.PLAYER1;
        }
        else
        {
            debugText = "Computer Wins!";
            myCurrentGameRecord.playerWinner = GameRecord.WINNER_SELECT.PLAYER2;
        }
        myPlayerHistory.gameRecords.Add(myCurrentGameRecord);
        myPlayerHistory.SaveSERProfiles(path);
        myPlayerHistory = PlayerHistory.LoadSerXML(path);
        currentGameState = GAME_STATES.NONE; 
    }

    //UI reports that the user clicked Turn On AI for the P1, establish the changes to the settings for AI to take over
    public void TurnOnP1AI()
    {
        myCurrentGameRecord.playerWinner = GameRecord.WINNER_SELECT.PLAYER2;
        myPlayerHistory.gameRecords.Add(myCurrentGameRecord);
        turnedOnAI = true;
        myNumberPlayers = NUMBER_PLAYERS.ONE_PLAYER;
        myPlayer1Profile = myPlayerHistory.computerProfile;
        myCurrentGameRecord.player1UID = myPlayer1Profile.UID;
        //AI is always considered 'player 2' so flip the turn swapper
        P1_turnFlag = !P1_turnFlag;
        AIManager.Singleton.myDifficulty = AIManager.AI_DIFFICULTY.HARD;
    }

    //UI reports that the user clicked Turn On AI for the P2, establish the changes to the settings for AI to take over
    public void TurnOnP2AI()
    {
        myCurrentGameRecord.playerWinner = GameRecord.WINNER_SELECT.PLAYER1;
        myPlayerHistory.gameRecords.Add(myCurrentGameRecord);
        turnedOnAI = true;
        myNumberPlayers = NUMBER_PLAYERS.ONE_PLAYER;
        myPlayer2Profile = myPlayerHistory.computerProfile;
        myCurrentGameRecord.player2UID = myPlayer2Profile.UID;
        
        AIManager.Singleton.myDifficulty = AIManager.AI_DIFFICULTY.HARD;
    }

}
