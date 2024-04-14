using System;
using System.Collections;
using System.Collections.Generic;
using BML.ScriptableObjectCore.Scripts.Events;
using UnityEngine;
using UnityEngine.Events;

namespace BML.Scripts {
    public class Cleanable : MonoBehaviour
    {
        [SerializeField] private Health _sprayHealth;
        [SerializeField] private Health _vacuumHealth;
        [SerializeField] private ParticleSystem _sprayDirtParticles;
        [SerializeField] private ParticleSystem _vacuumDirtParticles;
        [SerializeField] private GameEvent _onCleanedGameEvent;
        [SerializeField] private UnityEvent _onCleaned;

        public bool IsCleaned => isCleaned;
        private bool isCleaned;

        private void Start()
        {
            UpdateParticles();
        }

        public void CheckCleaned()
        {
            if (isCleaned) return;
            
            UpdateParticles();
            if (_sprayHealth.IsDead && _vacuumHealth.IsDead)
            {
                OnCleaned();
            }
        }

        private void UpdateParticles()
        {
            if (_sprayHealth.Value > 0)
            {
                _sprayDirtParticles.Play();
                
                _vacuumDirtParticles.Clear();
                _vacuumDirtParticles.Stop();
            }
            else if (_vacuumHealth.Value > 0)
            {
                _sprayDirtParticles.Clear();
                _sprayDirtParticles.Stop();
                
                _vacuumDirtParticles.Play();
            }
            else
            {
                _sprayDirtParticles.Clear();
                _sprayDirtParticles.Stop();
                
                _vacuumDirtParticles.Clear();
                _vacuumDirtParticles.Stop();
            }
        }

        public void OnCleaned()
        {
            isCleaned = true;
            _onCleanedGameEvent.Raise();
            _onCleaned.Invoke();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}
