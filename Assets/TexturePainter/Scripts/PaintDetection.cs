using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintDetection : MonoBehaviour {

    public List<Camera> paintingCam = new List<Camera>();

    public List<GameObject> brushContainer = new List<GameObject>();

    public Camera sceneCamera;

    public List<TexturePainter> texturePainter = new List<TexturePainter>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()    {

        CanvasDetection();
    }

    void CanvasDetection()
    {
        RaycastHit hit;
        Vector3 mousPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
        Ray cursorRay = sceneCamera.ScreenPointToRay(mousPos);

        

        if (Physics.Raycast(cursorRay, out hit, 200))
        {
            MeshCollider meshCol = hit.collider as MeshCollider;

            //if(meshCol == null)
            //{

            //}
            if(meshCol.tag == "canvas2")
            {
                brushContainer[1].SetActive(true);
                brushContainer[0].SetActive(false);
                texturePainter[1].brushCursor.SetActive(false);
                foreach (Transform child in brushContainer[0].transform)
                {//Clear brushes
                    Destroy(child.gameObject);
                }
                Debug.Log("canvas2");
                
            }
            else if(meshCol.tag == "canvas")
            {
                brushContainer[0].SetActive(true);
                brushContainer[1].SetActive(false);
                texturePainter[0].brushCursor.SetActive(false);
                foreach (Transform child in brushContainer[1].transform)
                {//Clear brushes
                    Destroy(child.gameObject);
                }

            }

           
        }
       

    }
}
