using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using BalloonBasket.Tech;

namespace BalloonBasket.Game {
    public class Ship : MonoBehaviour {
		public static readonly string PREFAB_NAME = "Ship";

		private static readonly string ANCHOR_NAME = "ShipAnchor";
		private static readonly string RANDOM_FORCE_METHODNAME = "RandomForce";

        [SerializeField] private SpriteRenderer _ship;
        [SerializeField] public BalloonBasketMain main;
        [SerializeField] private Vector2 force = new Vector2(500f, 500f);
        [SerializeField] private int _startBalloonCount;
		[SerializeField] private int _maxBalloonCount;
		[SerializeField] private GameObject _lineEndpoint;
		[SerializeField] private SliderJoint2D _sliderJoint;

        private List<Balloon> _balloons;
		private GameObjectPool _balloonPool;
		private bool _balloonChangeInProgress = false;
		private Buttons _buttons;

		public Vector2 BobbingRange = new Vector2(1500f, 3000f);

        void Start() {
			this._buttons = this.GetComponent<Buttons> ();
			this.transform.localPosition = Vector3.zero;

			this._balloonPool = this.gameObject.AddComponent<GameObjectPool>();
			this._balloonPool.ResourceId = Balloon.PREFAB_NAME;
			this._balloonPool.Size = this._maxBalloonCount;
			this._balloonPool.CreatePool();

			this._balloons = new List<Balloon>(this._maxBalloonCount);
			for(int i = 0; i < this._startBalloonCount; ++i) {
				this._balloons.Add(InstantiateBalloon(this._lineEndpoint.transform.localPosition + new Vector3(0.0f, 150f, 0.0f)));
			}

			this._sliderJoint.connectedBody = GameObject.Find(Ship.ANCHOR_NAME).GetComponent<Rigidbody2D>();
			this.BobbingRange = new Vector2(500f, 750f);

			Invoke(Ship.RANDOM_FORCE_METHODNAME, Random.Range(1f, 10f));
        }

        private Balloon InstantiateBalloon(Vector3 position) {
			GameObject obj = this._balloonPool.Instantiate();
            Vector3 origScale = obj.transform.localScale;
            obj.transform.parent = main.dynamicRoot;
            Utils.SetTransform(obj.transform, position, origScale);

            Balloon balloon = obj.GetComponent<Balloon>();
            balloon.Ship = this;
            balloon.SetJointDistance(Random.Range(150f, 200f));
            balloon.onPopped += this.OnBalloonPopped;
			balloon.onPumped += this.OnBalloonPumped;
            return balloon;
        }

		private void RandomForce() {
			Vector2 force = GenerateRandomForce(Random.Range(1f,10f) > 5f);
			this.rigidbody2D.AddForce(force);
			Debug.Log (string.Format("Force {0} at {1}", force.ToString(), Time.time));
			Invoke(Ship.RANDOM_FORCE_METHODNAME, Random.Range(1f, 10f));
		}

        private void OnBalloonPopped(Balloon balloon) {
            this._balloons.Remove(balloon);
			this._balloonPool.Destroy(balloon.gameObject);
			this._balloonChangeInProgress = false;
        }

		private void OnBalloonPumped(Balloon balloon) {
			this._balloonChangeInProgress = false;
		}

        void Update() {
			this._buttons.PollState ();

            Vector3 shipOffset = Vector2.zero;
			if(Input.GetKeyDown(KeyCode.A) || this._buttons.LeftPressed) {
                shipOffset.x -= force.x;
            }
			if(Input.GetKeyDown(KeyCode.D) || this._buttons.RightPressed) {
                shipOffset.x += force.x;
            }
			if(Input.GetKeyDown(KeyCode.W) || this._buttons.UpPressed) {
                shipOffset.y += force.y;
            }
			if(Input.GetKeyDown(KeyCode.S) || this._buttons.DownPressed) {
                shipOffset.y -= force.y;
            }
            
            this.rigidbody2D.AddForce(shipOffset);

			int oldCount = this._balloons.Count;
			if(Input.GetKeyDown(KeyCode.O) || this._buttons.CrossPressed) {
				if(!this._balloonChangeInProgress && this._balloons.Count < this._maxBalloonCount) {
					this._balloonChangeInProgress = true;
                    this._balloons.Add(InstantiateBalloon(this.transform.localPosition + new Vector3(0.0f, 150f, 0.0f)));
                }
            }
			if(Input.GetKeyDown(KeyCode.P) || this._buttons.TrianglePressed) {
				if(!this._balloonChangeInProgress && this._balloons.Count > 0) {
					this._balloonChangeInProgress = true;
                    this._balloons[this._balloons.Count-1].Pop();
                }
            }

			UpdateSliderPosition(oldCount);
        }

		private void UpdateSliderPosition(int oldCount) {
			int count = this._balloons.Count;
			JointTranslationLimits2D limits = new JointTranslationLimits2D();
			if(count > 12) {
				limits.min = 150f;
				limits.max = 250f;
			} else if(count > 9) {
				limits.min = 50f;
				limits.max = 150f;
			} else if(count > 6) {
				limits.min = -50f;
				limits.max = 50f;
			} else if(count > 3) {
				limits.min = -150f;
				limits.max = -50f;
			} else {
				limits.min = -275f;
				limits.max = -200f;
			}
			this._sliderJoint.limits = limits;

			if(this.transform.localPosition.y < limits.min) {
				Debug.Log ("Ship too low");
				//CancelInvoke(Ship.RANDOM_FORCE_METHODNAME);
				//Invoke(Ship.RANDOM_FORCE_METHODNAME, Random.Range(2f, 10f));
				//Vector2 randomForce = GenerateRandomForce(true);
				Vector2 randomForce = new Vector2(0f, 10000f);
				this.rigidbody2D.AddForce(randomForce);
			} else if(this.transform.localPosition.y > limits.max) {
				Debug.Log ("Ship too high");
				//CancelInvoke(Ship.RANDOM_FORCE_METHODNAME);
				//Invoke(Ship.RANDOM_FORCE_METHODNAME, Random.Range(2f, 10f));
				//Vector2 randomForce = GenerateRandomForce(false);
				Vector2 randomForce = new Vector2(0f, -10000f);
				this.rigidbody2D.AddForce(randomForce);
			}
		}

		private Vector2 GenerateRandomForce(bool upwards) {
			float random = Random.Range(this.BobbingRange.x, this.BobbingRange.y);
			return new Vector2(0f, upwards ? random : -random);
		}

		public Transform GetLineEndpoint() {
			return this._lineEndpoint.transform;
		}
    }
}
