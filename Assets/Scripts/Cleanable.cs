using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BML.Scripts {
    public class Cleanable : MonoBehaviour
    {
        [SerializeField] private Health _sprayHealth;
        [SerializeField] private Health _vacuumHealth;
        [SerializeField] private ParticleSystem _sprayDirtParticles;
        [SerializeField] private ParticleSystem _vacuumDirtParticles;

        private void Start()
        {
            UpdateParticles();
        }

        public void UpdateParticles()
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
    }
}
