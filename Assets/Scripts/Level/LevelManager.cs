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
using UnityEngine;
using DialogueActor = PixelCrushers.DialogueSystem.DialogueActor;

namespace BML.Scripts.Level
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private GameEvent _onCleanedGameEvent;
        [SerializeField] private TransformSceneReference _playerSceneRef;
        [SerializeField] private TransformSceneReference _summonPointSceneRef;

        [SerializeField] private SceneCollection _pregameSceneCollection;
        [SerializeField] private SceneCollection _gameCompletedSceneCollection;
        
        [SerializeField] private IntVariable _currentSceneLevel;
        [SerializeField] private CurrentSceneLevelTaskDictionary _levelTasksDict;
        [SerializeField] private DialogueActor _dialogueActor;
        [SerializeField] private DialogueSystemTrigger _dialogueSystemTrigger;

        private void Start()
        {
            _playerSceneRef.Value.GetComponent<KinematicCharacterMotor>().MoveCharacter(_summonPointSceneRef.Value.position);
            Debug.Log("Teleporting player to summon point");
        }

        private void OnEnable()
        {
            _onCleanedGameEvent.Subscribe(CheckIfLevelIsCleaned);
        }
        
        private void OnDisable()
        {
            _onCleanedGameEvent.Unsubscribe(CheckIfLevelIsCleaned);
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
                //TODO: WIN (go to next level)
                Debug.Log("No remaining cleanables, level is clean!");
                
                _dialogueActor.actor = _levelTasksDict.TryGetCurrentLevelTask()?.Actor;
                _dialogueSystemTrigger.conversation = GetDebriefConversationTitle();
                
                _dialogueSystemTrigger.OnUse();
            }
        }

        public void TryProgressToNextLevel()
        {
            _currentSceneLevel.Value += 1;

            var nextScene = (_levelTasksDict.TryGetCurrentLevelTask() != null)
                ? _pregameSceneCollection
                : _gameCompletedSceneCollection;
            SceneHelper.current.OpenOrReopenCollection(nextScene);
        }
    }
}