using UnityEngine;
using System.Collections;

namespace BalloonBasket {
    public class Mine : MonoBehaviour {
        public float force = 10.0f;
        public SpriteAnimation _explodeAnim;
        public SpriteRenderer _sprite;

        private bool _up = true;

        void Start() {
            InvokeRepeating("SwitchGravity", 0.0f, 0.5f);
        }

        void OnCollision2DEnter(Collision2D collision) {
            if (collision.rigidbody != null && collision.rigidbody.gameObject.GetComponent<Ship>() != null) {
                Explode();

                Vector3 diff = this.transform.localPosition - collision.rigidbody.transform.localPosition;
                diff.Normalize();
                collision.rigidbody.AddForce(-diff * 150.0f);
            }
        }

        private void SwitchGravity() {
            Vector3 vec = new Vector3(0.0f, (this._up ? force : -force), 0.0f);
            this.rigidbody2D.AddForce(vec);
            this._up = !this._up;
        }

        private void Explode() {
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
