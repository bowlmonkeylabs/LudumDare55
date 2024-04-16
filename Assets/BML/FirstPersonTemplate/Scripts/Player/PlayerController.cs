using System;
using System.Collections.Generic;
using BML.ScriptableObjectCore.Scripts.Variables;
using BML.Scripts;
using BML.Scripts.UI;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Micosmo.SensorToolkit;
using MoreMountains.Feedbacks;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("References")] protected Transform _mainCamera;
        [SerializeField, FoldoutGroup("References")] protected BoolReference _isPlayerInputDisabled;

        [SerializeField, FoldoutGroup("Interact")] protected float _interactDistance = 5f;
        [SerializeField, FoldoutGroup("Interact")] protected float _interactCastRadius = .25f;
        [SerializeField, FoldoutGroup("Interact")] protected LayerMask _interactMask;
        [SerializeField, FoldoutGroup("Interact")] protected TMP_Text _hoverText;
        [SerializeField, FoldoutGroup("Interact")] protected float _hoverUpdateDelay = .1f;
        
        [SerializeField, FoldoutGroup("Vacuum")] protected LOSSensor _vacuumSensor;
        [SerializeField, FoldoutGroup("Vacuum")] protected int _vacuumDamage = 1;
        [SerializeField, FoldoutGroup("Vacuum")] protected float _vacuumHitDelay = .2f;
        [SerializeField, FoldoutGroup("Vacuum")] protected MMF_Player _startVacuumFeedbacks;
        [SerializeField, FoldoutGroup("Vacuum")] protected MMF_Player _stopVacuumFeedbacks;
        [SerializeField, FoldoutGroup("Vacuum")] protected MMF_Player _startVacuumDirtFeedbacks;
        [SerializeField, FoldoutGroup("Vacuum")] protected MMF_Player _stopVacuumDirtFeedbacks;
        
        [SerializeField, FoldoutGroup("Spray")] protected LOSSensor _spraySensor;
        [SerializeField, FoldoutGroup("Spray")] protected int _sprayDamage = 1;
        [SerializeField, FoldoutGroup("Spray")] protected MMF_Player _sprayFeedbacks;


        [SerializeField, FoldoutGroup("Caffeine")] protected BoolReference _isCaffeineUnlocked; 
        [SerializeField, FoldoutGroup("Caffeine")] protected BoolReference _isCaffeinated;
        [SerializeField, FoldoutGroup("Caffeine")] protected TimerReference _caffeineTimer;
        [SerializeField, FoldoutGroup("Caffeine")] protected TimerReference _caffeineCooldownTimer;
        [SerializeField, FoldoutGroup("Caffeine")] protected BoolReference _outputShowCaffeineAvailable;

        private float lastHoverUpdateTime = Mathf.NegativeInfinity;
        private float lastVacuumTime = Mathf.NegativeInfinity;
        private bool vacuuming;
        
        #region Unity Lifecycle

        protected virtual void OnEnable()
        {
            UpdateCaffeineIndicator();
            _caffeineTimer.SubscribeFinished(DisableCaffeine);
            _caffeineCooldownTimer.SubscribeFinished(UpdateCaffeineIndicator);
        }

        protected virtual void OnDisable()
        {
            _caffeineTimer.UnsubscribeFinished(DisableCaffeine);
            _caffeineCooldownTimer.UnsubscribeFinished(UpdateCaffeineIndicator);
        }

        protected void Update()
        {
            if (lastHoverUpdateTime + _hoverUpdateDelay < Time.time)
            {
                CheckHover();
            }

            if (vacuuming) TryVacuum();
        }

        protected virtual void FixedUpdate()
        {
            if (_caffeineTimer != null)
            {
                _caffeineTimer.UpdateTime();
                _caffeineCooldownTimer.UpdateTime();
            }
        }

        #endregion

        #region Input Callback

        protected virtual void OnPrimary(InputValue value)
        {
            vacuuming = !_isPlayerInputDisabled.Value && value.isPressed;
            
            if (vacuuming) 
                _startVacuumFeedbacks.PlayFeedbacks();
            else
            {
                _stopVacuumFeedbacks.PlayFeedbacks();
                _stopVacuumDirtFeedbacks.PlayFeedbacks();
            }
            
            //Debug.Log($"OnPrimary: {value.isPressed}");
        }
        
        protected virtual void OnSecondary(InputValue value)
        {
            if (_isPlayerInputDisabled.Value) return;
            if (!value.isPressed) return;
            
            TrySpray();
        }
        
        protected virtual void OnInteract(InputValue value)
        {
            if (_isPlayerInputDisabled.Value) return;
            if (!value.isPressed) return;
            
            TryInteract();
        }

        protected virtual void OnUseCaffeine(InputValue value)
        {
            if (_isPlayerInputDisabled.Value) return;
            if (!value.isPressed) return;
            
            TryUseCaffeine();
        }

        #endregion

        #region Interact

        protected virtual void TryInteract()
        {
            RaycastHit hit;
            
            if (Physics.SphereCast(_mainCamera.position, _interactCastRadius, _mainCamera.forward, out hit,
                _interactDistance, _interactMask, QueryTriggerInteraction.Ignore))
            {
                InteractionReceiver interactionReceiver = hit.collider.GetComponent<InteractionReceiver>();
                if (interactionReceiver == null)
                    return;

                interactionReceiver.ReceiveInteraction();
            }
        }
        
        private void CheckHover()
        {
            if (Physics.Raycast(_mainCamera.position, _mainCamera.forward, out var hit, _interactDistance, _interactMask, QueryTriggerInteraction.Ignore))
            {
                InteractionReceiver interactionReceiver = hit.collider.GetComponent<InteractionReceiver>();
                if (interactionReceiver == null) return;

                _hoverText.text = interactionReceiver.HoverText;
            }
            else
            {
                _hoverText.text = "";
            }

            lastHoverUpdateTime = Time.time;
        }

        #endregion
        
        #region Vacuum

        private List<Damageable>  vacuumDetections = new List<Damageable>();
        private void TryVacuum()
        {
            if (Time.time - lastVacuumTime < _vacuumHitDelay) return;
            
            lastVacuumTime = Time.time;
            _vacuumSensor.PulseAll();
            _vacuumSensor.GetDetectedComponents(vacuumDetections);
            bool successfulVacuum = false;
            vacuumDetections.ForEach(d =>
                {
                    successfulVacuum = d.TakeDamage(new HitInfo
                    (DamageType.Vacuum, _vacuumDamage,
                        (d.transform.position - transform.position).normalized));
                    
                } 
            );

            if (successfulVacuum)
                _startVacuumDirtFeedbacks.PlayFeedbacks();
            else
                _stopVacuumDirtFeedbacks.PlayFeedbacks();
        }

        #endregion

        #region Spray

        private List<Damageable>  sprayDetections = new List<Damageable>();
        private void TrySpray()
        {
            _spraySensor.PulseAll();
            _spraySensor.GetDetectedComponents(sprayDetections);
            sprayDetections.ForEach(d => d.TakeDamage(new HitInfo
                (DamageType.Spray, _sprayDamage,
                    (d.transform.position - transform.position).normalized))
            );
            _sprayFeedbacks.PlayFeedbacks();
        }

        #endregion

        #region Caffeine

        protected virtual void TryUseCaffeine()
        {
            if (_isCaffeineUnlocked.Value
                && !_isCaffeinated.Value 
                && (!_caffeineTimer.IsStarted || _caffeineTimer.IsFinished)
                && (!_caffeineCooldownTimer.IsStarted || _caffeineCooldownTimer.IsFinished))
            {
                _isCaffeinated.Value = true;
                
                _caffeineTimer.RestartTimer();
            }

            UpdateCaffeineIndicator();
        }

        protected virtual void DisableCaffeine()
        {
            _isCaffeinated.Value = false;

            _caffeineTimer.ResetTimer();
            _caffeineCooldownTimer.RestartTimer();
            
            UpdateCaffeineIndicator();
        }

        protected virtual void UpdateCaffeineIndicator()
        {
            _outputShowCaffeineAvailable.Value =
                _isCaffeinated.Value || (
                    (!_caffeineCooldownTimer.IsStarted || _caffeineCooldownTimer.IsFinished) 
                    && (!_caffeineTimer.IsStarted || _caffeineTimer.IsFinished));
        }

        #endregion

    }
}