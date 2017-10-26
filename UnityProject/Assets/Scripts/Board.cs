using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
    Board - every virtual representation of a 5x5 game board
    Can locate cells with and without pieces, and maintains a list of all cells

    */
public class Board : MonoBehaviour {

    public Cell[,] allCells = new Cell[5, 5];
    public List<Cell> cellsWithPieces;
    public List<Cell> cellsWithoutPieces;
    public int AINumCellsWithoutPieces = 25;


	// Use this for initialization
	void Start () {
        cellsWithPieces = new List<Cell>();
        cellsWithoutPieces = new List<Cell>();
        initializeBoard();
	}
	
	// Update is called once per frame
	void Update () {

	}
    /*
    basic setup of board lists, cells are children of a board object
    every cell has a UID col_row orientation, use that to match them up and populate
    the 2d array
*/
    public void initializeBoard()
    {
        cellsWithPieces.Clear();
        cellsWithoutPieces.Clear();
        Cell[] allCellList = this.GetComponentsInChildren<Cell>();
        for (int i = 0; i < allCellList.Length; i++)
        {
            switch (allCellList[i].myUID)
            {
                case 11:
                    allCells[0, 0] = allCellList[i];
                    break;
                case 12:
                    allCells[0, 1] = allCellList[i];
                    break;
                case 13:
                    allCells[0, 2] = allCellList[i];
                    break;
                case 14:
                    allCells[0, 3] = allCellList[i];
                    break;
                case 15:
                    allCells[0, 4] = allCellList[i];
                    break;

                case 21:
                    allCells[1, 0] = allCellList[i];
                    break;
                case 22:
                    allCells[1, 1] = allCellList[i];
                    break;
                case 23:
                    allCells[1, 2] = allCellList[i];
                    break;
                case 24:
                    allCells[1, 3] = allCellList[i];
                    break;
                case 25:
                    allCells[1, 4] = allCellList[i];
                    break;

                case 31:
                    allCells[2, 0] = allCellList[i];
                    break;
                case 32:
                    allCells[2, 1] = allCellList[i];
                    break;
                case 33:
                    allCells[2, 2] = allCellList[i];
                    break;
                case 34:
                    allCells[2, 3] = allCellList[i];
                    break;
                case 35:
                    allCells[2, 4] = allCellList[i];
                    break;

                case 41:
                    allCells[3, 0] = allCellList[i];
                    break;
                case 42:
                    allCells[3, 1] = allCellList[i];
                    break;
                case 43:
                    allCells[3, 2] = allCellList[i];
                    break;
                case 44:
                    allCells[3, 3] = allCellList[i];
                    break;
                case 45:
                    allCells[3, 4] = allCellList[i];
                    break;

                case 51:
                    allCells[4, 0] = allCellList[i];
                    break;
                case 52:
                    allCells[4, 1] = allCellList[i];
                    break;
                case 53:
                    allCells[4, 2] = allCellList[i];
                    break;
                case 54:
                    allCells[4, 3] = allCellList[i];
                    break;
                case 55:
                    allCells[4, 4] = allCellList[i];
                    break;

            }

        }
        //empty out the my piece value in from previous uses
        for (int i = 0; i < 5; i++)
        {
            for(int ii= 0; ii< 5; ii++)
            {
                allCells[i,ii].myPiece = null;
                cellsWithoutPieces.Add(allCells[i, ii]);
            }
        }
    }

    /*
    Called from Cell to place the piece because this cell was selected
    move the cell from the list of cells without GamePieces to with Gamepieces
        */
    public GameObject PlacePiece(Cell theCell)
    {
        GameObject testGO = GameManager.Singleton.PlacePiece(theCell);
        if (testGO != null)
        {
            for (int i = 0; i < cellsWithoutPieces.Count; i++)
            {
                if (theCell.myUID == cellsWithoutPieces[i].myUID)
                {
                    cellsWithPieces.Add(theCell);
                    cellsWithoutPieces.RemoveAt(i);
                }
            }
        }
        return testGO;
    }

    /*
    Similar to initilize board, except it doesn't need to recalculate the UIDs of cells
    */
    public void resetBoard()
    {
        cellsWithoutPieces.Clear();
        cellsWithPieces.Clear();
        for(int i = 0; i < 5; i++)
        {
            for (int ii = 0; ii< 5; ii++)
            {
                if(allCells[i,ii] != null && allCells[i,ii].myPiece != null)
                {
                    GamePiece tempGP = allCells[i, ii].myPiece;
                    tempGP.myCell = null;
                    tempGP.transform.position = new Vector3(100, 100, 100);
                    allCells[i, ii].myPiece = null;
                    allCells[i, ii].myCellTrigger.resetTrigger();
                    
                }
                cellsWithoutPieces.Add(allCells[i, ii]);
            }
        }

    }

    /*
    a psudo place piece function for AI only, this lets the AI offload some of the board
    management tasks to here and allows AI to reference its board pool similarly to how it references the main board
    */
    public void AIPlacePieceTest(Cell theCell, GamePiece theGamePiece)
    {

        theCell.myPiece = theGamePiece;
        theGamePiece.myCell = theCell;
        AINumCellsWithoutPieces--;
        cellsWithPieces.Add(theCell);
    }

    /*
    same purpose as ResetBoard, but specific to AITestPiece cases
    */
    public void AIResetCells()
    {
        for(int i = 0; i< 5; i++)
        {
            for (int ii=0; ii< 5; ii++)
            {
                if(allCells[i,ii].myPiece!= null)
                {
                    allCells[i, ii].myPiece.myCell = null;
                }
                allCells[i, ii].myPiece = null;
            }
        }

        if (cellsWithPieces.Count > 0)
            cellsWithPieces.Clear();
        AINumCellsWithoutPieces = 25;
    }
}
