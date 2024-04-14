using System.Collections.Generic;
using AdvancedSceneManager.Models;
using BML.ScriptableObjectCore.Scripts.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BML.Scripts
{
    [CreateAssetMenu(fileName = "Level Tasks Dict", menuName = "BML/Level Tasks Dict", order = 0)]
    public class CurrentSceneLevelTaskDictionary : SerializedScriptableObject
    {
        [SerializeField] private IntVariable _currentSceneLevel;
        
        public struct LevelTask
        {
            public string Actor;
            public string Conversation;
            public SceneCollection SceneCollection;
        }

        public Dictionary<int, LevelTask> LevelTasks = new Dictionary<int, LevelTask>();
        
        public LevelTask? TryGetCurrentLevelTask()
        {
            return LevelTasks.TryGetValue(_currentSceneLevel.Value, out var task) ? (LevelTask?) task : null;
        }
        
        public LevelTask GetLevelTask(int level)
        {
            return LevelTasks[level];
        }
    }
}