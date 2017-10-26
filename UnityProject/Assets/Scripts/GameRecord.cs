using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

/*
 An XML serializable object for a game record, containes the needed info for display and tracking of win/losses
 used by PlayerHistory
*/
public class GameRecord{
    public enum WINNER_SELECT { PLAYER1, PLAYER2, TIE}
    [XmlAttribute("uid")]
    public string UID;
    public string player1UID;
    public string player2UID;
    public WINNER_SELECT playerWinner;
    public float gameTime;
}
