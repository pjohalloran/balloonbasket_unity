using System.Collections;

using UnityEngine;


namespace BalloonBasket {
    public class Gust : MonoBehaviour {
		public static readonly string PREFAB_NAME = "Gust";

        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private SpriteAnimation _gustAnim;
        [SerializeField] private Vector2 _force = new Vector2(10.0f, 0.0f);

        void Start() {
            int res = Random.Range(0, 2);

            if(res == 1) {
                this._gustAnim.AnimPrefix = "GustSmall";
                this._gustAnim.FrameCount = 12;
            } else {
                this._gustAnim.AnimPrefix = "GustLarge";
                this._gustAnim.FrameCount = 12;
                this._force = this._force * 2.0f;
            }

            this._gustAnim.Play();
        }

        void Update() {
        }

        void OnTrigger2DEnter(Collider2D other) {
            GustEvents.TriggerGustEnter();
            other.rigidbody2D.AddForce(this._force);
        }
    }

    public class GustEvents {
        public delegate void OnGustEnter();

        private static OnGustEnter _onGustEnter;

        public static event OnGustEnter OnGustEnterDelegate {
            add {
                GustEvents._onGustEnter += value;
            } remove {
                GustEvents._onGustEnter -= value;
            }
        }

        public static void TriggerGustEnter() {
            if(GustEvents._onGustEnter != null) {
                GustEvents._onGustEnter();
            }
        }
    }
}
