using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using BalloonBasket.Tech;
using BalloonBasket.Game;

//namespace BalloonBasket.Game {
    public class BalloonBasketMain : MonoBehaviour {
		[SerializeField] private ScoreBoard _timer;
		//[SerializeField] private MenuButton _pauseButton; // TODO
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

		[SerializeField] private TextMesh _speedText;

		[SerializeField] public List<Vector3> _lanePositions = new List<Vector3>(5);
		[SerializeField] private float _timeBetweenColumns = 5f; // TODO probably not right to have a fixed time..

        private int _maxObstacles = 6;
        private int _currObstacles = 0;
        private float _lastExplosionTime;
        private float _lastGustTime;
		private Buttons _buttons;
		private float _lastColumnTime = 0;

		private List<List<char>> _levelData = null;
		private int _colIndex = 0;
		private bool _random = true;

		public void StartLevel(List<List<char>> levelData) {
			this._levelData = levelData;
			this._random = false;
			Invoke("MakeRandomBg", this._spawnCurve.Evaluate(Time.time));
		}

		public void StartRandom() {
			this._random = true;
			Invoke("MakeRandomObstacle", this._spawnCurve.Evaluate(Time.time));
			Invoke("MakeRandomBg", this._spawnCurve.Evaluate(Time.time));
		}

    	void Start () {
			this._buttons = this.GetComponent<Buttons>();

            Utils.InitTexture(this._bg, _staticRoot, "BackgroundFinal", "Unlit/Transparent");
            this._bg.transform.localScale = new Vector3(512f, 384f, 1f);

            GameObject shipObj = (GameObject)GameObject.Instantiate(Utils.LoadResource(Ship.PREFAB_NAME));
            shipObj.transform.parent = this.dynamicRoot;
            Utils.SetTransform(shipObj.transform, Vector3.zero);
            shipObj.GetComponent<Ship>().main = this;

            this._lastExplosionTime = 0.0f;
            this._lastGustTime = 0.0f;

            GustEvents.OnGustEnterDelegate += this.OnGust;
    	}

        void OnDestroy() {
            GustEvents.OnGustEnterDelegate -= this.OnGust;
        }
    	
        private void OnGust() {
            this._lastGustTime = Time.time;
        }

    	private void Update () {
			this._buttons.PollState ();

			if (this._buttons.StartPressed || Input.GetKeyUp(KeyCode.Escape)) {
				// reload level
				LoadMainMenu();
			}

			this._timer.SetScore(Time.timeSinceLevelLoad);

            float scrollTimeDiff = Time.time - this._lastExplosionTime;
            float speedScroll = this._scrollCurve.Evaluate(scrollTimeDiff) * 500f;

            float gustTimeDiff = Time.time - this._lastGustTime;
            float speedGust = 0f;
            if(gustTimeDiff > 0f) {
                speedGust = this._gustCurve.Evaluate(gustTimeDiff) * 500f;
            }

			this._speed.x = Mathf.Max(speedScroll, speedGust);
			int speed = Mathf.FloorToInt(this._speed.x);
			this._speedText.text = string.Format("{0} km/h", Mathf.Floor(this._speed.x));
			if(speed > 356) {
				this._speedText.color = Color.red;
			} else if(speed > 275) {
				this._speedText.color = Color.green;
			} else {
				this._speedText.color = Color.white;
			}
			
			UpdateBgScroll();
			CheckObstacles();
			UpdateScenery(this._nearLayer, this._nearSpeed);
			UpdateScenery(this._midLayer, this._midSpeed);
			UpdateScenery(this._farLayer, this._farSpeed);

			if(!this._random) {
				CheckForNewColumn();
			}
        }

		private void CheckForNewColumn() {
			if(Time.time - this._lastColumnTime >= this._timeBetweenColumns) {
				bool flagMade = false;
				for(int rowIndex = 0; rowIndex < this._levelData.Count; ++rowIndex) {
					char c = this._levelData[rowIndex][this._colIndex];
					if(c == LevelReader.O_MINE) {
						MakeMine(rowIndex);
					} else if(c == LevelReader.O_SEAGULL) {
						MakeGull(rowIndex);
					} else if(c == LevelReader.O_FINISH && !flagMade) {
						MakeFlag(0);
						flagMade = true;
					} else if(c == LevelReader.O_WIND150 || c == LevelReader.O_WIND200) {
						MakeGust(rowIndex/*, c*/);//TODO type of gust
					}
				}
				++this._colIndex;
				this._lastColumnTime = Time.time;
			}
		}

		private void UpdateBgScroll() {
			Vector2 curr = this._bg.renderer.material.GetTextureOffset("_MainTex");
			this._bg.renderer.material.SetTextureOffset("_MainTex", curr + new Vector2(Time.deltaTime*_speed.x * 0.001f, Time.deltaTime*_speed.y * 0.001f));
		}

		private void CheckObstacles() {
			float halfScreenWidth = Screen.width * 0.5f;
			foreach(Transform t in this.obstacleRoot.transform) {
				if(t.localPosition.x < -halfScreenWidth - 400f) {
					Destroy(t.gameObject);
					--this._currObstacles;
				} else {
					t.localPosition -= new Vector3(this._speed.x * Time.deltaTime, 0.0f, 0.0f);
				}
			}
		}

		private void UpdateScenery(Transform parent, float baseSpeed) {
			float halfScreenWidth = Screen.width * 0.5f;
			float graceX = 600f;
			float maxLayerX = (1f / parent.localScale.x) * halfScreenWidth + graceX;

			foreach(Transform t in parent) {
				if(t.localPosition.x < -maxLayerX) {
					Destroy(t.gameObject);
				} else {
					t.localPosition -= new Vector3(baseSpeed * this._speed.x * Time.deltaTime, 0.0f, 0.0f);
				}
			}
		}
        
		private GameObject MakeObstacle(string prefabName, int lane) {
			float halfScreenWidth = Screen.width * 0.5f;
			float halfScreenHeight = Screen.height * 0.5f;
			
			Vector3 startPosition = Vector3.zero;
			if(this._random) {
				startPosition = new Vector3(halfScreenWidth+200f, Random.Range(-halfScreenHeight, halfScreenHeight), 0f);
			} else {
				startPosition = this._lanePositions[lane];
			}
			
			return InstantiateObstacle(startPosition, prefabName);
		}

        private void MakeMine(int lane = -1) {
			GameObject obj = MakeObstacle(Mine.PREFAB_NAME, lane);
        	obj.GetComponent<Mine>()._explodeAnim.onFinish += this.OnMineDestroy;
        	obj.GetComponent<Mine>().onExplodeShip += this.OnMineCollideShip;
        }

		private void MakeGull(int lane = -1) {
			GameObject obj = MakeObstacle(Gull.PREFAB_NAME, lane);
        	obj.GetComponent<Gull>().onDeath = this.OnGullDestroy;
        }

		private void MakeFlag(int lane = -1) {
			GameObject obj = MakeObstacle(Flag.PREFAB_NAME, lane);
			obj.GetComponent<Flag>().onShipEnter -= this.OnShipHitFlag;
			obj.GetComponent<Flag>().onShipEnter += this.OnShipHitFlag;
		}

		private void MakeGust(int lane = -1) {
			GameObject obj = MakeObstacle(Gust.PREFAB_NAME, lane);
			
		}

        private void MakeRandomObstacle() {
			if(this._random && this._currObstacles < this._maxObstacles) {
				int res = Random.Range(0, 3);
				
				if(res == 1) {
					MakeMine();
				} else if(res == 2){
					MakeGull();
				} else {
					MakeFlag();
				}
			}

            //Debug.Log ("Spawning again in "+nextTime);
			Invoke("MakeRandomObstacle", this._spawnCurve.Evaluate(Time.time)*5f);
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

			int typeRes = Random.Range(0, 4);
			Texture2D tex = null;
			float graceX = 200f;

			if(typeRes == 3) {
				// Fix trees in near layer
				t = this._nearLayer;
			}

			maxLayerX = (1f / t.localScale.x) * halfScreenWidth + graceX;
			maxLayerY = (1f / t.localScale.y) * halfScreenHeight;
			position.x = maxLayerX;
			position.y = typeRes != 3 ? Random.Range(-maxLayerY, maxLayerY) : -maxLayerY; // fix at bottom for trees

            if(typeRes == 1) {
                tex = Utils.LoadResource("Cloud"+Random.Range(1, 5)) as Texture2D;
				position.y = Random.Range(-100f, maxLayerY);
            } else if(typeRes == 2) {
                tex = Utils.LoadResource("Ground"+Random.Range(1, 6)) as Texture2D;
				position.y = -maxLayerY;
			} else if(this._random && typeRes == 3) { // TODO: Move out for level data
                tex = Utils.LoadResource("Tree"+Random.Range(1, 2)) as Texture2D;
				position.y = -maxLayerY;
			} else if(this._random) { // TODO: Move out for level data
                InstantiateBg(position, "Gust", this._nearLayer);
            }

            if(typeRes != 0) {
                GameObject obj = InstantiateBg(position, "Scenery", t);
                obj.GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex,
                                                                          new Rect(0f, 0f, tex.width, tex.height),
                                                                          new Vector2(0.5f, 0.5f), 1f);
                obj.GetComponent<SpriteRenderer>().sortingOrder = layerRes;
            }

            float nextTime = this._spawnCurve.Evaluate(Time.time);
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
			if(obj.rigidbody2D != null) { // TODO might not need this null check, think flag has a rigid body
            	obj.rigidbody2D.AddForce(new Vector2(-_speed.x*100.0f, 0.0f));
			}
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

		private void OnShipHitFlag() {
			PlayerPrefs.SetFloat(Menu.SCORE_KEY, Time.timeSinceLevelLoad);
			PlayerPrefs.Save();
			Invoke("LoadMainMenu", 2f);
		}

		private void LoadMainMenu() {
			Application.LoadLevel("game");
		}
    }
//}
