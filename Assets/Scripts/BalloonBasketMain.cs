using UnityEngine;
using System.Collections;

namespace BalloonBasket {
    public class BalloonBasketMain : MonoBehaviour {
    	public Transform staticRoot;
    	public GameObject bg;

    	public Transform dynamicRoot;

    	public Vector2 speed = new Vector2(0.1f, 0.0f);
    	public float rotationAngle = 0.0f;
    	
        public Rect screenRect;

        private int _maxMines = 3;
        private int _currMines = 0;

    	void Start () {
            Utils.InitTexture(this.bg, staticRoot, "BackgroundFinal", "Unlit/Transparent");
            this.bg.transform.localScale = new Vector3(1.35f, 1f, 1f);

            GameObject shipObj = (GameObject)GameObject.Instantiate(Utils.LoadResource("Ship"));
            shipObj.transform.parent = this.dynamicRoot;
            Utils.SetTransform(shipObj.transform, new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.2f, 0.2f, 1.0f));

            InvokeRepeating("MakeMine", 0.0f, 0.25f);
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

            foreach(Transform t in this.dynamicRoot.transform) {
                Mine mine = t.gameObject.GetComponent<Mine>();
                if(mine != null) {
                    //if(!this.screenRect.Contains(mine.transform.localPosition)){
                    if(mine.transform.localPosition.x < -1.5f || mine.transform.localPosition.y > 1.5f/* || mine.transform.localPosition.y < 1.5f || mine.transform.localPosition.x > 1.5f*/){
                       mine.Explode();
                    }
                }
            }
    	}

        private void MakeMine() {
            if(this._currMines < this._maxMines) {
                InstantiateMine(new Vector3(Random.Range(0.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0.0f));
            }
        }

        private void InstantiateMine(Vector3 position) {
            GameObject mineObj = (GameObject)GameObject.Instantiate(Utils.LoadResource("Mine"));
            mineObj.transform.parent = this.dynamicRoot;
            Utils.SetTransform(mineObj.transform, position, new Vector3(0.2f, 0.2f, 1.0f));
            mineObj.rigidbody2D.AddForce(new Vector2(-speed.x*100.0f, 0.0f));
            mineObj.GetComponent<Mine>()._explodeAnim.onFinish += this.OnMineDestroy;
            ++this._currMines;
        }

        private void OnMineDestroy() {
            --this._currMines;
        }
    }
}
