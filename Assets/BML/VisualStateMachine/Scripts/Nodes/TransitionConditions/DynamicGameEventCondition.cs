using System.Collections;
using System.Collections.Generic;
using BML.ScriptableObjectCore.Scripts.Events;
using BML.ScriptableObjectCore.Scripts.Variables;
using Sirenix.OdinInspector;
using UnityEngine;
namespace BML.VisualStateMachine.Scripts.Nodes.TransitionConditions
{
    [System.Serializable]
    [HideReferenceObjectPicker]
    public class DynamicGameEventCondition : ITransitionCondition
    {
        [SerializeField] [HideLabel] private DynamicGameEvent targetParameter;
        
        [Tooltip("Keep this dynamic game event condition on after first match")]
        [LabelWidth(100f)] [InfoBox("Dynamic Game Event will keep evaluating true after first receive!",
            InfoMessageType.Warning, "KeepGameEventOn")]
        public bool KeepGameEventOn = false;
    
        private string parentTransitionName = "";
        private bool stayingActive = false;
    
        private bool eventReceived;
    
        public DynamicGameEvent  TargetParameter => targetParameter;
    
    
        public void Init(string transitionName)
        {
            if (targetParameter == null) return;
            parentTransitionName = transitionName;
            targetParameter.Subscribe(ReceiveGameEvent);
        }
    
        //Check if the trigger variable that was activated matches the one for this condition
        public bool Evaluate(List<TriggerVariable> receivedTriggers)
        {
            if (stayingActive) return true;
            
            if (eventReceived)
            {
                eventReceived = false;
                return true;
            }
    
            return false;
        }
    
        public void ResetGameEvent()
        {
            stayingActive = false;
            eventReceived = false;
        }
    
        private void ReceiveGameEvent(object prevVal, object currentVal)
        {
            eventReceived = true;
            if (KeepGameEventOn) stayingActive = true;
        }
    
        public override string ToString()
        {
            if (targetParameter != null)
                return $"{targetParameter.name}";
            
            return "<Missing GameEvent>";
        }
    }
}

