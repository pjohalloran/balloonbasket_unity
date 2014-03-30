using System;
using System.Collections.Generic;

using UnityEngine;

namespace BalloonBasket.Tech {
	public interface PoolObject {
		void Init();
		void Destroy();
	}

	public class GameObjectPool : MonoBehaviour {
		[SerializeField] private int _size = 10;
		[SerializeField] private string _resource;

		private List<GameObject> _pool;

		public string ResourceId {
			get {
				return this._resource;
			} set {
				this._resource = value;
			}
		}

		public int Size {
			get {
				return this._size;
			} set {
				this._size = value;
			}
		}

		private void AddToPool(GameObject go) {
			go.transform.parent = this.transform;
			Utils.ResetTransform(go.transform);
			go.SetActive(false);
		}

		private GameObject GetFromPool() {
			GameObject go = null;
			for(int i = 0; (go == null) && (i < this._pool.Count); ++i) {
				if(!this._pool[i].activeSelf) {
					go = this._pool[i];
					go.SetActive(true);
					//go.transform.parent = null;
					Utils.ResetTransform(go.transform);
				}
			}
			return go;
		}

		private PoolObject GetPoolObject(GameObject go) {
			return go.GetComponent(typeof(PoolObject)) as PoolObject;
		}

		public GameObject Instantiate() {
			GameObject go = GetFromPool();
			if(go == null) {
				Debug.LogError(string.Format("No more objects available from {0} pool", this.name));
			} else {
				GetPoolObject(go).Init();
			}
			return go;
		}

		public void Destroy(GameObject go) {
			if(go == null || !this._pool.Contains(go)) {
				Debug.LogError(string.Format("Cannot return go not managed by {0} pool", this.name));
				return;
			}
			GetPoolObject(go).Destroy();
			AddToPool(go);
		}

		public void CreatePool() {
			DestroyPool();

			this._pool = new List<GameObject>(this._size);

			GameObject go = Utils.LoadResource(this._resource) as GameObject;
			if(GetPoolObject(go) == null) {
				Debug.LogError(string.Format("Cannot use {0} in pool {1} as it doesn't impleemnted PoolObject interface!"));
				go = null;
				return;
			}
			
			for(int i = 0; i < this._size; ++i) {
				GameObject copy = GameObject.Instantiate(go) as GameObject;
				this._pool.Add(copy);
				AddToPool(copy);
			}

			go = null;
		}

		public void DestroyPool() {
			if(this._pool == null || this._pool.Count == 0) {
				return;
			}

			for(int i = 0; i < this._pool.Count; ++i) {
				GameObject.Destroy(this._pool[i]);
				this._pool[i] = null;
			}
			this._pool.Clear();
			this._pool = null;
		}
	}
}
