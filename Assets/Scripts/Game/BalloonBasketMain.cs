using System.Collections;

using UnityEngine;

using BalloonBasket.Tech;

namespace BalloonBasket.Game {
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
		[SerializeField] public Transform obstacleRoot;
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

            GameObject shipObj = (GameObject)GameObject.Instantiate(Utils.LoadResource(Ship.PREFAB_NAME));
            shipObj.transform.parent = this.dynamicRoot;
            Utils.SetTransform(shipObj.transform, Vector3.zero);
            shipObj.GetComponent<Ship>().main = this;

            this._lastExplosionTime = 0.0f;
            this._lastGustTime = 0.0f;

            Invoke("MakeRandomObstacle", this._spawnCurve.Evaluate(Time.time % 10.0f));
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

    	private void Update () {
            float scrollTimeDiff = Time.time - this._lastExplosionTime;
            float speedScroll = this._scrollCurve.Evaluate(scrollTimeDiff) * 500f;

            float gustTimeDiff = Time.time - this._lastGustTime;
            float speedGust = 0f;
            if(gustTimeDiff > 0f) {
                speedGust = this._gustCurve.Evaluate(gustTimeDiff) * 500f;
            }

			this._speed.x = Mathf.Max(speedScroll, speedGust);

			UpdateBgScroll();
			CheckObstacles();
			UpdateScenery(this._nearLayer, this._nearSpeed);
			UpdateScenery(this._midLayer, this._midSpeed);
			UpdateScenery(this._farLayer, this._farSpeed);
        }

		private void UpdateBgScroll() {
			Vector2 curr = this._bg.renderer.material.GetTextureOffset("_MainTex");
			this._bg.renderer.material.SetTextureOffset("_MainTex", curr + new Vector2(Time.deltaTime*_speed.x * 0.001f, Time.deltaTime*_speed.y * 0.001f));
		}

		private void CheckObstacles() {
			float halfScreenWidth = Screen.width * 0.5f;
			foreach(Transform t in this.obstacleRoot.transform) {
				if(t.localPosition.x < -halfScreenWidth) {
					Destroy(t.gameObject);
					--this._currObstacles;
				} else {
					t.localPosition -= new Vector3(this._speed.x * Time.deltaTime, 0.0f, 0.0f);
				}
			}
		}

		private void UpdateScenery(Transform parent, float baseSpeed) {
			float halfScreenWidth = Screen.width * 0.5f;
			float graceX = 200f;
			float maxLayerX = (1f / parent.localScale.x) * halfScreenWidth + graceX;

			foreach(Transform t in parent) {
				if(t.localPosition.x < -maxLayerX) {
					Destroy(t.gameObject);
				} else {
					t.localPosition -= new Vector3(baseSpeed * this._speed.x * Time.deltaTime, 0.0f, 0.0f);
				}
			}
		}
        
        private void MakeMine() {
            if(this._spawnItems && this._currObstacles < this._maxObstacles) {
				float halfScreenWidth = Screen.width * 0.5f;
				float halfScreenHeight = Screen.height * 0.5f;
				GameObject obj = InstantiateObstacle(new Vector3(halfScreenWidth+200f, Random.Range(-halfScreenHeight, halfScreenHeight), 0f), Mine.PREFAB_NAME);
                obj.GetComponent<Mine>()._explodeAnim.onFinish += this.OnMineDestroy;
                obj.GetComponent<Mine>().onExplodeShip += this.OnMineCollideShip;
            }
        }

        private void MakeGull() {
            if(this._spawnItems && this._currObstacles < this._maxObstacles) {
				float halfScreenWidth = Screen.width * 0.5f;
				float halfScreenHeight = Screen.height * 0.5f;
				GameObject obj = InstantiateObstacle(new Vector3(halfScreenWidth+200f, Random.Range(-halfScreenHeight, halfScreenHeight), 0f), Gull.PREFAB_NAME);
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
			float halfScreenWidth = Screen.width * 0.5f;
			float halfScreenHeight = Screen.height * 0.5f;
			float maxLayerX = 0f;
			float maxLayerY = 0f;

            if(layerRes == 1) {
                t = this._nearLayer;
            } else if(layerRes == 2){
                t = this._midLayer;
            } else {
                t = this._farLayer;
            }

			float graceX = 200f;

			maxLayerX = (1f / t.localScale.x) * halfScreenWidth + graceX;
			maxLayerY = (1f / t.localScale.y) * halfScreenHeight;
			position.x = maxLayerX;
			position.y = Random.Range(-maxLayerY, maxLayerY);

            int typeRes = Random.Range(0, 4);
            Texture2D tex = null;

            if(typeRes == 1) {
                tex = Utils.LoadResource("Cloud"+Random.Range(1, 5)) as Texture2D;
				position.y = Random.Range(-100f, maxLayerY);
            } else if(typeRes == 2) {
                tex = Utils.LoadResource("Ground"+Random.Range(1, 6)) as Texture2D;
				position.y = -maxLayerY;
            } else if(typeRes == 3) {
                tex = Utils.LoadResource("Tree"+Random.Range(1, 2)) as Texture2D;
				position.y = -maxLayerY;
            } else {
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
            obj.transform.parent = this.obstacleRoot;
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
