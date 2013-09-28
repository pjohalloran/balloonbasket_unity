using UnityEngine;
using System.Collections;

namespace BalloonBasket {
    public class Mine : MonoBehaviour {
        public delegate void OnExplodeShip();

        [SerializeField] private float _force = 10.0f;
        [SerializeField] public SpriteAnimation _explodeAnim;
        [SerializeField] private SpriteRenderer _sprite;

        public OnExplodeShip onExplodeShip;

        private bool _up = true;

        void Start() {
        }

        void OnCollision2DEnter(Collision2D collision) {
            if (collision.rigidbody != null) {
                Explode();

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
