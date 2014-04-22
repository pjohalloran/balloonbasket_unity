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
			this.rigidbody2D.sleepMode = RigidbodySleepMode2D.NeverSleep;

			this._buttons = this.GetComponent<Buttons> ();
			this.transform.localPosition = Vector3.zero;

			this._balloonPool = this.gameObject.AddComponent<GameObjectPool>();
			this._balloonPool.ResourceId = Balloon.PREFAB_NAME;
			this._balloonPool.Size = this._maxBalloonCount;
			this._balloonPool.CreatePool();

			this._balloons = new List<Balloon>(this._maxBalloonCount);
			for(int i = 0; i < this._startBalloonCount; ++i) {
				Balloon balloon = InstantiateBalloon(this._lineEndpoint.transform.localPosition + new Vector3(0.0f, 150f, 0.0f));
				balloon.GetComponent<SpriteRenderer>().sortingOrder = i;
				this._balloons.Add(balloon);
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

		private void OnCollisionEnter2D(Collision2D collision) {
			if(collision.rigidbody != null && 
			   (collision.rigidbody.gameObject.GetComponent<Mine>() != null ||
			 	collision.rigidbody.gameObject.GetComponent<Gull>() != null)) {
				if(collision.rigidbody.IsAwake() && this._balloons.Count > 0 && !this._balloonChangeInProgress) {
					this._balloonChangeInProgress = true;
					this._balloons[Random.Range(0, this._balloons.Count-1)].Pop();
				}
			}
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

		private float halfTargetHeight = 768f * 0.5f;
		private float laneHeight = 768f / 5f;

		public float a = -384;
		public float b = -164;
		public float c = -25;
		public float d = 126;
		public float e = 280;
		public float f = 384;

		private void OnGUI() {
			GUI.Label(new Rect(5f, 5f, 200f, 50f), string.Format("a = " + a));
			GUI.Label(new Rect(5f, 55f, 200f, 50f), string.Format("b = " + b));
			GUI.Label(new Rect(5f, 105f, 200f, 50f), string.Format("c = " + c));
			GUI.Label(new Rect(5f, 155f, 200f, 50f), string.Format("d = " + d));
			GUI.Label(new Rect(5f, 205f, 200f, 50f), string.Format("e = " + e));
			GUI.Label(new Rect(5f, 255f, 200f, 50f), string.Format("f = " + f));
		}

		private void UpdateSliderPosition(int oldCount) {
			int count = this._balloons.Count;
			JointTranslationLimits2D limits = new JointTranslationLimits2D();
			if(count >= 12) {
				limits.max = f;
				limits.min = e;
			} else if(count >= 9) {
				limits.max = e;
				limits.min = d;
			} else if(count >= 6) {
				limits.max = d;
				limits.min = c;
			} else if(count >= 3) {
				limits.max = c;
				limits.min = b;
			} else {
				limits.max = b;
				limits.min = a;
			}
			this._sliderJoint.limits = limits;

			if(this.transform.localPosition.y < limits.min) {
				Debug.Log ("Ship too low");
				//CancelInvoke(Ship.RANDOM_FORCE_METHODNAME);
				//Invoke(Ship.RANDOM_FORCE_METHODNAME, Random.Range(2f, 10f));
				//Vector2 randomForce = GenerateRandomForce(true);
				Vector2 randomForce = new Vector2(0f, 1000f);
				this.rigidbody2D.AddForce(randomForce);
			} else if(this.transform.localPosition.y > limits.max) {
				Debug.Log ("Ship too high");
				//CancelInvoke(Ship.RANDOM_FORCE_METHODNAME);
				//Invoke(Ship.RANDOM_FORCE_METHODNAME, Random.Range(2f, 10f));
				//Vector2 randomForce = GenerateRandomForce(false);
				Vector2 randomForce = new Vector2(0f, -1000f);
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
