using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BalloonBasket {
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimation : MonoBehaviour {
        public delegate void OnStart();
        public delegate void OnFinish();
        public delegate void OnFrameChange(int frameIndex);
        public delegate void OnPause();
        public delegate void OnStop();

        public string animPrefix;
        public int frameCount;
        public float duration;

        public OnStart onStart;
        public OnFinish onFinish;
        public OnFrameChange onFrameUpdate;
        public OnPause onPause;
        public OnStop onStop;

        private List<Texture2D> _frames;
        private float _frameTime;
        private float _currTime;
        private int _currIndex;
        private bool _play;
        private bool _pause;
        private SpriteRenderer _sprite;

        void Start () {
            this._frameTime = duration / frameCount;
            this._frames = new List<Texture2D>(this.frameCount);
            for(int i = 0; i < this.frameCount; ++i) {
                this._frames.Add(Utils.LoadResource(string.Format("{0}{1}", this.animPrefix, i+1)) as Texture2D);
            }
            this._sprite = this.gameObject.GetComponent<SpriteRenderer>();

            Reset();
        }

        void Update() {
            if(this._play && !this._pause) {
                this._currTime += Time.deltaTime;
                if(this._currTime > this._currIndex*this._frameTime) {
                    this._currIndex++;
                    if(this._currIndex < this.frameCount) {
                        if(this.onFrameUpdate != null) {
                            this.onFrameUpdate(this._currIndex);
                        }
                        Texture2D t = this._frames[this._currIndex];
                        this._sprite.sprite = Sprite.Create(this._frames[this._currIndex],
                                                            new Rect(0f, 0f, t.width, t.height),
                                                            new Vector2(0.5f, 0.5f));
                    } else {
                        // done
                        this._play = false;
                        if(this.onFinish != null) {
                            this.onFinish();
                        }
                    }
                }
            }
        }

        private void Reset() {
            this._currTime = 0.0f;
            this._currIndex = 0;
            this._play = false;
            this._pause = false;
        }

        public void Play() {
            if(this.onStart != null) {
                this.onStart();
            }
            this._play = true;
            this._sprite.sprite.name = this._frames[this._currIndex].name;
        }

        public void Pause() {
            if(this.onPause != null) {
                this.onPause();
            }
            this._pause = !this._pause;
        }

        public void Stop() {
            if(this.onStop != null) {
                this.onStop();
            }
            Reset();
        }
    }
}
