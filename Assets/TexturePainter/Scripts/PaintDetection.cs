using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintDetection : MonoBehaviour {

    public List<Camera> paintingCam = new List<Camera>();

    public List<GameObject> brushContainer = new List<GameObject>();

    public Camera sceneCamera;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()    {

        CanvasDetection();
        brushContainer[0].SetActive(true);
        brushContainer[1].SetActive(false);
        if(Input.GetKey(KeyCode.A))
        {
            brushContainer[1].SetActive(true);
            brushContainer[0].SetActive(false);
        }
    }

    bool CanvasDetection()
    {
        RaycastHit hit;
        Vector3 mousPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
        Ray cursorRay = sceneCamera.ScreenPointToRay(mousPos);

        if (Physics.Raycast(cursorRay, out hit, 200))
        {
            MeshCollider meshCol = hit.collider as MeshCollider;

            if (meshCol == null || meshCol.sharedMaterial == null)
                return false;
            if(meshCol.tag == "canvas2")
            {
                paintingCam[0].enabled = false;
                paintingCam[1].enabled = true;
                Debug.Log("canvas 2 activate");
            }
            else if(meshCol.tag == "canvas")
            {
                paintingCam[1].enabled = false;
                paintingCam[0].enabled = true;
            }

            return true;
        }
        else
            return false;

    }
}
