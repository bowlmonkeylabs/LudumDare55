using System;
using System.Collections.Generic;
using System.Linq;
using BML.ScriptableObjectCore.Scripts.Events;
using UnityEngine;

namespace BML.Scripts.Level
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private GameEvent _onCleanedGameEvent;

        private void OnEnable()
        {
            _onCleanedGameEvent.Subscribe(CheckIfLevelIsCleaned);
        }
        
        private void OnDisable()
        {
            _onCleanedGameEvent.Unsubscribe(CheckIfLevelIsCleaned);
        }
        
        private void CheckIfLevelIsCleaned()
        {
            bool levelHasRemainingCleanables = FindObjectsOfType<Cleanable>().ToList().Exists(c => !c.IsCleaned);
            Debug.Log($"levelHasRemainingCleanables: {levelHasRemainingCleanables}");
            if (!levelHasRemainingCleanables)
            {
                //TODO: WIN (go to next level)
                Debug.Log("No remaining cleanables, level is clean!");
            }
        }
    }
}