using System;
using AdvancedSceneManager.Utility;
using BML.ScriptableObjectCore.Scripts.Events;
using BML.ScriptableObjectCore.Scripts.Variables;
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
        
        [SerializeField] private IntReference _currentSceneLevel;
        [SerializeField, Required] private CurrentSceneLevelTaskDictionary _levelTasksDict;
        
        [SerializeField, Required] private DialogueActor _dialogueActor;
        [SerializeField, Required] private DialogueSystemTrigger _dialogueSystemTrigger;

        [SerializeField] private GameEvent _onStartTask;

        private CurrentSceneLevelTaskDictionary.LevelTask _currentLevelTask => _levelTasksDict.GetLevelTask(_currentSceneLevel.Value);

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
            _dialogueActor.actor = _currentLevelTask.Actor;
            _dialogueSystemTrigger.conversation = GetPregameConversationTitle();
            
            _dialogueSystemTrigger.OnUse();
        }

        #endregion
        
        private string GetPregameConversationTitle()
        {
            var title =
                $"{_currentLevelTask.Actor}/{_currentLevelTask.Conversation}/Initial";

            return title;
        }

        public void OnStartTask()
        {
            Debug.Log($"OnStartTask: {_currentLevelTask.SceneCollection.name}");
            // TODO exit dialogue
            
            // TODO add transition?
            SceneHelper.current.OpenOrReopenCollection(_currentLevelTask.SceneCollection);
        }
    }
}