using UnityEngine;
using System.Collections;

/*
Entity class for Cell, this contains all the linking, piece ownership, and spawn loacation information for a cell on
either the main board or the AI boards
    */
public class Cell : MonoBehaviour {

    public Cell myPos1;
    public Cell myPos2;
    public Cell myPos3;
    public Cell myPos4;
    public Cell myPos6;
    public Cell myPos7;
    public Cell myPos8;
    public Cell myPos9;

    public Cell[] myPosArr= new Cell[11];

    public CellTrigger myCellTrigger;
	
	public Board myBoard;

    public float myHeuristicValue = 1.5f;
	
	public GamePiece myPiece;

    public int myUID;
	
	private Vector3 mySpawnPoint;

    // Use this for initialization
    void Start () {
        myBoard = this.transform.parent.GetComponent<Board>();
        mySpawnPoint = new Vector3(this.transform.position.x, 
                                            this.transform.position.y + 2.0f, 
                                            this.transform.position.z) ;
        myPosArr[1] = myPos1;
        myPosArr[2] = myPos2;
        myPosArr[3] = myPos3;
        myPosArr[4] = myPos4;
        myPosArr[6] = myPos6;
        myPosArr[7] = myPos7;
        myPosArr[8] = myPos8;
        myPosArr[9] = myPos9;

        myCellTrigger = this.gameObject.GetComponentInChildren<CellTrigger>();
    }

    // Update is called once per frame
    void Update () {
	
	}
	
	/*
		OnClick -
		Triggered when the player clicks the mouse on this cell
		If I do not already contain a piece, request the board to
		add a piece to this point.
		By doing it this way, the cell does not need to know who's turn it is.
		Additionally, AI can fake this OnClick process.
	*/
	public void OnClick(){
        if (GameManager.Singleton.currentGameState == GameManager.GAME_STATES.RUNROUND && 
            (this.myPiece == null || this.myPiece.myPieceType == GamePiece.PIECE_TYPES.NONE))
        {
            GameObject testGO = myBoard.PlacePiece(this);
            if (testGO != null)
            {
                GamePiece theGamePiece = testGO.GetComponent<GamePiece>();
                this.myPiece = theGamePiece;
                testGO.transform.position = mySpawnPoint;
                Rigidbody testGORigidbody = testGO.GetComponent<Rigidbody>();
                testGORigidbody.velocity = Vector3.zero;
                testGORigidbody.velocity = Vector3.zero;
                myCellTrigger.placePieceFire();
            }
        }
	}

}
