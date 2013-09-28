using UnityEngine;
using System.Collections;

namespace BalloonBasket {
    public class Gull : MonoBehaviour {
        public delegate void OnDeath(Gull gull);
        
        [SerializeField] private SpriteAnimation _dieAnim;
        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private Texture2D _defaultImage;
        [SerializeField] private AudioClip _dieFx;
        
        public OnDeath onDeath;
        
        public void Die() {
            this.audio.PlayOneShot(this._dieFx);
            this._dieAnim.onFinish = this.OnDieDone;
            this._dieAnim.Play();
        }
        
        private void OnDieDone() {
            if(this.onDeath != null) {
                this.onDeath(this);
            }
            this._sprite.color = Color.clear;
            this.rigidbody2D.Sleep();
            this._dieAnim.Stop();
            Destroy(this.gameObject);
        }
        
        void OnCollision2DEnter(Collision2D collision) {
            if(collision.rigidbody != null && 
               (collision.rigidbody.gameObject.GetComponent<Ship>() != null ||
                collision.rigidbody.gameObject.GetComponent<Mine>() != null)) {
                Die();
            }
        }
    }
}