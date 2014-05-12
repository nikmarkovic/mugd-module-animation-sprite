using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Core.Animation.Sprite
{
    [AddComponentMenu("Mugd/Animation/Sprite/Animator")]
    [RequireComponent(typeof (MeshRenderer))]
    public class SpriteAnimator : MonoBehaviour
    {
        [SerializeField] 
        private SpriteAnimation[] _playlist;
        private readonly LinkedList<SpriteAnimation> _queue = new LinkedList<SpriteAnimation>();
        private float _lastFrameTime;

        public SpriteAnimation Current { get; private set; }

        private void Awake()
        {
            renderer.sharedMaterial = new Material(renderer.sharedMaterial);
        }

        private void Update()
        {
            if (Current == null && _queue.Count > 0)
            {
                Play(_queue.First());
                _queue.RemoveFirst();
            }
            if (Current == null || !(Time.time - _lastFrameTime >= Current.Spf)) return;
            renderer.sharedMaterial.SetTextureOffset("_MainTex", Current.GetNextFrame());
            _lastFrameTime = Time.time;
            if (Current.CurrentFrame != Current.Frames - 1) return;
            Current.OnAnimationEnd();
            if (!Current.Loop) Stop();
        }

        public SpriteAnimation PlayForce(string animationName, bool loop)
        {
            var newAnimation = FindAnimation(animationName);
            if (newAnimation == null) return null;
            newAnimation.CurrentFrame = -1;
            newAnimation.Loop = loop;
            _queue.AddFirst(newAnimation);
            Stop();
            return newAnimation;
        }

        public SpriteAnimation Play(string animationName, bool loop)
        {
            var newAnimation = FindAnimation(animationName);
            if (newAnimation == null) return null;
            newAnimation.CurrentFrame = -1;
            newAnimation.Loop = loop;
            _queue.AddLast(newAnimation);
            return newAnimation;
        }

        public void StopForce()
        {
            _queue.Clear();
            Current = null;
        }

        public void Stop()
        {
            Current = null;
        }

        private void Play(SpriteAnimation newAnimation)
        {
            Current = newAnimation;
            renderer.sharedMaterial.SetTexture("_MainTex", Current.Texture);
            renderer.sharedMaterial.SetTextureScale("_MainTex", Current.Scale);
            _lastFrameTime = Time.time;
        }

        private SpriteAnimation FindAnimation(string animationName)
        {
            var animations = _playlist.Where(a => a.Name.ToUpper().Contains(animationName.ToUpper())).ToList();
            Random.seed = (int) (Time.time*1000);
            return animations.Any() ? animations[Random.Range(0, animations.Count)] : null;
        }
    }
}