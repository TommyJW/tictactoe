using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/*
player button objects need to track its own elements, this script creates those connections

    */
public class PlayerBtn : MonoBehaviour {


    private PlayerProfile myPP;

    //dynamically assign the trigger event listener for the engine
    void Awake()
    {
        this.GetComponent<Button>().onClick.AddListener(delegate { this.Trigger(); });
    }
    public void Trigger()
    {
            SelectPlayer sp = UIManager.Singleton.playerHistory.GetComponentInChildren<SelectPlayer>();
            sp.ClickedProfile(myPP);
    }
    public void setPlayerProfile(PlayerProfile thePP)
    {
        myPP = thePP;
    }


}
