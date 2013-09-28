using UnityEngine;
using System.Collections;

namespace BalloonBasket {
    public class BalloonBasketMain : MonoBehaviour {
        [SerializeField] private Transform _staticRoot;
        [SerializeField] private GameObject _bg;
        [SerializeField] public Transform dynamicRoot;
        [SerializeField] private Vector2 _speed = new Vector2(0.1f, 0.0f);
        [SerializeField] private AnimationCurve _scrollCurve;
        [SerializeField] private AnimationCurve _spawnCurve;

        [SerializeField] private bool _spawnItems = true;

        private int _maxObstacles = 6;
        private int _currObstacles = 0;
        private float _lastExplosionTime;

    	void Start () {
            Utils.InitTexture(this._bg, _staticRoot, "BackgroundFinal", "Unlit/Transparent");
            this._bg.transform.localScale = new Vector3(1.35f, 1f, 1f);

            GameObject shipObj = (GameObject)GameObject.Instantiate(Utils.LoadResource("Ship"));
            shipObj.transform.parent = this.dynamicRoot;
            Utils.SetTransform(shipObj.transform, new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.2f, 0.2f, 1.0f));
            shipObj.GetComponent<Ship>().main = this;

            this._lastExplosionTime = 0.0f;

            //InvokeRepeating("MakeMine", 0.0f, 0.5f);
            //InvokeRepeating("MakeGull", 0.0f, 0.5f);

            Invoke("MakeRandomObstacle", this._spawnCurve.Evaluate(Time.time % 10.0f));
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
                        Destroy(mine.gameObject);
                        --this._currObstacles;
                    }
                } else {
                    Gull gull = t.gameObject.GetComponent<Gull>();
                    if(gull != null) {
                        //if(!this.screenRect.Contains(mine.transform.localPosition)){
                        if(gull.transform.localPosition.x < -1.5f || gull.transform.localPosition.y > 1.5f/* || mine.transform.localPosition.y < 1.5f || mine.transform.localPosition.x > 1.5f*/){
                            Destroy(gull.gameObject);
                            --this._currObstacles;
                        }
                    }
                }
            }
    	}

        private void MakeMine() {
            if(this._spawnItems && this._currObstacles < this._maxObstacles) {
                GameObject obj = InstantiateObstacle(new Vector3(Random.Range(1.0f, 1.1f), Random.Range(-1.0f, 1.0f), 0.0f), "Mine");
                obj.GetComponent<Mine>()._explodeAnim.onFinish += this.OnMineDestroy;
                obj.GetComponent<Mine>().onExplodeShip += this.OnMineCollideShip;
            }
        }

        private void MakeGull() {
            if(this._spawnItems && this._currObstacles < this._maxObstacles) {
                GameObject obj = InstantiateObstacle(new Vector3(Random.Range(1.0f, 1.5f), Random.Range(-1.0f, 1.0f), 0.0f), "Gull");
                obj.GetComponent<Gull>().onDeath = this.OnGullDestroy;
            }
        }

        private void MakeRandomObstacle() {
            int res = Random.Range(1, 10);

            if(res < 5) {
                MakeMine();
            } else {
                MakeGull();
            }

            float nextTime = this._spawnCurve.Evaluate(Time.time % 10.0f);
            Debug.Log ("Spawning again in "+nextTime);
            Invoke("MakeRandomObstacle", nextTime);
        }

        private GameObject InstantiateObstacle(Vector3 position, string prefabName) {
            GameObject obj = (GameObject)GameObject.Instantiate(Utils.LoadResource(prefabName));
            Vector3 origScale = obj.transform.localScale;
            obj.transform.parent = this.dynamicRoot;
            Utils.SetTransform(obj.transform, position, origScale);
            obj.rigidbody2D.AddForce(new Vector2(-_speed.x*100.0f, 0.0f));
            ++this._currObstacles;
            return obj;
        }

        private void OnMineCollideShip() {
            this._lastExplosionTime = Time.time;
        }

        private void OnMineDestroy() {
            --this._currObstacles;
        }

        private void OnGullDestroy(Gull gull) {
            --this._currObstacles;
        }
    }
}
