using UnityEngine;
using System.Collections;
/*
Screen script for the Select difficulty canvas

    */
public class SelectDifficulty : MonoBehaviour {


    /*
    EasyBtn - 
    Triggered by the event system
    Will setup the AI for an Easy Mode match
    Set AIManager to Easy AI
    Request UI Manger to move to next screen
    */
    public void EasyBtn(){
        AIManager.Singleton.myDifficulty = AIManager.AI_DIFFICULTY.EASY;
        UIManager.Singleton.setDifficulty.enabled = false;
        UIManager.Singleton.playerHistory.enabled = true;
    }
    /*
    MediumBtn - 
    Triggered by the event system
    Will setup the AI for an Medium Mode match
    Set AIManager to Medium AI
    Request UI Manger to move to next screen
    */
    public void MediumBtn(){
        AIManager.Singleton.myDifficulty = AIManager.AI_DIFFICULTY.MEDIUM;
        UIManager.Singleton.setDifficulty.enabled = false;
        UIManager.Singleton.playerHistory.enabled = true;
    }
    /*
    HardBtn - 
    Triggered by the event system
    Will setup the AI for an Hard Mode match
    Set AIManager to Hard AI
    Request UI Manger to move to next screen
    */
    public void HardBtn(){
        AIManager.Singleton.myDifficulty = AIManager.AI_DIFFICULTY.HARD;
        UIManager.Singleton.setDifficulty.enabled = false;
        UIManager.Singleton.playerHistory.enabled = true;
    }

    /*
    BackBtn - 
    Triggered by the event system
    Will take the player back one screen - to main menu
    Request UI Manger to move to next screen
    */
    public void BackBtn()
    {
        UIManager.Singleton.setDifficulty.enabled = false;
        UIManager.Singleton.mainMenu.enabled = true;
    }
}
