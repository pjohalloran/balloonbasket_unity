using UnityEngine;
using System.Collections;

namespace BalloonBasket {
    public class Ship : MonoBehaviour {
        public SpriteRenderer ship;
        public BalloonBasketMain main;

        void Start() {
        
        }

        void Update() {
            Vector3 shipOffset = Vector2.zero;
            if(Input.GetKeyDown(KeyCode.A)) {
                shipOffset.x -= 100f;
            }
            if(Input.GetKeyDown(KeyCode.D)) {
                shipOffset.x += 100f;
            }
            if(Input.GetKeyDown(KeyCode.W)) {
                shipOffset.y += 100f;
            }
            if(Input.GetKeyDown(KeyCode.S)) {
                shipOffset.y -= 100f;
            }
            
            this.ship.GetComponent<Rigidbody2D>().AddForce(shipOffset * Time.deltaTime);
        }
    }
}
