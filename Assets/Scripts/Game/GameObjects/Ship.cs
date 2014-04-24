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
			SetLimitsFromBalloonCount(this._startBalloonCount);
			this._lastCount = this._startBalloonCount;

			this.BobbingRange = new Vector2(500f, 750f);

			//Invoke(Ship.RANDOM_FORCE_METHODNAME, Random.Range(1f, 10f));
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

		private bool IsTouching() {
			if(Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor) {
				return Input.GetMouseButtonUp(0);
			}
			return Input.touchCount > 0;
		}

		private Vector2 GetTouchPosition() {
			if(Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor) {
				return new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			} else {
				return Input.touches[0].position;
			}
		}

		private int _lastCount = 0;
		private Vector2 _lastTouchPosition = Vector2.zero;
		private bool _lastWasTouching = false;
        void FixedUpdate() {
			if(!IsTouching()) {
				this._lastTouchPosition = Vector2.zero;
				this._lastWasTouching = false;
			} else {
				if(!this._lastWasTouching) {
					this._lastWasTouching = true;
					this._lastTouchPosition = GetTouchPosition();
				} else {
					Vector2 touchPosition = this._lastTouchPosition;

					if(this._lastTouchPosition.y < touchPosition.y + 100f) {
						if(!this._balloonChangeInProgress && this._balloons.Count < this._maxBalloonCount) {
							this._balloonChangeInProgress = true;
							this._balloons.Add(InstantiateBalloon(this.transform.localPosition + new Vector3(0.0f, 150f, 0.0f)));
						}
					}

					this._lastTouchPosition = touchPosition;
				}
			}

			this._buttons.PollState();

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

			UpdateSliderPosition(this._balloons.Count, this._lastCount);
			this._lastCount = this._balloons.Count;
			Debug.LogError(string.Format("LC ={0}, C={1}", this._lastCount, this._balloons.Count));
        }

		private float halfTargetHeight = 768f * 0.5f;
		private float laneHeight = 768f / 5f;

		public float a = -384f;
		public LineRenderer _aLine;
		public float b = -174f;
		public LineRenderer _bLine;
		public float c = -9f;
		public LineRenderer _cLine;
		public float d = 138f;
		public LineRenderer _dLine;
		public float e = 291f;
		public LineRenderer _eLine;
		public float f = 384f;
		public LineRenderer _fLine;

		/*
		private void OnGUI() {

			GUI.Label(new Rect(5f, 5f, 200f, 50f), string.Format("a = " + a));
			SetupLine(a, _aLine);
			GUI.Label(new Rect(5f, 55f, 200f, 50f), string.Format("b = " + b));
			SetupLine(b, _bLine);
			GUI.Label(new Rect(5f, 105f, 200f, 50f), string.Format("c = " + c));
			SetupLine(c, _cLine);
			GUI.Label(new Rect(5f, 155f, 200f, 50f), string.Format("d = " + d));
			SetupLine(d, _dLine);
			GUI.Label(new Rect(5f, 205f, 200f, 50f), string.Format("e = " + e));
			SetupLine(e, _eLine);
			GUI.Label(new Rect(5f, 255f, 200f, 50f), string.Format("f = " + f));
			SetupLine(f, _fLine);

			if(GUI.Button(new Rect(5f, 300f, 100f, 100f), "UP")) {
				this.rigidbody2D.AddForce(Vector2.up * 5000f);
			}
			if(GUI.Button(new Rect(5f, 400f, 100f, 100f), "DOWN")) {
				this.rigidbody2D.AddForce(-Vector2.up * 5000f);
			}
		}
		*/

		private void SetupLine(float value, LineRenderer line) {
			line.SetPosition(0, new Vector3(-500f, value, -10f));
			line.SetPosition(1, new Vector3(500f, value, -10f));
			line.SetColors(Color.green, Color.green);
			line.SetWidth(10f, 10f);
		}

		private void SetLimitsFromBalloonCount(int count) {
			JointTranslationLimits2D limits = this._sliderJoint.limits;
			this._sliderJoint.useLimits = true;

			if(count >= 12) {
				Debug.LogError ("Lane 5");
				limits.max = f;
				limits.min = e;
			} else if(count >= 9 && count < 12) {
				Debug.LogError ("Lane 4");
				limits.max = e;
				limits.min = d;
			} else if(count >= 6 && count < 9) {
				Debug.LogError ("Lane 3");
				limits.max = d;
				limits.min = c;
			} else if(count >= 3 && count < 6) {
				Debug.LogError ("Lane 2");
				limits.max = c;
				limits.min = b;
			} else if(count < 3) {
				Debug.LogError ("Lane 1");
				limits.max = b;
				limits.min = a;
			} else {
				Debug.LogError("COUNT = " + count);
			}

			this._sliderJoint.limits = limits;
		}

		private Vector2 _correctForce = new Vector2(0f, 10000f);
		private void UpdateSliderPosition(int newCount, int oldCount) {
			if(newCount != oldCount) {
				SetLimitsFromBalloonCount(newCount);
			}

			if(this.transform.localPosition.y < this._sliderJoint.limits.min) {
				Debug.LogError("UPWARD");
				this.rigidbody2D.AddForce(Vector2.up * 5000f);
			} else if(this.transform.localPosition.y > this._sliderJoint.limits.max) {
				Debug.LogError("DOWNWARD");
				this.rigidbody2D.AddForce(-Vector2.up * 5000f);
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
