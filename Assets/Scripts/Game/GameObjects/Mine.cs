using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using BalloonBasket.Tech;

namespace BalloonBasket.Game {
    public class Mine : MonoBehaviour {
		public static readonly string PREFAB_NAME = "Mine";

        public delegate void OnExplodeShip();

        [SerializeField] private float _force = 10.0f;
        [SerializeField] public SpriteAnimation _explodeAnim;
        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private List<AudioClip> _explodeFx;

        public OnExplodeShip onExplodeShip;

        private bool _up = true;

        void Start() {
        }

		void OnCollisionEnter2D(Collision2D collision) {
            if (collision.rigidbody != null) {
                Explode();
                this.rigidbody2D.Sleep();

                Vector3 diff = this.transform.localPosition - collision.rigidbody.transform.localPosition;
                diff.Normalize();
                if(collision.rigidbody.gameObject.GetComponent<Ship>() != null) {
                    if(this.onExplodeShip != null) {
                        this.onExplodeShip();
                    }
                    collision.rigidbody.AddForce(-diff * 50.0f);
                }
            }
        }

        private void SwitchGravity() {
            Vector3 vec = new Vector3(0.0f, (this._up ? _force : -_force), 0.0f);
            this.rigidbody2D.AddForce(vec);
            this._up = !this._up;
        }

        public void Explode() {
            this.audio.PlayOneShot(this._explodeFx[Random.Range(0, this._explodeFx.Count-1)]);
            this._explodeAnim.onFinish += this.ExplodeDone;
            this._explodeAnim.Play();
        }

        private void ExplodeDone() {
            this._sprite.color = Color.clear;
            this.rigidbody2D.Sleep();
            Destroy(this.gameObject);
        }
    }
}
