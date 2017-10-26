using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

/*
PlayerProfile XML Serializable Object, contains all the information required of a profile and associates game records to it
    */
public class PlayerProfile {
    [XmlAttribute("uid")]
    public string UID;
    [XmlAttribute("name")]
    public string playerName;

    private int currentWins;
    private int currentLosses;
    private int currentTies;
    private float totalGameTime;
    private List<GameRecord> theGameRecords;

    public void setTheGameRecords(List<GameRecord> theRecords)
    {
        theGameRecords = theRecords;
    }
    public List<GameRecord> getTheGameRecords()
    {
        if(theGameRecords == null)
        {
            theGameRecords = new List<GameRecord>();
        }
        return theGameRecords;
    }

    /*
    Analyze the existing gameRecords array, and update internal win/loss/tie attributes
    */
    public void CalculateTotals()
    {
        currentWins = 0;
        currentLosses = 0;
        currentTies = 0;
        foreach(GameRecord gameRecord in theGameRecords)
        {
            if(gameRecord.player1UID == this.UID)
            {
                if(gameRecord.playerWinner == GameRecord.WINNER_SELECT.PLAYER1)
                {
                    currentWins++;
                }else if(gameRecord.playerWinner == GameRecord.WINNER_SELECT.PLAYER2)
                {
                    currentLosses++;
                }
                else
                {
                    currentTies++;
                }
            }else if(gameRecord.player2UID == this.UID)
            {
                if (gameRecord.playerWinner == GameRecord.WINNER_SELECT.PLAYER2)
                {
                    currentWins++;
                }
                else if (gameRecord.playerWinner == GameRecord.WINNER_SELECT.PLAYER1)
                {
                    currentLosses++;
                }
                else
                {
                    currentTies++;
                }
            }
        }
    }

    //return win/loss/tie attributes as an array
    public int[] getWinLossTie()
    {
        int[] returnable = { 0, 0, 0 };
        returnable[0] = currentWins;
        returnable[1] = currentLosses;
        returnable[2] = currentTies;
        return returnable;
    }
}
