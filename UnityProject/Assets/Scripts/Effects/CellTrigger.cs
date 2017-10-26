using UnityEngine;
using System.Collections;

/*
Cell effect engine
Trigger and change the particle effects for the cell that owns this script
*/
public class CellTrigger : MonoBehaviour {

    public ParticleSystem side1;
    public ParticleSystem side2;
    public ParticleSystem side3;
    public ParticleSystem side4;
    public Color winFireColor;

    private Color placePieceFireColor;
    public bool turnON = false;
	// Use this for initialization
	void Start () {
        placePieceFireColor = side1.startColor;
	}
	
	// Update is called once per frame
    // if told to fire an event, play it
	void Update () {
	    if(turnON)
        {
            side1.Play();
            side2.Play();
            side3.Play();
            side4.Play();
            turnON = false;
        }
	}

    //piece dropped, play basic effect
    public void placePieceFire()
    {
        side1.Play();
        side2.Play();
        side3.Play();
        side4.Play();
    }

    //cell is part of winning row, play the winning effect
    public void winFire()
    {
        side1.Stop();
        side2.Stop();
        side3.Stop();
        side4.Stop();
         side1.startColor = winFireColor;
         side2.startColor = winFireColor;
         side3.startColor = winFireColor;
         side4.startColor = winFireColor;
        side1.loop = true;
        side2.loop = true;
        side3.loop = true;
        side4.loop = true;
        side1.Play();
        side2.Play();
        side3.Play(); 
        side4.Play();
    }

    //reset the effects (called on reset board)
    public void resetTrigger()
    {
        side1.Stop();
        side2.Stop();
        side3.Stop();
        side4.Stop();
        side1.startColor = placePieceFireColor;
        side2.startColor = placePieceFireColor;
        side3.startColor = placePieceFireColor;
        side4.startColor = placePieceFireColor;
        side1.loop = false;
        side2.loop = false;
        side3.loop = false;
        side4.loop = false;
    }
}
