using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedSceneManager.Models;
using AdvancedSceneManager.Utility;
using BML.ScriptableObjectCore.Scripts.Events;
using BML.ScriptableObjectCore.Scripts.SceneReferences;
using BML.ScriptableObjectCore.Scripts.Variables;
using KinematicCharacterController;
using MoreMountains.Feedbacks;
using PixelCrushers.DialogueSystem.Wrappers;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Events;
using DialogueActor = PixelCrushers.DialogueSystem.DialogueActor;

namespace BML.Scripts.Level
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private GameEvent _onCleanedGameEvent;
        [SerializeField] private GameEvent _onLevelStarted;
        [SerializeField] private TimerVariable _levelTimer;
        [SerializeField] private MMF_Player _lowTimeFeedbacks;
        [SerializeField] private float _lowTimeThreshold = 5;
        [SerializeField] private GameEvent _onPlayerDeath;
        [SerializeField] private FloatReference _levelTime;

        [SerializeField] private TransformSceneReference _playerSceneRef;
        [SerializeField] private TransformSceneReference _summonPointSceneRef;

        [SerializeField] private SceneCollection _pregameSceneCollection;
        [SerializeField] private SceneCollection _gameCompletedSceneCollection;
        
        [SerializeField] private IntVariable _currentSceneLevel;
        [SerializeField] private CurrentSceneLevelTaskDictionary _levelTasksDict;
        [SerializeField] private DialogueActor _dialogueActor;
        [SerializeField] private PixelCrushers.DialogueSystem.Wrappers.DialogueSystemTrigger _dialogueSystemTrigger;
        
        [SerializeField] private UnityEvent _onWinLevel;
        [SerializeField] private UnityEvent _onLoseLevel;

        [SerializeField] private BoolVariable _outputPlayerWonGame;

        private bool lowTimerFeedbacksPlayed;

        private void Start()
        {
            Debug.Log("Teleporting player to summon point");
            var kinematicCharacterMotor = _playerSceneRef.Value.GetComponent<KinematicCharacterMotor>();
            kinematicCharacterMotor.SetPositionAndRotation(_summonPointSceneRef.Value.position, _summonPointSceneRef.Value.rotation);
            
            DialogueLua.SetVariable("TaskSucceed", false);
            _levelTimer.Duration = _levelTime != null ? _levelTime.Value : _levelTimer.Duration;
            _levelTimer.RestartTimer();
            _onLevelStarted.Raise();
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
            
            if (_levelTimer.RemainingTime < _lowTimeThreshold && !lowTimerFeedbacksPlayed) {
                _lowTimeFeedbacks.PlayFeedbacks();
                lowTimerFeedbacksPlayed = true;
                Debug.Log("Playing low time feedbacks");
            }
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
                _onWinLevel.Invoke();
            }
        }

        private void OnLevelFailed() {
            _levelTimer.StopTimer();
            DialogueLua.SetVariable("TaskSucceed", false);
            this.StartDebriefConversation();
            _onLoseLevel.Invoke();
            Debug.Log("OnLevelFailed");
        }

        private void StartDebriefConversation() {
            _dialogueActor.actor = _levelTasksDict.TryGetCurrentLevelTask()?.Actor;
            _dialogueSystemTrigger.conversation = GetDebriefConversationTitle();
                
            _dialogueSystemTrigger.OnUse();
        }

        public void TryProgressToNextLevel()
        {
            bool levelSuccess = DialogueLua.GetVariable("TaskSucceed").AsBool;
            if (levelSuccess) {
                _currentSceneLevel.Value += 1;

                var nextTask = _levelTasksDict.TryGetCurrentLevelTask();
                if (nextTask == null) // if completed all tasks
                {
                    _outputPlayerWonGame.Value = true;
                    SceneHelper.current.OpenOrReopenCollection(_gameCompletedSceneCollection);
                }
                else
                {
                    SceneHelper.current.OpenOrReopenCollection(_pregameSceneCollection);
                }
            } else {
                SceneHelper.current.OpenOrReopenCollection(_pregameSceneCollection);
            }
        }
    }
}