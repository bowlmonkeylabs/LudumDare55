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
    public class PregameManager : MonoBehaviour
    {
        private static string TITLE_TASK_START = "TaskStart";
        private static string TITLE_TASK_DEBRIEF = "TaskDebrief";
        
        [SerializeField, Required] private CurrentSceneLevelTaskDictionary _levelTasksDict;
        
        [SerializeField, Required] private DialogueActor _dialogueActor;
        [SerializeField, Required] private DialogueSystemTrigger _dialogueSystemTrigger;

        [SerializeField] private GameEvent _onStartTask;
        [SerializeField] private TransformSceneReference _playerSceneRef;
        [SerializeField] private TransformSceneReference _summonPointSceneRef;

        [SerializeField] private BoolVariable _outputIsPregame;

        #region Unity lifecycle

        private void OnEnable()
        {
            _onStartTask.Subscribe(OnStartTask);
        }
        
        private void OnDisable()
        {
            _onStartTask.Unsubscribe(OnStartTask);
        }
        
        private void Start()
        {
            _outputIsPregame.Value = true;
            
            var kinematicCharacterMotor = _playerSceneRef.Value.GetComponent<KinematicCharacterMotor>();
            kinematicCharacterMotor.SetPositionAndRotation(_summonPointSceneRef.Value.position, _summonPointSceneRef.Value.rotation);
            
            _dialogueActor.actor = _levelTasksDict.TryGetCurrentLevelTask()?.Actor;
            _dialogueSystemTrigger.conversation = GetPregameConversationTitle();
        }

        #endregion
        
        public void StartConversation()
        {
            _dialogueSystemTrigger.OnUse();
            Debug.Log("Starting Pregame Dialogue");
        }
        
        private string GetPregameConversationTitle()
        {
            var title =
                $"{_levelTasksDict.TryGetCurrentLevelTask()?.Actor}/{_levelTasksDict.TryGetCurrentLevelTask()?.Conversation}/Initial";

            return title;
        }

        public void OnStartTask()
        {
            Debug.Log($"OnStartTask: {_levelTasksDict.TryGetCurrentLevelTask()?.SceneCollection.name}");
            // TODO exit dialogue

            _outputIsPregame.Value = false;
            
            // TODO add transition?
            SceneHelper.current.OpenOrReopenCollection(_levelTasksDict.TryGetCurrentLevelTask()?.SceneCollection);
        }
    }
}