using UnityEngine;
using System.Collections;
/*
This controlls camera movement
using several points to look at and move towards, the camera transitions between menu and game modes
    */
public class CameraScript : MonoBehaviour {


    public Transform menuPointToLookAt;
    public Transform menuPointToStart;
    public Transform gamePointToLookAt;
    public Transform gamePointToStart;
    public float Menuspeed = 1.0f;
    public float flyToSpeed = 1.0f;
    public static CameraScript Singleton;

    public float masterVolume;

    public AudioSource cricketsSource;
    public AudioSource pianoSource;

    private bool lookAtMenu = true;
	// Use this for initialization
	void Start () {
        transform.position = menuPointToStart.position;
        transform.LookAt(menuPointToLookAt);
        Singleton = this;
	}
	
	// Update is called once per frame
    //fly to the selected point
	void Update () {
        menuPointToStart.transform.LookAt(menuPointToLookAt);
        menuPointToStart.Translate(Vector3.right * Time.deltaTime * Menuspeed);
        if (lookAtMenu)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, menuPointToStart.rotation, flyToSpeed * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, menuPointToStart.position, flyToSpeed * Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, gamePointToStart.rotation, flyToSpeed * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, gamePointToStart.position, flyToSpeed * Time.deltaTime);
        }
	}

    //change the look/move to point as the menu point
    public void MoveToMenu()
    {
        lookAtMenu = true;
        pianoSource.volume = 1.0f * masterVolume;
        cricketsSource.volume = 0.2f * masterVolume;
    }
    //change the look/move to point as the game point
    public void MoveToGame()
    {
        lookAtMenu = false;
        pianoSource.volume = 0.2f * masterVolume;
        cricketsSource.volume = 1.0f * masterVolume;
    }
}
