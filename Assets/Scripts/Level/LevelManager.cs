using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedSceneManager.Models;
using AdvancedSceneManager.Utility;
using BML.ScriptableObjectCore.Scripts.Events;
using BML.ScriptableObjectCore.Scripts.SceneReferences;
using BML.ScriptableObjectCore.Scripts.Variables;
using KinematicCharacterController;
using PixelCrushers.DialogueSystem.Wrappers;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using DialogueActor = PixelCrushers.DialogueSystem.DialogueActor;

namespace BML.Scripts.Level
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private GameEvent _onCleanedGameEvent;
        [SerializeField] private TimerVariable _levelTimer;
        [SerializeField] private GameEvent _onPlayerDeath;

        [SerializeField] private TransformSceneReference _playerSceneRef;
        [SerializeField] private TransformSceneReference _summonPointSceneRef;

        [SerializeField] private SceneCollection _pregameSceneCollection;
        [SerializeField] private SceneCollection _gameCompletedSceneCollection;
        
        [SerializeField] private IntVariable _currentSceneLevel;
        [SerializeField] private CurrentSceneLevelTaskDictionary _levelTasksDict;
        [SerializeField] private DialogueActor _dialogueActor;
        [SerializeField] private PixelCrushers.DialogueSystem.Wrappers.DialogueSystemTrigger _dialogueSystemTrigger;

        private void Start()
        {
            _playerSceneRef.Value.GetComponent<KinematicCharacterMotor>().MoveCharacter(_summonPointSceneRef.Value.position);
            DialogueLua.SetVariable("TaskSucceed", false);
            _levelTimer.RestartTimer();
        }

        private void OnEnable()
        {
            _onCleanedGameEvent.Subscribe(CheckIfLevelIsCleaned);
            _levelTimer.SubscribeFinished(OnLevelFailed);
            _onPlayerDeath.Subscribe(OnLevelFailed);
        }
        
        private void OnDisable()
        {
            _onCleanedGameEvent.Unsubscribe(CheckIfLevelIsCleaned);
            _levelTimer.UnsubscribeFinished(OnLevelFailed);
            _onPlayerDeath.Unsubscribe(OnLevelFailed);
        }

        private void Update() {
            _levelTimer.UpdateTime();
        }
        
        private string GetDebriefConversationTitle()
        {
            var title =
                $"{_levelTasksDict.TryGetCurrentLevelTask()?.Actor}/{_levelTasksDict.TryGetCurrentLevelTask()?.Conversation}/Debrief";

            return title;
        }
        
        private void CheckIfLevelIsCleaned()
        {
            bool levelHasRemainingCleanables = FindObjectsOfType<Cleanable>().ToList().Exists(c => !c.IsCleaned);
            Debug.Log($"levelHasRemainingCleanables: {levelHasRemainingCleanables}");
            if (!levelHasRemainingCleanables)
            {
                Debug.Log("No remaining cleanables, level is clean!");
                
                _levelTimer.StopTimer();
                DialogueLua.SetVariable("TaskSucceed", true);
                this.StartDebriefConversation();
            }
        }

        private void OnLevelFailed() {
            _levelTimer.StopTimer();
            DialogueLua.SetVariable("TaskSucceed", false);
            this.StartDebriefConversation();
        }

        private void StartDebriefConversation() {
            _dialogueActor.actor = _levelTasksDict.TryGetCurrentLevelTask()?.Actor;
            _dialogueSystemTrigger.conversation = GetDebriefConversationTitle();
                
            _dialogueSystemTrigger.OnUse();
        }

        public void TryProgressToNextLevel()
        {
            bool levelSuccess = DialogueLua.GetVariable("TaskSucceed").AsBool;
            if(levelSuccess) {
                _currentSceneLevel.Value += 1;

                var nextScene = (_levelTasksDict.TryGetCurrentLevelTask() != null)
                    ? _pregameSceneCollection
                    : _gameCompletedSceneCollection;
                SceneHelper.current.OpenOrReopenCollection(nextScene);
            } else {
                SceneHelper.current.OpenOrReopenCollection(_pregameSceneCollection);
            }
        }
    }
}