using System.Collections;

using UnityEngine;


namespace BalloonBasket {
    public class Gust : MonoBehaviour {
        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private SpriteAnimation _gustAnim;

        void Start() {
            this._gustAnim.Play();
        }

        void Update() {
        }
    }
}
