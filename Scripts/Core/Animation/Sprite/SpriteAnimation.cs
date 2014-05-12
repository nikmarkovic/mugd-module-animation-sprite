using System;
using UnityEngine;

namespace Assets.Scripts.Core.Animation.Sprite
{
    [Serializable]
    public class SpriteAnimation
    {
        [SerializeField]
        private Texture _texture;
        [SerializeField]
        private int _frames;
        [SerializeField]
        private int _angles;
        [SerializeField]
        private int _fps;
        private int _currentFrame;
        private bool _loop;
        private Func<float> _angle;
        private Action _onAnimationEnd;

        public bool Loop
        {
            get { return _loop; }
            set { _loop = value; }
        }

        public int CurrentFrame
        {
            get { return _currentFrame; }
            set { _currentFrame = value; }
        }

        public Texture Texture
        {
            get { return _texture; }
        }

        public string Name
        {
            get { return _texture.name; }
        }

        public int Frames
        {
            get { return _frames; }
        }

        public int Angles
        {
            get { return _angles; }
        }

        public Vector2 Scale
        {
            get { return new Vector2(1f/_frames, 1f/_angles); }
        }

        public int Fps
        {
            get { return _fps; }
        }

        public float Spf
        {
            get { return 1f/_fps; }
        }

        public void SetAngleFunction(Func<float> angle)
        {
            _angle = angle;
        }

        public void SetOnAnimationEnd(Action onAnimationEnd)
        {
            _onAnimationEnd = onAnimationEnd;
        }

        public Vector2 GetNextFrame()
        {
            _currentFrame = (_currentFrame + 1)%_frames;
            var x = (float) _currentFrame/_frames;
            var y = 1 - 1f/_angles - GetAngle()/360;
            return new Vector2(x, y);
        }

        public void OnAnimationEnd()
        {
            if (_onAnimationEnd == null) return;
            _onAnimationEnd();
        }

        private float GetAngle()
        {
            if (_angle == null) return 0;
            var angle = (Mathf.Round((int) _angle()/GetStep())*GetStep())%360;
            return angle < 0 ? angle + 360 : angle;
        }

        private float GetStep()
        {
            return 360f/_angles;
        }
    }
}