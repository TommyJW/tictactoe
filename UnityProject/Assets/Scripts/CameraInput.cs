using UnityEngine;
using System.Collections;
using System.IO;

/*
Inputs in first person mode are often handeled by the camera object
for ease of use, thisCamera is a reference to itself by the editor
    */
public class CameraInput : MonoBehaviour {


    public Camera thisCamera;
    // Use this for initialization

    void Awake()
    {

    }

	void Start () {
        UnityEngine.Profiling.Profiler.maxNumberOfSamplesPerFrame = -1;
	}
	
	// Update is called once per frame
    /*
    check if the user clicked on a cell, and if so trigger a place piece event
    */
	void Update () {
        if (Input.GetButtonDown("Fire1"))
        {
            RaycastHit hit;
            Ray ray = thisCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
                if (objectHit.tag == "Cell")
                {
                    Cell theCellScript = (Cell)objectHit.GetComponent<Cell>();
                    theCellScript.OnClick();
                }
            }
        }
	}
}
