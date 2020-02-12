using System;
using UnityEngine;


namespace UnityStandardAssets.Effects
{
    public class Hose : MonoBehaviour
    {
        public float maxPower = 20;
        public float minPower = 5;
        public float changeSpeed = 5;
        public ParticleSystem[] hoseWaterSystems;
        public Renderer systemRenderer;

        private float m_Power;


        // Update is called once per frame
        private void Update()
        {
            m_Power = Mathf.Lerp(m_Power, Input.GetMouseButton(0) ? maxPower : minPower, Time.deltaTime*changeSpeed);

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                systemRenderer.enabled = !systemRenderer.enabled;
            }

            foreach (var system in hoseWaterSystems)
            {
                system.startSpeed = m_Power;
                system.enableEmission = (m_Power > minPower*1.1f);
            }
        }
    }
}
