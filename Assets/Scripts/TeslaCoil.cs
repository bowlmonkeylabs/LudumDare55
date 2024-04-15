using System.Collections;
using System.Collections.Generic;
using BML.ScriptableObjectCore.Scripts.Variables;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

namespace BML.Scripts {
    public class TeslaCoil : MonoBehaviour
    {
        [SerializeField] private Collider _triggerCollider;
        [SerializeField] private TimerReference _cooldownTimer;
        [SerializeField] private TimerReference _timer;
        [SerializeField] private DamageOnCollision _damageOnCollision;
        [SerializeField] private LayerMask _playerMask;
        [SerializeField] private MMF_Player _electricFeedbacks;

        // private void OnEnable() {
        //     _timer.SubscribeFinished(OnTimerFinish);
        //     _cooldownTimer.SubscribeFinished(OnCooldownFinish);
        // }

        // private void OnDisable() {
        //     _timer.UnsubscribeFinished(OnTimerFinish);
        //     _cooldownTimer.UnsubscribeFinished(OnCooldownFinish);
        // }

        private void Start() {
            _timer.RestartTimer();
            ToggleTrigger(true);
        }

        void Update() {
            _timer.UpdateTime();
            _cooldownTimer.UpdateTime();
            
            if(_timer.IsFinished && !_cooldownTimer.IsStarted) {
                _timer.ResetTimer();
                ToggleTrigger(false);
                _cooldownTimer.StartTimer();
            }

            if(_cooldownTimer.IsFinished && !_timer.IsStarted) {
                _cooldownTimer.ResetTimer();
                _timer.StartTimer();
                ToggleTrigger(true);
            }
        }

        protected void OnTriggerEnter(Collider other)
        {
            
            if(_playerMask.MMContains(other.gameObject)) {
              this._damageOnCollision.AttemptDamage(other);
            }
        }

        void ToggleTrigger(bool enabled) {
            _triggerCollider.enabled = enabled;
            if(enabled) {
                _electricFeedbacks.PlayFeedbacks();
            } else {
                _electricFeedbacks.ResetFeedbacks();
            }
        }
    }
}

