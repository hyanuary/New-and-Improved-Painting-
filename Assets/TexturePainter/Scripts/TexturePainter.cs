/// <summary>
/// CodeArtist.mx 2015
/// This is the main class of the project, its in charge of raycasting to a model and place brush prefabs infront of the canvas camera.
/// If you are interested in saving the painted texture you can use the method at the end and should save it to a file.
/// </summary>


using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum Painter_BrushMode{PAINT,DECAL};
public class TexturePainter : MonoBehaviour {
	public GameObject brushCursor,brushContainer, stencil; //The cursor that overlaps the model and our container for the brushes painted
	public Camera sceneCamera,canvasCam;  //The camera that looks at the model, and the camera that looks at the canvas.
	public Sprite cursorPaint,cursorDecal; // Cursor for the differen functions 
	public RenderTexture canvasTexture; // Render Texture that lotor decals)
	float brushSize=1.0f; //The size of our brush
	Color brushColor; //The selected color
	public int brushCounter=0,MAX_BRUSH_COUNT=50; //To avoid havoks at our Base Texture and the painted brushes
	public Material baseMaterial; // The material of our base texing millions of brushes
	bool saving=false; //Flag to check if we are saving the texturure (Were we will save the painted texture)
    public Texture2D stencilSprite; //sprite for the stencil
	Painter_BrushMode mode; //Our painter mode (Paint brushes e
    Texture2D mainTex;

    void Start()
    {
        mainTex = new Texture2D(canvasTexture.width, canvasTexture.height, TextureFormat.RGB24, false);
    }


    void Update () {
		brushColor = ColorSelector.GetColor ();	//Updates our painted color with the selected color
		if (Input.GetMouseButton(0)) {
			DoAction();
		}
		UpdateBrushCursor ();

        //Invoke("SaveTexture", 0.1f);
    }

	//The main action, instantiates a brush or decal entity at the clicked position on the UV map
	void DoAction(){	
		if (saving)
			return;
		Vector3 uvWorldPosition=Vector3.zero;		
		if(HitTestUVPosition(ref uvWorldPosition)){
			GameObject brushObj;
			if(mode==Painter_BrushMode.PAINT){

				brushObj=(GameObject)Instantiate(Resources.Load("TexturePainter-Instances/BrushEntity")); //Paint a brush
				brushObj.GetComponent<SpriteRenderer>().color=brushColor; //Set the brush color
			}
			else{
				brushObj=(GameObject)Instantiate(Resources.Load("TexturePainter-Instances/DecalEntity")); //Paint a decal
			}
			brushColor.a=brushSize*2.0f; // Brushes have alpha to have a merging effect when painted over.
			brushObj.transform.parent=brushContainer.transform; //Add the brush to our container to be wiped later
			brushObj.transform.localPosition=uvWorldPosition; //The position of the brush (in the UVMap)
			brushObj.transform.localScale=Vector3.one*brushSize;//The size of the brush
		}
		brushCounter++; //Add to the max brushes
        if (brushCounter >= MAX_BRUSH_COUNT)
        { //If we reach the max brushes available, flatten the texture and clear the brushes
            brushCursor.SetActive(false);
            saving = true;
            Invoke("SaveTexture", 0.1f);

        }
    }
	//To update at realtime the painting cursor on the mesh
	void UpdateBrushCursor(){
		Vector3 uvWorldPosition=Vector3.zero;
		if (HitTestUVPosition (ref uvWorldPosition) && !saving) {
			brushCursor.SetActive(true);
			brushCursor.transform.position =uvWorldPosition+brushContainer.transform.position;									
		} else {
			brushCursor.SetActive(false);
		}		
	}
	//Returns the position on the texuremap according to a hit in the mesh collider
	bool HitTestUVPosition(ref Vector3 uvWorldPosition){

        RaycastHit[] hits;
        Vector3 cursorPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
        Ray cursorRay = sceneCamera.ScreenPointToRay(cursorPos);
        hits = Physics.RaycastAll(cursorRay,1000);
       

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit1 = hits[i];
            MeshCollider mesh1 = hits[i].collider as MeshCollider;
            if (mesh1 == null || mesh1.sharedMesh == null /*|| hits[0].collider.tag == "stencil"*/)
                return false;
            //Debug.Log(mesh1.tag);
            Vector2 pixelUV1 = new Vector2(hit1.textureCoord.x, hit1.textureCoord.y);
            //Debug.Log(pixelUV1);
            Color stencilColor = stencilSprite.GetPixel((int)(pixelUV1.x * stencilSprite.width), (int)(pixelUV1.y * stencilSprite.height));
            Color canvasColor = mainTex.GetPixel((int)(pixelUV1.x * mainTex.width), (int)(pixelUV1.y * mainTex.height));
            Debug.Log(canvasColor);
            //Debug.Log(stencilColor);
            uvWorldPosition.x = pixelUV1.x - canvasCam.orthographicSize;//To center the UV on X
            uvWorldPosition.y = pixelUV1.y - canvasCam.orthographicSize;//To center the UV on Y
            uvWorldPosition.z = 0.0f;

            //stencil detection
            if (stencilColor.a > 0.5f)
            {
                Debug.Log("white");
            }


            //return canvasColor.r > 1.0f;

        }
        return true;
        //return canvasColor.a < 0.5f;

        //RaycastHit hit /*= hits[i];
        //if (Physics.Raycast(cursorRay, out hit, 200))
        //{
        //    MeshCollider meshCollider = hit.collider as MeshCollider;
        //    if (meshCollider == null || meshCollider.sharedMesh == null /*|| meshCollider.tag == "canvas2"*/)
        //        return false;
        //    Vector2 pixelUV = new Vector2(hit.textureCoord.x, hit.textureCoord.y);
        //    Debug.Log(pixelUV);
        //    stencilSprite.GetPixel((int)(pixelUV.x * stencilSprite.width), (int)(pixelUV.y * stencilSprite.height));
        //    Color canvasColor = stencilSprite.GetPixel((int)(pixelUV.x * stencilSprite.width), (int)(pixelUV.y * stencilSprite.height));
        //    Debug.Log(canvasColor);
        //    uvWorldPosition.x = pixelUV.x - canvasCam.orthographicSize;//To center the UV on X
        //    uvWorldPosition.y = pixelUV.y - canvasCam.orthographicSize;//To center the UV on Y
        //    uvWorldPosition.z = 0.0f;
        //    return canvasColor.a < 0.5f;
        //}
        //else
        //{
        //    return false;
        //}




    }
    //Sets the base material with a our canvas texture, then removes all our brushes
    void SaveTexture()
    {
        brushCounter = 0;
        System.DateTime date = System.DateTime.Now;
        RenderTexture.active = canvasTexture;
        mainTex.ReadPixels(new Rect(0, 0, canvasTexture.width, canvasTexture.height), 0, 0);
        RenderTexture.active = null;
        baseMaterial.mainTexture = mainTex; //Put the painted texture as the base
        foreach (Transform child in brushContainer.transform)
        {//Clear brushes
            Destroy(child.gameObject);
        }


        // make everything white
        //for (int x = 0; x < tex.width; x++)
        //{
        //    for (int y = 0; y < tex.height; y++)
        //    {
        //        tex.SetPixel(x, y, Color.white);
        //    }
        //}

        //stencil part
        //RaycastHit hit;
        //if (Physics.Raycast(stencil.transform.position, Vector3.forward, out hit))
        //{
        //    Renderer rend = hit.transform.GetComponent<Renderer>();
        //    MeshCollider meshColl = hit.collider as MeshCollider;
        //    //Debug.Log(meshColl.tag);

        //    Vector2 pixelUV = hit.textureCoord;

        //    int x = (int)(pixelUV.x * tex.width);
        //    int y = (int)(pixelUV.y * tex.height);

        //    //tex.SetPixel(x, y, Color.white);
        //    //tex.Apply();
        //    Debug.Log(pixelUV);
        //}
        mainTex.Apply();
        //Debug.Log("clear");
        //Debug.Log(tex.GetPixel(2,10));
        //StartCoroutine ("SaveTextureToFile"); //Do you want to save the texture? This is your method!
        Invoke("ShowCursor", 0.1f);
    }


    //Show again the user cursor (To avoid saving it to the texture)
    void ShowCursor(){	
		saving = false;
	}



	////////////////// PUBLIC METHODS //////////////////

	public void SetBrushMode(Painter_BrushMode brushMode){ //Sets if we are painting or placing decals
		mode = brushMode;
		brushCursor.GetComponent<SpriteRenderer> ().sprite = brushMode == Painter_BrushMode.PAINT ? cursorPaint : cursorDecal;
	}
	public void SetBrushSize(float newBrushSize){ //Sets the size of the cursor brush or decal
		brushSize = newBrushSize;
		brushCursor.transform.localScale = Vector3.one * brushSize;
	}

	////////////////// OPTIONAL METHODS //////////////////

	#if !UNITY_WEBPLAYER 
		IEnumerator SaveTextureToFile(Texture2D savedTexture){		
			brushCounter=0;
			string fullPath=System.IO.Directory.GetCurrentDirectory()+"\\UserCanvas\\";
			System.DateTime date = System.DateTime.Now;
			string fileName = "CanvasTexture.png";
			if (!System.IO.Directory.Exists(fullPath))		
				System.IO.Directory.CreateDirectory(fullPath);
			var bytes = savedTexture.EncodeToPNG();
			System.IO.File.WriteAllBytes(fullPath+fileName, bytes);
			Debug.Log ("<color=orange>Saved Successfully!</color>"+fullPath+fileName);
			yield return null;
		}
	#endif
}
