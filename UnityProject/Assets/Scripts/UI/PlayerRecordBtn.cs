using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/*
player record button objects need to track its own elements, this script creates those connections

    */
public class PlayerRecordBtn : MonoBehaviour {

    private PlayerProfile myPP;
    private Text playerName;
    private Text winHolder;
    private Text lossHolder;
    private Text tieHolder;

    void Awake()
    {
        this.GetComponent<Button>().onClick.AddListener(delegate { this.Trigger(); });
    }

    public void BuildConnections()
    {
        Text[] childrenTxt = this.gameObject.GetComponentsInChildren<Text>();
        foreach (Text txt in childrenTxt)
        {
            if (txt.gameObject.name == "PlayerName")
                playerName = txt;
            if (txt.gameObject.name == "WinHolder")
                winHolder = txt;
            if (txt.gameObject.name == "LossHolder")
                lossHolder = txt;
            if (txt.gameObject.name == "TieHolder")
                tieHolder = txt;
        }
    }
    public void Trigger()
    {

        MatchHistory mh = UIManager.Singleton.matchHistory.GetComponentInChildren<MatchHistory>();
        mh.ClickedProfile(myPP);
        
    }
    public void setPlayerProfile(PlayerProfile thePP)
    {
        myPP = thePP;

    }
    //set the player button up (this is usually called by match history screen)
    public void updateFields(string name, string wins, string losses, string ties)
    {
        playerName.text = name;
        winHolder.text = wins;
        lossHolder.text = losses;
        tieHolder.text = ties;
    }
}
