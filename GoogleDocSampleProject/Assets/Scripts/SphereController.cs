using System;
using UnityEngine;

namespace StansAssets.GoogleDoc.Example
{
    public class SphereController : MonoBehaviour
    {
        [SerializeField] Rigidbody m_rigidbody;

        public event Action Release = delegate { };
        const float DELAY = 5;
        float m_time;
        bool m_release;

        private Vector3 m_initialVelocity;

        private void Awake() {
            m_initialVelocity = m_rigidbody.velocity;
        }

        void OnEnable() {
            m_time = 0;
            m_release = false;
        }

        void Update() {
            m_time += Time.deltaTime;
            if (m_time > DELAY && !m_release)
            {
                m_release = true;
                m_rigidbody.velocity = m_initialVelocity;
                Release.Invoke();
            }
        }
    }
}