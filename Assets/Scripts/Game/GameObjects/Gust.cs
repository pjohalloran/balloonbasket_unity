using System.Collections;

using UnityEngine;

using BalloonBasket.Tech;

namespace BalloonBasket.Game {
    public class Gust : MonoBehaviour {
		public static readonly string PREFAB_NAME = "Gust";

		private static readonly string SMALL_ANIM_NAME = "GustSmall";
		private static readonly string LARGE_ANIM_NAME = "GustLarge";

        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private SpriteAnimation _gustAnim;
        [SerializeField] private Vector2 _force = new Vector2(400.0f, 0.0f);

        void Start() {
            int res = Random.Range(0, 2);

            if(res == 1) {
                this._gustAnim.AnimPrefix = Gust.SMALL_ANIM_NAME;
                this._gustAnim.FrameCount = 12;
            } else {
                this._gustAnim.AnimPrefix = Gust.LARGE_ANIM_NAME;
                this._gustAnim.FrameCount = 12;
                this._force = this._force * 2.0f;
            }

            this._gustAnim.Play();
        }

        void Update() {
        }

        void OnTriggerEnter2D(Collider2D other) {
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
