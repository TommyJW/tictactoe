using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/*
Match record needs to track its own elements, this script creates those connections

    */
public class MatchRecordBtn : MonoBehaviour {

    private Text winnerTxt;
    private Text player1Name;
    private Text player2Name;

    void Start()
    {

    }

    public void BuildConnections()
    {
        Text[] childrenTxt = this.gameObject.GetComponentsInChildren<Text>();
        foreach (Text txt in childrenTxt)
        {

            if (txt.gameObject.name == "WinHolder")
                winnerTxt = txt;
            if (txt.gameObject.name == "P1Holder")
                player1Name = txt;
            if (txt.gameObject.name == "P2Holder")
                player2Name = txt;
        }
    }
    // this should only have the match record info
    public void updateFields(string winner, string p1, string p2)
    {
        winnerTxt.text = winner;
        player1Name.text = p1;
        player2Name.text = p2;
    }
}
