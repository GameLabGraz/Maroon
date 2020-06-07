using System;
using UnityEngine;

namespace DissolveShader
{
    [RequireComponent(typeof(MeshRenderer))]
    public class DissolveHandler : MonoBehaviour
    {
        private enum State
        {
            Appear,
            Disappear,
            Pause
        }
        
        // [Range(0f, 1f)]
        // public float dissolveFactor;
        [Range(0f, 1f)]
        public float glowTimeInPercent = 0.05f;
        public float _glowRange = 0f;
        public float _glowFalloff = 0.2f;
        
        
        private Renderer _renderer;
        private float _currentDissolveFactor;
        private static readonly int GlowRange = Shader.PropertyToID("_GlowRange");
        private static readonly int GlowFalloff = Shader.PropertyToID("_GlowFalloff");
        private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");

        private float _totalTime;
        private float _restTime;
        private State _currentState = State.Pause;

        // Start is called before the first frame update
        void Start()
        {
            _renderer = GetComponent<Renderer>();
      
            _renderer.material.SetFloat(GlowRange, 0f);
            _renderer.material.SetFloat(GlowFalloff, 0.001f);
            _renderer.material.SetFloat(DissolveAmount, 0f);

            _currentDissolveFactor = 0f;
        }

        // Update is called once per frame
        void Update()
        {
            if (_currentState == State.Pause) return;
            
            if (_restTime > 0f)
            {
                if (_currentState == State.Appear)
                    Dissolve(_restTime/_totalTime);
                else
                    Dissolve(1f - (_restTime/_totalTime));

                _restTime -= Time.deltaTime;
            }

            if (_restTime <= 0f)
            {
                Dissolve(_currentState == State.Appear? 0f : 1f); //Just to make sure
                _currentState = State.Pause;
            }
            
            // if (Math.Abs(_currentDissolveFactor - dissolveFactor) > 0.0001)
            // {
            //     Dissolve(dissolveFactor);
            //     _currentDissolveFactor = dissolveFactor;
            // }
        }

        private void Dissolve(float newFactor)
        {
            if (newFactor <= glowTimeInPercent)
            {
                _renderer.material.SetFloat(GlowRange, _glowRange * (newFactor / glowTimeInPercent));
                _renderer.material.SetFloat(GlowFalloff, 0.001f + (_glowFalloff - 0.001f) * (newFactor / glowTimeInPercent));
                _renderer.material.SetFloat(DissolveAmount, 0f);
            }
            else
            {
                _renderer.material.SetFloat(GlowRange, _glowRange);
                _renderer.material.SetFloat(GlowFalloff, _glowFalloff);
                _renderer.material.SetFloat(DissolveAmount, (newFactor - glowTimeInPercent) / (1f - glowTimeInPercent));
            }

            _currentDissolveFactor = Mathf.Clamp(newFactor, 0f, 1f);
        }

        public void StartDissolving(float time, bool isAppearing)
        {
            _currentState = isAppearing ? State.Appear : State.Disappear;
            _totalTime = _restTime = time;
        }
    }
     
}
