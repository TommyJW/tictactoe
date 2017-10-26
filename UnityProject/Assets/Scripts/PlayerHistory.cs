using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System;
using System.Xml.Serialization;


/*
Root XML serializable object for a PlayerHistory if a file isn't found, create a new object otherwise load the data
parse the players and their win/loss records
    */
[XmlRoot("PlayerHistory")]
public class PlayerHistory
{


    [XmlArray("PlayerProfiles")]
    [XmlArrayItem("PlayerProfile")]
    public List<PlayerProfile> playerProfiles = new List<PlayerProfile>();

    [XmlArray("GameRecords")]
    [XmlArrayItem("GameRecord")]
    public List<GameRecord> gameRecords = new List<GameRecord>();


    public int nextPlayerProfileUID;
    public int nextGameRecordUID;

    public PlayerProfile computerProfile;
  

    //loading function
   public static PlayerHistory LoadSerXML(string path)
    {
        bool foundGuest = false;
        // will fail if file DNE
        try
        { 
            var serializer = new XmlSerializer(typeof(PlayerHistory));
            using (var stream = new FileStream(path, FileMode.Open))
            {
                PlayerHistory returnable = serializer.Deserialize(stream) as PlayerHistory;
                // parse all the found XML Objects of player profile, and create new PlayerProfiles
                for (int plr = 0; plr < returnable.playerProfiles.Count; plr++)
                {
                    if(returnable.playerProfiles[plr].UID == "-999")
                    {
                        foundGuest = true;
                    }
                    List<GameRecord> tempRecords = new List<GameRecord>();
                    //add any game records featuring this player to itself
                    for (int gamerec = 0; gamerec < returnable.gameRecords.Count; gamerec++)
                    {
                        if (returnable.gameRecords[gamerec].player1UID == returnable.playerProfiles[plr].UID)
                        {
                            tempRecords.Add(returnable.gameRecords[gamerec]);
                        } else if (returnable.gameRecords[gamerec].player2UID == returnable.playerProfiles[plr].UID)
                        {
                            tempRecords.Add(returnable.gameRecords[gamerec]);
                        }
                    }
                    returnable.playerProfiles[plr].setTheGameRecords(tempRecords);
                    returnable.playerProfiles[plr].CalculateTotals();
                }
                if(!foundGuest)
                {
                    PlayerProfile pp = new PlayerProfile();
                    pp.UID = "-999";
                    pp.playerName = "Guest";
                    returnable.playerProfiles.Add(pp);
                }
                return returnable; 
            }
            //File DNE, create a new profile object and a guest
        }catch(Exception e)
        {
            
            PlayerHistory returnable = new PlayerHistory();
            PlayerProfile pp = new PlayerProfile();
            pp.UID = "-999";
            pp.playerName = "Guest";
            returnable.playerProfiles.Add(pp);
            return returnable;
        }
    }
   /*
       SaveProfiles -
       Called from GameManager
       Perform FileIO on Resources/PlayerHistoryFile.txt to output
       a list of PlayerProfiles, and linked GameRecords
   */

    public void SaveSERProfiles(string path)
    {
        var serializer = new XmlSerializer(typeof(PlayerHistory));
        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }

    /*
    Helper function, using a UID, find a player name
    */
    public string retrievePlayerNameByUID(string playerUID)
    {
        foreach (PlayerProfile pp in playerProfiles)
        {
            if(playerUID == pp.UID)
            {
                return pp.playerName;
            }
        }
        return "";
    }

    /*
    Helper test function, build several players with fake game matches to confirm file io works

    */
    public void buildFakeData()
    {
        PlayerProfile testP1 = new PlayerProfile();
        testP1.UID = "0001";
        testP1.playerName = "Jim";
        PlayerProfile testP2 = new PlayerProfile();
        testP2.UID = "0002";
        testP2.playerName = "Bill";

        playerProfiles.Add(testP1);
        playerProfiles.Add(testP2);
        /*
        Game one p1 and p2, p1 won in 10 seconds
    */
        GameRecord testGame1 = new GameRecord();
        testGame1.player1UID = testP1.UID;
        testGame1.player2UID = testP2.UID;
        testGame1.playerWinner = GameRecord.WINNER_SELECT.PLAYER1;
        testGame1.gameTime = 10.0f;


        List<GameRecord> playerHistoryList = testP1.getTheGameRecords();
        if(playerHistoryList == null)
        { playerHistoryList = new List<GameRecord>(); }
        playerHistoryList.Add(testGame1);
        playerHistoryList = testP2.getTheGameRecords();
        if (playerHistoryList == null)
        { playerHistoryList = new List<GameRecord>(); }
        playerHistoryList.Add(testGame1);

        gameRecords.Add(testGame1);

        /*
        Game two p1 and p2, p2 won in 15 seconds
        */
        testGame1 = new GameRecord();
        testGame1.player1UID = testP1.UID;
        testGame1.player2UID = testP2.UID;
        testGame1.playerWinner = GameRecord.WINNER_SELECT.PLAYER2;
        testGame1.gameTime = 15.0f;

        playerHistoryList = testP1.getTheGameRecords();
        if (playerHistoryList == null)
        { playerHistoryList = new List<GameRecord>(); }
        playerHistoryList.Add(testGame1);
        playerHistoryList = testP2.getTheGameRecords();
        if (playerHistoryList == null)
        { playerHistoryList = new List<GameRecord>(); } 
        playerHistoryList.Add(testGame1);

        gameRecords.Add(testGame1);

        /*
        Game three p1 and p2, tie in 20 seconds
        */
        testGame1 = new GameRecord();
        testGame1.player1UID = testP2.UID;
        testGame1.player2UID = testP1.UID;
        testGame1.playerWinner = GameRecord.WINNER_SELECT.TIE;
        testGame1.gameTime = 20.0f;

        playerHistoryList = testP1.getTheGameRecords();
        if (playerHistoryList == null)
        { playerHistoryList = new List<GameRecord>(); }
        playerHistoryList.Add(testGame1);
        playerHistoryList = testP2.getTheGameRecords();
        if (playerHistoryList == null)
        { playerHistoryList = new List<GameRecord>(); }
        playerHistoryList.Add(testGame1);

        gameRecords.Add(testGame1);
    }
}
