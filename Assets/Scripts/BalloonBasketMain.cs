using UnityEngine;
using System.Collections;

public class BalloonBasketMain : MonoBehaviour {
	public Transform staticRoot;
	public GameObject bg;
	public SpriteRenderer ship;

	public Transform dynamicRoot;

	public Vector2 speed = new Vector2(0.1f, 0.0f);
	public float rotationAngle = 0.0f;

	private Mesh _m1;
	
	void Start () {
		Debug.Log ("Enter");
		DrawTexture();
		Debug.Log ("Exit");
	}
	
	// Create a quad mesh
	public Mesh CreateMesh() {
		
		Mesh mesh = new Mesh();

		int sw = Screen.width;
		int sh = Screen.height;

		Vector3[] vertices = new Vector3[]
		{
			new Vector3(1, 1,  0),
			new Vector3(1, -1, 0),
			new Vector3(-1, 1, 0),
			new Vector3(-1, -1, 0),
		};
		
		Vector2[] uv = new Vector2[]
		{
			new Vector2(1, 1),
			new Vector2(1, 0),
			new Vector2(0, 1),
			new Vector2(0, 0),
		};
		
		int[] triangles = new int[]
		{
			0, 1, 2,
			2, 1, 3,
		};
		
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		
		return mesh;
	}

	public void DrawTexture() {
		this.bg.AddComponent<MeshRenderer>();
		MeshFilter filter = this.bg.AddComponent<MeshFilter>();

		filter.mesh = CreateMesh();
		this.bg.transform.parent = this.staticRoot.transform;
		this.bg.transform.localPosition = new Vector3(0f, 0f, 1f);
		this.bg.transform.localScale = Vector3.one;

		// Set texture
		Texture tex = (Texture) Resources.Load("BackgroundFinal");
		if(tex == null) {
			Debug.Log ("NULL ");
		}
		this.bg.renderer.material.mainTexture = tex;

		Debug.Log ("3");

		// Set shader for this sprite; unlit supporting transparency
		// If we dont do this the sprite seems 'dark' when drawn. 
		Shader shader = Shader.Find ("Unlit/Transparent");
		if(shader == null) {
			Debug.Log ("S NULL ");
		}
		this.bg.renderer.material.shader = shader;
		Debug.Log ("4");
	}
	
	void Update () {
		Vector2 curr = this.bg.renderer.material.GetTextureOffset("_MainTex");
		this.bg.renderer.material.SetTextureOffset("_MainTex", curr + new Vector2(Mathf.Clamp01(Time.deltaTime*speed.x), Mathf.Clamp01(Time.deltaTime*speed.y)));

		if(Input.GetKeyDown(KeyCode.UpArrow)) {
			speed.y += 0.05f;
		} else if(Input.GetKeyDown(KeyCode.DownArrow)) {
			speed.y -= 0.05f;
		} else if(Input.GetKeyDown(KeyCode.LeftArrow)) {
			speed.x -= 0.05f;
		} else if(Input.GetKeyDown(KeyCode.RightArrow)) {
			speed.x += 0.05f;
		}
		speed.x = Mathf.Clamp01(speed.x);
		speed.y = Mathf.Clamp01(speed.y);

		if(Input.GetKeyDown(KeyCode.Q)) {
			rotationAngle += 0.10f;
		} else if(Input.GetKeyDown(KeyCode.E)) {
			rotationAngle -= 0.10f;
		}
		this.bg.transform.RotateAround(Vector3.zero, new Vector3(0f,0f,1f), rotationAngle*Time.deltaTime);

		Vector3 shipOffset = Vector2.zero;
		if(Input.GetKeyDown(KeyCode.A)) {
			shipOffset.x -= 100f;
		}
		if(Input.GetKeyDown(KeyCode.D)) {
			shipOffset.x += 100f;
		}
		if(Input.GetKeyDown(KeyCode.W)) {
			shipOffset.y += 100f;
		}
		if(Input.GetKeyDown(KeyCode.S)) {
			shipOffset.y -= 100f;
		}

		this.ship.GetComponent<Rigidbody2D>().AddForce(shipOffset * Time.deltaTime);
	}
}
