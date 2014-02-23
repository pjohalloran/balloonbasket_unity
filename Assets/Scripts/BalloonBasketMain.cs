﻿using UnityEngine;
using System.Collections;

namespace BalloonBasket {
    public class BalloonBasketMain : MonoBehaviour {
        [SerializeField] private Transform _staticRoot;
        [SerializeField] private Transform _nearLayer;
        [SerializeField] private float _nearSpeed = 0.5f;
        [SerializeField] private Transform _midLayer;
        [SerializeField] private float _midSpeed = 0.25f;
        [SerializeField] private Transform _farLayer;
        [SerializeField] private float _farSpeed = 0.1f;
        [SerializeField] private GameObject _bg;
        [SerializeField] public Transform dynamicRoot;
        [SerializeField] private Vector2 _speed = new Vector2(0.1f, 0.0f);
        [SerializeField] private AnimationCurve _scrollCurve;
        [SerializeField] private AnimationCurve _spawnCurve;
        [SerializeField] private AnimationCurve _gustCurve;

        [SerializeField] private bool _spawnItems = true;

        private int _maxObstacles = 6;
        private int _currObstacles = 0;
        private float _lastExplosionTime;
        private float _lastGustTime;

    	void Start () {
            Utils.InitTexture(this._bg, _staticRoot, "BackgroundFinal", "Unlit/Transparent");
            this._bg.transform.localScale = new Vector3(512f, 384f, 1f);

            GameObject shipObj = (GameObject)GameObject.Instantiate(Utils.LoadResource("Ship"));
            shipObj.transform.parent = this.dynamicRoot;
            Utils.SetTransform(shipObj.transform, Vector3.zero);
            shipObj.GetComponent<Ship>().main = this;

            this._lastExplosionTime = 0.0f;
            this._lastGustTime = 0.0f;

            //Invoke("MakeRandomObstacle", this._spawnCurve.Evaluate(Time.time % 10.0f));
            Invoke("MakeRandomBg", this._spawnCurve.Evaluate(Time.time % 10.0f));

            GustEvents.OnGustEnterDelegate += this.OnGust;
    	}

        void OnDestroy() {
            GustEvents.OnGustEnterDelegate -= this.OnGust;
        }
    	
        private void OnGust() {
            Debug.Log("HIT GUST");
            this._lastGustTime = Time.time;
        }

        private void UpdateBgScroll() {
            Vector2 curr = this._bg.renderer.material.GetTextureOffset("_MainTex");
            this._bg.renderer.material.SetTextureOffset("_MainTex", curr + new Vector2(Mathf.Clamp01(Time.deltaTime*_speed.x), Mathf.Clamp01(Time.deltaTime*_speed.y)));
        }

    	void Update () {
            UpdateBgScroll();

            float scrollTimeDiff = Time.time - this._lastExplosionTime;
            float speedScroll = this._scrollCurve.Evaluate(scrollTimeDiff);

            float gustTimeDiff = Time.time - this._lastGustTime;
            float speedGust = 0f;
            if(gustTimeDiff > 0f) {
                speedGust = this._gustCurve.Evaluate(gustTimeDiff);
            }

            this._speed.x = Mathf.Clamp01(Mathf.Max(speedScroll, speedGust));

//            foreach(Transform t in this.dynamicRoot.transform) {
//                if(t.localPosition.x < -1.5f || t.localPosition.x > 2.0f || t.localPosition.y > 1.5f) {
//                    Destroy(t.gameObject);
//                    --this._currObstacles;
//                }
//            }

            foreach(Transform t in this._nearLayer) {
                if(t.localPosition.x < -600f) {
                    Destroy(t.gameObject);
                } else {
                    t.localPosition -= new Vector3(this._nearSpeed*this._speed.x*Time.deltaTime, 0.0f, 0.0f);
                }
            }
            foreach(Transform t in this._midLayer) {
                if(t.localPosition.x < -600f) {
                    Destroy(t.gameObject);
                } else {
                    t.localPosition -= new Vector3(this._midSpeed*this._speed.x*Time.deltaTime, 0.0f, 0.0f);
                }
            }
            foreach(Transform t in this._farLayer) {
                if(t.localPosition.x < -600f) {
                    Destroy(t.gameObject);
                } else {
                    t.localPosition -= new Vector3(this._farSpeed*this._speed.x*Time.deltaTime, 0.0f, 0.0f);
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
            int res = Random.Range(0, 2);

            if(res == 1) {
                MakeMine();
            } else {
                MakeGull();
            }

            float nextTime = this._spawnCurve.Evaluate(Time.time % 10.0f);
            //Debug.Log ("Spawning again in "+nextTime);
            Invoke("MakeRandomObstacle", nextTime);
        }

        private void MakeRandomBg() {
            int layerRes = Random.Range(0, 3);
            Transform t = null;

            Vector3 position = Vector3.zero;

            if(layerRes == 1) {
                t = this._nearLayer;
                position.x = Random.Range(-550f, 600f);
            } else if(layerRes == 2){
                t = this._midLayer;
				position.x = Random.Range(-550f, 600f);
            } else {
                t = this._farLayer;
				position.x = Random.Range(-550f, 600f);
            }

            int typeRes = Random.Range(0, 4);
            Texture2D tex = null;

            if(typeRes == 1) {
                tex = Utils.LoadResource("Cloud"+Random.Range(1, 5)) as Texture2D;
				position.y = Random.Range(-100f, 384f);
            } else if(typeRes == 2) {
                tex = Utils.LoadResource("Ground"+Random.Range(1, 6)) as Texture2D;
                position.y = -512f;
            } else if(typeRes == 3) {
                tex = Utils.LoadResource("Tree"+Random.Range(1, 2)) as Texture2D;
                position.y = -512f;
            } else {
				position.y = Random.Range(-384f, 384f);
                InstantiateBg(position, "Gust", this._nearLayer);
            }

            if(typeRes != 0) {
                GameObject obj = InstantiateBg(position, "Scenery", t);
                obj.GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex,
                                                                          new Rect(0f, 0f, tex.width, tex.height),
                                                                          new Vector2(0.5f, 0.5f), 1f);
                obj.GetComponent<SpriteRenderer>().sortingOrder = layerRes;
            }

            float nextTime = this._spawnCurve.Evaluate(Time.time % 10.0f);
            //Debug.Log ("Spawning again in "+nextTime);
            Invoke("MakeRandomBg", nextTime);
        }

        private GameObject InstantiateBg(Vector3 position, string prefabName, Transform parent) {
            GameObject obj = (GameObject)GameObject.Instantiate(Utils.LoadResource(prefabName));
            Vector3 origScale = obj.transform.localScale;
            obj.transform.parent = parent;
            Utils.SetTransform(obj.transform, position, origScale);
            return obj;
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
