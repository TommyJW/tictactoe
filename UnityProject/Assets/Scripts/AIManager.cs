using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
    This is the main interface for AI turns
    It is a Singleton, because there should never be more than one instance of this object
    Difficulty is set by outside classes for configuration - primarily by the Select Difficulty screen
    */
public class AIManager : MonoBehaviour {
    public static AIManager Singleton;
    public enum AI_DIFFICULTY { EASY, MEDIUM, HARD}
    public AI_DIFFICULTY myDifficulty;

    //assigned in the editor for effeciency
    public GameObject boardPoolGO;
    public GameObject piecePoolGO;

    private GamePiece.PIECE_TYPES myPieceType;

    private ArrayList boardPool;
    private int currentBoardIndex;
    private ArrayList piecePool;
    private int currentPieceIndex;
    private GamePiece.PIECE_TYPES enemyPieceType;
    private GamePiece.PIECE_TYPES flippingPiece;
    private int depthGoal = 0;

    public int countCreatedPieces = 0;
    public int highestPoolValue = 0;

    private Cell thePickedCellReady = null;

    //this is called on load - configure necessary lists and initilize 
    void Awake()
    {
        Singleton = this;
        boardPool = new ArrayList(boardPoolGO.GetComponentsInChildren<Board>());

        currentBoardIndex = 0;
        piecePool = new ArrayList(piecePoolGO.GetComponentsInChildren<GamePiece>());
        currentPieceIndex = 0;
    }


    /*
    Client objects (Game Manager) call this function to trigger an AI turn
    It uses the established difficulty mode to trigger an OnClick Event for a cell on the main board
    The process involves a recursive procedure that is bootstraped from 'placePieceStart'
        */
    public void placePiece(GamePiece.PIECE_TYPES AIPieceType)
    {
        myPieceType = AIPieceType;
        if (myPieceType == GamePiece.PIECE_TYPES.O){
            enemyPieceType = GamePiece.PIECE_TYPES.X;
        }
        else{
            enemyPieceType = GamePiece.PIECE_TYPES.O;
        }
        switch (myDifficulty)
        {
            case AI_DIFFICULTY.EASY:
                setupEasyDepth();
                break;
            case AI_DIFFICULTY.MEDIUM:
                setupMediumDepth();
                break;
            case AI_DIFFICULTY.HARD:
                setupHardDepth();
                break;
        }
        placePieceStart();
    }

    /*
    Copy the current board for the number of empty spaces left
    call recTestPiece for each board with a different piece in each board pass the next player's piece, 
    and multiply the returning value by the heuristics
    determine which of the returned boards has the highest value, and place a piece there
    */
    private void placePieceStart()
    {
        currentBoardIndex = -1;
        currentPieceIndex = 0;
        flippingPiece = myPieceType;
        
        //current game state will be in index 0
        duplicateBoard(GameManager.Singleton.mainBoard);

        //duplicate the starting board in states 2-25/26
        int startingBoardGroup = currentBoardIndex+1;
        Board startBoard = ((Board)boardPool[currentBoardIndex]);
        duplicateSubPool(startBoard);


        //put a piece into each board where possible
        int subPoolSize = placeTestPiece(startBoard, startingBoardGroup, myPieceType);

        //find the scores of all children boards
        List<int> scores = performSubCallsGetScores(startingBoardGroup, subPoolSize, 25, flipPiece(myPieceType));

        //convert to Cell array and find the heuristic values for the scores
        List<Cell> cellList = new List<Cell>();
        List<int> heuristicsList = new List<int>();

        string deb = "";
        float highestHeuristics = -999;
        for (int i = 0; i< scores.Count; i++)
        {
            Cell tempCell = null;
   //         if (scores[i] != 0)
   //         {
                tempCell = findCell(i);
                deb += tempCell.myUID.ToString() + " ";
                // heuristicsList.Add(tempCell.myHeuristicValue * scores[i]);
                float tempHeuristicValue = tempCell.myHeuristicValue * scores[i];
             //  tempHeuristicValue = 1 * scores[i];
            if (tempHeuristicValue > highestHeuristics)
                {
                    cellList.Clear();
                    
                    cellList.Add(tempCell);
                    highestHeuristics = tempHeuristicValue;
                }
                else if (tempHeuristicValue == highestHeuristics)
                {
                    
                    cellList.Add(tempCell);
                }
                else
                { }

        }
 
        Cell thePickedCell = null;
      //  Debug.Log("Top Hieruistics length " + topHeuristicIndexs.Count + " and values " + convertArrToString(topHeuristicIndexs));
        if (cellList.Count == 0)
        {
            //pick random cell
            Debug.Log("picked random");
            int availableCellsCount = GameManager.Singleton.mainBoard.cellsWithoutPieces.Count;
            int cellToPick = Random.Range(0, availableCellsCount);
            thePickedCell = GameManager.Singleton.mainBoard.cellsWithoutPieces[cellToPick];
        }
        else if (cellList.Count == 1)
        {
           Debug.Log("only one choice");
            thePickedCell = cellList[0];
        }else
        {
            //pick random cell from the list
            Debug.Log("many in list, picked random");
            int cellToPick = Random.Range(0, cellList.Count);
            thePickedCell = cellList[cellToPick];
        }
        thePickedCell.OnClick();
        
    }

    /*
    locate the cell 'numNull' cells left to right, top to bottom that are empty or null in the Main board
    this is used to determine where the final pieces can be placed
    */
    private Cell findCell(int numNull)
    {
        Cell returnable = null;
        for(int i = 0; i<5; i++)
        {
            for(int ii = 0; ii< 5; ii++)
            {
                if (GameManager.Singleton.mainBoard.allCells[i, ii].myPiece == null ||
                    GameManager.Singleton.mainBoard.allCells[i, ii].myPiece.myPieceType == GamePiece.PIECE_TYPES.NONE)
                {
                    if (numNull == 0)
                    {
                        returnable = GameManager.Singleton.mainBoard.allCells[i,ii];
                        return returnable;
                    }
                    numNull--;
                }
            }
        }
        return returnable;
    }


    /*
        take an incomming board, get a board from the pool and duplicate it from the source
        return the index of the copied board.
        ensure the currentBoardIndex isn't greater than the size of the pool
    */
    private int duplicateBoard(Board source)
    {
      //  Profiler.BeginSample("DuplicateBoard");
        currentBoardIndex++;
     
        Board tempBoard = ((Board)boardPool[currentBoardIndex]);
        tempBoard.AIResetCells();
        for (int i = 0; i < 5; i++)
        {
   
            for (int ii = 0; ii < 5; ii++)
            {
   
                if (source.allCells[i,ii].myPiece != null)
                {
      
                    if (tempBoard.allCells[i,ii].myPiece == null)
                    {
      

                        GamePiece goTempPiece = null;
                        if(currentPieceIndex >= piecePool.Count-20)
                        {
                            GameObject goTemp = new GameObject();
                            goTempPiece = goTemp.AddComponent<GamePiece>();
                            countCreatedPieces++;
                        }
                        else
                        {
                          //  Profiler.BeginSample("InnerCreateBoard");
                            bool foundAvailablePiece = false;
                            while (!foundAvailablePiece)
                            {
                                if (((GamePiece)piecePool[currentPieceIndex]).myCell == null)
                                {
                                    foundAvailablePiece = true;
                                    goTempPiece = (GamePiece)piecePool[currentPieceIndex];
                                }
                                currentPieceIndex++;
                            }
                       //     Profiler.EndSample();
                        }

                        goTempPiece.myPieceType = source.allCells[i, ii].myPiece.myPieceType;
                        tempBoard.AIPlacePieceTest(tempBoard.allCells[i, ii], goTempPiece);

                     
                    }
                    else
                    {
                        GamePiece goTempPiece = tempBoard.allCells[i, ii].myPiece;
                        goTempPiece.myPieceType = source.allCells[i, ii].myPiece.myPieceType;
                        tempBoard.AIPlacePieceTest(tempBoard.allCells[i, ii], goTempPiece);
                    }
                }
            }
        }
   //     Profiler.EndSample();
        return currentBoardIndex;
    }

    /*
        duplicate sub-pool
        create a subpool of duplicated boards based on the source, for the number of empty cells
    */
    private void duplicateSubPool(Board source)
    {

        int numEmptyCells = source.AINumCellsWithoutPieces;
        for (int i = 0; i < numEmptyCells; i++)
        {
            duplicateBoard(source);
        }


       
    }

    /*
        place test pieces
        from the starting index, place myPiece on one empty spot on each board
        return the length of the modified boards so the caller knows the board pool size
    */
    private int placeTestPiece(Board source, int startIndex, GamePiece.PIECE_TYPES pieceType)
    {
        int placingPieceIndex = startIndex;
        int startPoint = 0;
        int endPoint = 5;
   /*     Optimization code - testing only*/
   if (pieceType == enemyPieceType && source.cellsWithPieces.Count < 6 )
        {
            startPoint = 1;
            endPoint = 4;
        }
        for (int i = startPoint; i < endPoint; i++)
        {
            for (int ii = startPoint; ii < endPoint; ii++)
            {
                if (source.allCells[i,ii].myPiece == null)
                {
                    

                    GamePiece goTempPiece = null;
                    if (currentPieceIndex >= piecePool.Count - 20)
                    {
                        GameObject goTemp = new GameObject();
                        goTempPiece = goTemp.AddComponent<GamePiece>();
                        countCreatedPieces++;
                    }
                    else
                    {
                        bool foundAvailablePiece = false;
                        while(!foundAvailablePiece)
                        {
                            if(((GamePiece)piecePool[currentPieceIndex]).myCell == null)
                            {
                                foundAvailablePiece = true;
                                goTempPiece = (GamePiece)piecePool[currentPieceIndex];
                            }
                            currentPieceIndex++;
                        }
                        
                        if(currentPieceIndex> highestPoolValue)
                        {
                            highestPoolValue = currentPieceIndex;
                        }
                    }
                    goTempPiece.myPieceType = pieceType;
                    ((Board)boardPool[placingPieceIndex]).AIPlacePieceTest(((Board)boardPool[placingPieceIndex]).allCells[i, ii], goTempPiece);
                    placingPieceIndex++;
                }
            }
        }

        //Optimization code - test only
        //now from the source board find all enemy pieces that we need to add to
        // any vert/horz cell owned by a enemy piece that is on the edge, only if we are placing enemy pieces
        if (pieceType == enemyPieceType && source.cellsWithPieces.Count < 6)
        {
            List<Cell> edges = new List<Cell>();
            for (int index = 0; index < source.cellsWithPieces.Count; index++)
            {
                List<Cell> tempEdge = new List<Cell>();
                if (source.cellsWithPieces[index].myPiece.myPieceType == enemyPieceType) { 
                    tempEdge = findEdges(source.cellsWithPieces[index]);
                    foreach (Cell ct in tempEdge)
                    {
                        bool found = false;
                        foreach (Cell ce in edges)
                        {
                            if (ct.myUID == ce.myUID)
                            {
                                found = true;
                            }
                        }
                        if (!found)
                        {
                            edges.Add(ct);
                        }
                    }
                }
            }

            foreach(Cell c in edges)
            {
                if(c.myPiece == null)
                {
                    int i = c.myUID / 10 - 1;
                    int ii = c.myUID % 10 -1;
                    GamePiece goTempPiece = null;
                    if (currentPieceIndex >= piecePool.Count - 20)
                    {
                        GameObject goTemp = new GameObject();
                        goTempPiece = goTemp.AddComponent<GamePiece>();
                        countCreatedPieces++;
                    }
                    else
                    {
                        bool foundAvailablePiece = false;
                        while (!foundAvailablePiece)
                        {
                            if (((GamePiece)piecePool[currentPieceIndex]).myCell == null)
                            {
                                foundAvailablePiece = true;
                                goTempPiece = (GamePiece)piecePool[currentPieceIndex];
                            }
                            currentPieceIndex++;
                        }

                        if (currentPieceIndex > highestPoolValue)
                        {
                            highestPoolValue = currentPieceIndex;
                        }
                    }
                    //              verifySource((Board)boardPool[placingPieceIndex], 2);
                    goTempPiece.myPieceType = pieceType;
                    ((Board)boardPool[placingPieceIndex]).AIPlacePieceTest(((Board)boardPool[placingPieceIndex]).allCells[i, ii], goTempPiece);

                    //dummy test
                    //                 verifyBoards(2);


                    placingPieceIndex++;
                }
            }
        }

        return placingPieceIndex-startIndex;
    }

    /*
    Starting with the start index in the board pool, and going length deep into the pool
    create a recursive call to recTestPiece with the upcomming piece (the alternate)
    Depth is used to determine the search depth in the recursive call
        */
    private List<int> performSubCallsGetScores(int start, int length, int depth, GamePiece.PIECE_TYPES nextPiece)
    {
        
        //for each new board, call the recursive board function, those values will go into the scores array
        List<int> scores = new List<int>();
        for (int i = start; i < start+ length; i++)
        {
            int score = 0;
            Board tempBoard = ((Board)boardPool[i]);
            score = recTestPiece(tempBoard, nextPiece, depth);
            scores.Add(score);


                // the piece was already flipped, so a winning result would be opposite of the next piece (ie. if X just won, nextpiece would be O)
            if (nextPiece != enemyPieceType && depth <25)
            {
                
               if(score < -90)
                {
                    return scores;
                }

            }else if(nextPiece == enemyPieceType && depth < 25)
            {
                if(score > 90)
                {
                    return scores;
                }
            }else if(score == 100)
            {
                return scores;
            }
        }
        return scores;
    }


    /*
    take the incoming board, and see if the board is a solved board
        if yes, determine the victor -  'AI won' return 10 points
            if  'Human won' return 0 points.
        if no
            for each remainig empty space, duplicate the incomming board, and place a nextPiece
            on an empty space
            call recTestPiece for that new board if the returned value is 0, pass back 0

        consider resetting the current board index as the tree unwinds
    */
    private int recTestPiece(Board incommingBoard, GamePiece.PIECE_TYPES nextPiece, int depth)
    {
        
        GamePiece.PIECE_TYPES winnerType = GameManager.Singleton.WhoSolvedBoard(incommingBoard, false);
        switch (winnerType)
        {
            case GamePiece.PIECE_TYPES.O:
                // O is mine, and I won
                if (myPieceType == GamePiece.PIECE_TYPES.O)
                {
                    return 100 - (25 - depth) ;
                }
                // O is mine, and I lost
                return (25 - depth) - 100 ;
                
            case GamePiece.PIECE_TYPES.X:
                if (myPieceType == GamePiece.PIECE_TYPES.X)
                {
                    return 100 - (25 - depth);
                }
                return (25- depth) - 100;
            // neither player won or lost this board (base case) search deeper if needed
            case GamePiece.PIECE_TYPES.NONE:
                // found the maximum depth allowed, return without further processing
                if (depth <= depthGoal)
                {
                    return 1; 
                }

                //Moving along the pool, save our next starting point
                int startingBoardGroup = currentBoardIndex + 1;

                //queue up the piece pool so we can recycle them
                int startingPieceGroup = currentPieceIndex;

                duplicateSubPool(incommingBoard);

                //put a piece into each board where possible
                int subPoolSize = placeTestPiece(incommingBoard, startingBoardGroup, nextPiece);

                //if the board is now full 0 would have been returned as no pieces were placed
                if (subPoolSize == 0)
                {
                    // roll back the currentBoard index to allow for reuse by next pool call.
                    currentBoardIndex = startingBoardGroup;
                    currentPieceIndex = startingPieceGroup;
                    return 1;
                }

                List<int> scores = performSubCallsGetScores(startingBoardGroup, subPoolSize, depth-1, flipPiece(nextPiece));

                // roll back the currentBoard index to allow for reuse by next pool call.
                currentBoardIndex = startingBoardGroup;

                currentPieceIndex = startingPieceGroup;


                //find out if the enemy won durring any of the sub-calls
                // and calculate the highest value and lowest value

                int highestValueOfSubscores = -999;
                int lowestValueOfSubscores = 999; 
                for (int i = 0; i < scores.Count; i++)
                {
                    if(highestValueOfSubscores < scores[i])
                    {
                        highestValueOfSubscores = scores[i];
                    }
                    if(lowestValueOfSubscores > scores[i])
                    {
                        lowestValueOfSubscores = scores[i];
                    }
                }

                // determine if we should return the best case, or the worst case scenario based on whos turn played in the child recusion
                if (nextPiece == myPieceType)
                {
                    return highestValueOfSubscores;
                }else
                {
                    return lowestValueOfSubscores;
                }
        }

        return 0;
    }

    //simply returns the opposite piece type as the incomming piece type
    private GamePiece.PIECE_TYPES flipPiece(GamePiece.PIECE_TYPES swap)
    {
        if (swap == GamePiece.PIECE_TYPES.O)
        {
            return GamePiece.PIECE_TYPES.X;
        }
        else
        {
            return GamePiece.PIECE_TYPES.O;
        }
    }

    // test code for debuging purposes
    private string convertArrToString(List<int> convert)
    {

        string returnable = "";
        foreach(int x in convert)
        {
            returnable += x.ToString() + " ";
        }
        return returnable;
    }

    /// <summary>
    /// This determines how deep the search should go
    /// it uses 25-depthgoal = depth searched;
    /// so a depthgoal of 20 will search 5 moves ahead.
    /// </summary>
    private void setupEasyDepth()
    {
        depthGoal = 24;
    }
    private void setupMediumDepth()
    {
        depthGoal = 21;
    }
    private void setupHardDepth()
    {
        int boardFilled = GameManager.Singleton.mainBoard.cellsWithoutPieces.Count;
        if(boardFilled >= 24)
        {
            depthGoal = 24;
        }
        else if (boardFilled >= 21)
        {
            // first two turns
            //two move ahead
            depthGoal = 21;
        } else if (boardFilled >= 17)
        {
            // next two turns
            //two moves ahead
            depthGoal = 21;
        } else if (boardFilled >= 13)
        {
            //two moves ahead
            depthGoal = 21;

        } else if (boardFilled >= 9)
        {
            depthGoal = 21; 
        } else if (boardFilled > 5)
        {
            depthGoal = 0;
        } else
        {
            depthGoal = 0;
        }
    }

    
    // given a cell find the cells along the walls of the board from it vert and horz only
    private List<Cell> findEdges(Cell sourceCell)
    {
        List<Cell> returnable = new List<Cell>();
        bool foundAll = false;
        bool foundOne = false;
        Cell searchCell = sourceCell;
        int indexToSearch = 2;
        while (!foundAll)
        {
            if (searchCell.myPosArr[indexToSearch] != null)
            {
                searchCell = searchCell.myPosArr[indexToSearch];
                if(searchCell.myPosArr[2] == null|| searchCell.myPosArr[4] == null ||
                    searchCell.myPosArr[6] == null || searchCell.myPosArr[8] == null)
                {
                    if (searchCell.myUID != sourceCell.myUID)
                    {
                        returnable.Add(searchCell);
                    }
                }
            }
            else
            {
                if (searchCell.myUID != sourceCell.myUID)
                {
                    returnable.Add(searchCell);
                }
                foundOne = true;
            }
            if(foundOne)
            {
                indexToSearch += 2;
                searchCell = sourceCell;
                if (indexToSearch > 8)
                    foundAll = true;
                foundOne = false;
            }
        }
        return returnable;
    }
}
