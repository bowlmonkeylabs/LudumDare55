using System.Collections;
using System.Collections.Generic;
using BML.ScriptableObjectCore.Scripts.Variables;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

namespace BML.Scripts {
    public class TeslaCoil : MonoBehaviour
    {
        [SerializeField] private TimerReference _cooldownTimer;
        [SerializeField] private TimerReference _timer;
        [SerializeField] private float _preDelay;
        [SerializeField] private LayerMask _playerMask;
        [SerializeField] private MMF_Player _electricFeedbacks;
        [SerializeField] private MMF_Player _shockFeedbacks;
        [SerializeField] private int _damage = 1;
        [SerializeField] private DamageType _damageType;

        private bool isStarted;
        private bool isEnabled;
        private bool isCleaned;
        private float spawnTime;

        // private void OnEnable() {
        //     _timer.SubscribeFinished(OnTimerFinish);
        //     _cooldownTimer.SubscribeFinished(OnCooldownFinish);
        // }

        // private void OnDisable() {
        //     _timer.UnsubscribeFinished(OnTimerFinish);
        //     _cooldownTimer.UnsubscribeFinished(OnCooldownFinish);
        // }

        private void Start() {
            spawnTime = Time.time;
        }

        void Update()
        {
            if (isCleaned) return;

            // Start after pre-delay
            if (!isStarted && spawnTime + _preDelay < Time.time) {
                isStarted = true;
                _timer.StartTimer();
                ToggleActive(true);
            }
            
            if (!isStarted) return;
            
            _timer.UpdateTime();
            _cooldownTimer.UpdateTime();
            
            if(_timer.IsFinished && !_cooldownTimer.IsStarted) {
                _timer.ResetTimer();
                ToggleActive(false);
                _cooldownTimer.StartTimer();
            }

            if(_cooldownTimer.IsFinished && !_timer.IsStarted) {
                _timer.StartTimer();
                ToggleActive(true);
                _cooldownTimer.ResetTimer();
            }
        }

        public void TryShock()
        {
            if (!isEnabled) return;
            
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 999, _playerMask);
            foreach (var hitCollider in hitColliders)
            {
                hitCollider.GetComponent<Damageable>()?.TakeDamage(new HitInfo(_damageType, _damage, Vector3.zero));
                _shockFeedbacks.PlayFeedbacks();
            }
        }

        public void OnClean()
        {
            isCleaned = true;
            isEnabled = false;
            _electricFeedbacks.StopFeedbacks();
        }

        void ToggleActive(bool newState) {
            isEnabled = newState;
            if(isEnabled) {
                _electricFeedbacks.PlayFeedbacks();
            } else {
                _electricFeedbacks.StopFeedbacks();
            }
        }
    }
}

