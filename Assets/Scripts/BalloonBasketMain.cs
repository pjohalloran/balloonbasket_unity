using UnityEngine;
using System.Collections;

namespace BalloonBasket {
    public class BalloonBasketMain : MonoBehaviour {
        [SerializeField] private Transform _staticRoot;
        [SerializeField] private GameObject _bg;
        [SerializeField] public Transform dynamicRoot;
        [SerializeField] private Vector2 _speed = new Vector2(0.1f, 0.0f);
        [SerializeField] private float _rotationAngle = 0.0f;
        [SerializeField] private AnimationCurve _scrollCurve;

        [SerializeField] private bool _spawnItems = true;

        private int _maxMines = 3;
        private int _currMines = 0;
        private float _lastExplosionTime;

    	void Start () {
            Utils.InitTexture(this._bg, _staticRoot, "BackgroundFinal", "Unlit/Transparent");
            this._bg.transform.localScale = new Vector3(1.35f, 1f, 1f);

            GameObject shipObj = (GameObject)GameObject.Instantiate(Utils.LoadResource("Ship"));
            shipObj.transform.parent = this.dynamicRoot;
            Utils.SetTransform(shipObj.transform, new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.2f, 0.2f, 1.0f));
            shipObj.GetComponent<Ship>().main = this;

            this._lastExplosionTime = 0.0f;

            InvokeRepeating("MakeMine", 0.0f, 0.25f);
    	}
    	
        private void UpdateBgScroll() {
            Vector2 curr = this._bg.renderer.material.GetTextureOffset("_MainTex");
            this._bg.renderer.material.SetTextureOffset("_MainTex", curr + new Vector2(Mathf.Clamp01(Time.deltaTime*_speed.x), Mathf.Clamp01(Time.deltaTime*_speed.y)));
        }

    	void Update () {
            UpdateBgScroll();

            float timeDiff = Time.time - this._lastExplosionTime;
            this._speed.x = this._scrollCurve.Evaluate(timeDiff);
    		_speed.x = Mathf.Clamp01(_speed.x);

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
            if(this._spawnItems && this._currMines < this._maxMines) {
                InstantiateMine(new Vector3(Random.Range(1.0f, 1.1f), Random.Range(-1.0f, 1.0f), 0.0f));
            }
        }

        private void InstantiateMine(Vector3 position) {
            GameObject mineObj = (GameObject)GameObject.Instantiate(Utils.LoadResource("Mine"));
            Vector3 origScale = mineObj.transform.localScale;
            mineObj.transform.parent = this.dynamicRoot;
            Utils.SetTransform(mineObj.transform, position, origScale);
            mineObj.rigidbody2D.AddForce(new Vector2(-_speed.x*100.0f, 0.0f));
            mineObj.GetComponent<Mine>()._explodeAnim.onFinish += this.OnMineDestroy;
            mineObj.GetComponent<Mine>().onExplodeShip += this.OnMineCollideShip;
            ++this._currMines;
        }

        private void OnMineCollideShip() {
            this._lastExplosionTime = Time.time;
        }

        private void OnMineDestroy() {
            --this._currMines;
        }
    }
}
