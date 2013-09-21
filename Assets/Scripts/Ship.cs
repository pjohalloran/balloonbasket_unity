using UnityEngine;
using System.Collections;

namespace BalloonBasket {
    public class Ship : MonoBehaviour {
        public SpriteRenderer ship;
        public BalloonBasketMain main;

        public Vector2 force = new Vector2(500f, 500f);

        void Start() {
        
        }

        void Update() {
            Vector3 shipOffset = Vector2.zero;
            if(Input.GetKeyDown(KeyCode.A)) {
                shipOffset.x -= force.x;
            }
            if(Input.GetKeyDown(KeyCode.D)) {
                shipOffset.x += force.x;
            }
            if(Input.GetKeyDown(KeyCode.W)) {
                shipOffset.y += force.y;
            }
            if(Input.GetKeyDown(KeyCode.S)) {
                shipOffset.y -= force.y;
            }
            
            this.ship.GetComponent<Rigidbody2D>().AddForce(shipOffset * Time.deltaTime);
        }
    }
}
