using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace BalloonBasket {
    public class Ship : MonoBehaviour {
        [SerializeField] private SpriteRenderer _ship;
        [SerializeField] public BalloonBasketMain main;
        [SerializeField] private Vector2 force = new Vector2(500f, 500f);
        [SerializeField] private int _balloonCount;

        private List<Balloon> _balloons;

        void Start() {
            this.transform.localPosition = Vector3.zero;

            float x = this.transform.localPosition.x;
            float y = this.transform.localPosition.y;
            Debug.Log (x + "_" + y);
            this._balloons = new List<Balloon>(this._balloonCount);
            for(int i = 0; i < this._balloonCount; ++i) {
                this._balloons.Add(InstantiateBalloon(this.transform.localPosition + new Vector3(0.0f, 0.2f, 0.0f)));
                //this._balloons.Add(InstantiateBalloon(new Vector3(0.0f, 0.1f, 0.0f)));
            }
        }

        private Balloon InstantiateBalloon(Vector3 position) {
            GameObject obj = (GameObject)GameObject.Instantiate(Utils.LoadResource("Balloon") as GameObject);
            Vector3 origScale = obj.transform.localScale;
            obj.transform.parent = main.dynamicRoot;
            Utils.SetTransform(obj.transform, position, origScale);

            Balloon balloon = obj.GetComponent<Balloon>();
            balloon.Ship = this;
            balloon.SetJointDistance(Random.Range(0.4f, 0.6f));
            balloon.onPopped += this.OnBalloonPopped;
            return balloon;
        }

        private void OnBalloonPopped(Balloon balloon) {
            this._balloons.Remove(balloon);
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
            
            this.rigidbody2D.AddForce(shipOffset * Time.deltaTime);

            if(Input.GetKeyDown(KeyCode.O)) {
                if(this._balloons.Count < this._balloonCount) {
                    this._balloons.Add(InstantiateBalloon(this.transform.localPosition + new Vector3(0.0f, 0.2f, 0.0f)));
                }
            }
            if(Input.GetKeyDown(KeyCode.P)) {
                if(this._balloons.Count > 0) {
                    this._balloons[this._balloons.Count-1].Pop();
                }
            }
        }
    }
}
