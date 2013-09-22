using UnityEngine;
using System.Collections;

namespace BalloonBasket {
    public class BalloonBasketMain : MonoBehaviour {
    	public Transform staticRoot;
    	public GameObject bg;

    	public Transform dynamicRoot;

    	public Vector2 speed = new Vector2(0.1f, 0.0f);
    	public float rotationAngle = 0.0f;
    	
    	void Start () {
            Utils.InitTexture(this.bg, staticRoot, "BackgroundFinal", "Unlit/Transparent");
            this.bg.transform.localScale = new Vector3(1.35f, 1f, 1f);
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
    	}
    }
}
