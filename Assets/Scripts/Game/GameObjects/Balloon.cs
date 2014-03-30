using System.Collections;

using UnityEngine;

using BalloonBasket.Tech;

namespace BalloonBasket.Game {
    public class Balloon : MonoBehaviour, PoolObject {
		public static readonly string PREFAB_NAME = "Balloon";

        public delegate void OnPopped(Balloon balloon);
		public delegate void OnPumped(Balloon balloon);

        [SerializeField] private SpriteAnimation _popAnim;
        [SerializeField] private SpriteAnimation _inflateAnim;
        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private Texture2D _defaultImage;
        [SerializeField] private Color[] _color;
        [SerializeField] private DistanceJoint2D _joint;
        [SerializeField] private AudioClip _inflateCip;
        [SerializeField] private AudioClip _popClip;
        [SerializeField] private LineRenderer _line;
		[SerializeField] private GameObject _lineEndpoint;

        public OnPopped onPopped;
		public OnPumped onPumped;

        private Ship _ship;
        public Ship Ship {
            get {
                return this._ship;
            }
            set {
                this._ship = value;
                if(this._ship != null) {
                    this._joint.connectedBody = this.Ship.rigidbody2D;
                }
            }
        }

		#region PoolObject interface
		public void Init() {
			Inflate();
			this._sprite.color = this._color[Random.Range(0, this._color.Length-1)];
		}

		public void Destroy() {
		}
		#endregion

        void Update() {
			this._line.SetPosition(0, this._lineEndpoint.transform.position);
            this._line.SetPosition(1, this.Ship.GetLineEndpoint().position);
        }

        private void Inflate() {
            this.audio.PlayOneShot(this._inflateCip);
            this._inflateAnim.onFinish = this.OnInflateDone;
            this._inflateAnim.Play();
        }

        private void OnInflateDone() {
            this._inflateAnim.Stop();
            this._sprite.sprite = Sprite.Create(this._defaultImage,
                                         new Rect(0f, 0f, this._defaultImage.width, this._defaultImage.height),
                                         new Vector2(0.5f, 0.5f), 1f);
			if (this.onPumped != null) {
				this.onPumped(this);
			}
        }

        public void Pop() {
            this.audio.PlayOneShot(this._popClip);
            this._joint.connectedBody = null;
            this._popAnim.onFinish = this.OnPopDone;
            this._popAnim.Play();
        }

        private void OnPopDone() {
            this._popAnim.Stop();
			if(this.onPopped != null) {
				this.onPopped(this);
			}
        }

        public void SetJointDistance(float distance) {
            this._joint.distance = distance;
        }
        
		void OnCollisionEnter2D(Collision2D collision) {
            if(collision.rigidbody != null && 
            	(collision.rigidbody.gameObject.GetComponent<Mine>() != null ||
            	collision.rigidbody.gameObject.GetComponent<Gull>() != null)) {
                Pop();
            }
        }
    }
}