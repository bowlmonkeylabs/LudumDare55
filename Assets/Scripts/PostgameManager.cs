using System;
using AdvancedSceneManager.Utility;
using BML.ScriptableObjectCore.Scripts.Events;
using BML.ScriptableObjectCore.Scripts.SceneReferences;
using BML.ScriptableObjectCore.Scripts.Variables;
using KinematicCharacterController;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using DialogueActor = PixelCrushers.DialogueSystem.Wrappers.DialogueActor;
using DialogueSystemEvents = PixelCrushers.DialogueSystem.DialogueSystemEvents;
using DialogueSystemTrigger = PixelCrushers.DialogueSystem.Wrappers.DialogueSystemTrigger;

namespace BML.Scripts
{
    public class PostgameManager : MonoBehaviour
    {
        [SerializeField, Required] private DialogueActor _dialogueActor;
        [SerializeField, Required] private DialogueSystemTrigger _dialogueSystemTrigger;

        [SerializeField] private TransformSceneReference _playerSceneRef;
        [SerializeField] private TransformSceneReference _summonPointSceneRef;

        #region Unity lifecycle
        
        private void Start()
        {
            var kinematicCharacterMotor = _playerSceneRef.Value.GetComponent<KinematicCharacterMotor>();
            kinematicCharacterMotor.SetPositionAndRotation(_summonPointSceneRef.Value.position, _summonPointSceneRef.Value.rotation);
            
            _dialogueSystemTrigger.OnUse();
        }

        #endregion
    }
}