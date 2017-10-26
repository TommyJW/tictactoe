using UnityEngine;
using System.Collections;

public class GamePiece : MonoBehaviour
{

    public enum PIECE_TYPES {NONE, X, O }
    public int UID = -1;
    public PIECE_TYPES myPieceType;
    public Cell myCell;

    //force myself to get a UID if I haven't already
    void Update()
    {
        if(UID == -1)
            UID = GameManager.Singleton.getGamePieceNextUID();
    }

}
