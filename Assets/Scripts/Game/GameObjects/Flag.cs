using UnityEngine;
using System.Collections;

namespace BalloonBasket.Game {
    public class Flag : MonoBehaviour {
		public static readonly string PREFAB_NAME = "Flag";

		[SerializeField] private AudioClip _finishLine;

		public delegate void OnShipEnter();
		public OnShipEnter onShipEnter;

		// TODO Cannot get trigger enter when other object is kinematic so workround using collide enter
		private void OnTriggerEnter2D(Collider2D other) {
			if (other.gameObject.GetComponent<Ship> () != null) {
				if(this.onShipEnter != null) {
					this.onShipEnter();
				}
				this.audio.PlayOneShot(this._finishLine);
			}
		}

		void OnCollisionEnter2D(Collision2D collision) {
			if (collision.gameObject.GetComponent<Ship> () != null) {
				if(this.onShipEnter != null) {
					this.onShipEnter();
				}
				this.audio.PlayOneShot(this._finishLine);
			}
		}


    }
}