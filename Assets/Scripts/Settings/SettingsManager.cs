using BML.Scripts;
using MoreMountains.Tools;
using UnityEngine;

namespace BML.Scripts
{
    public class SettingsManager : MMPersistentSingleton<SettingsManager>
    {
        [SerializeField] private LoadVariables _loadSettings;

        private bool _loaded;

        void OnEnable()
        {
            if (!_loaded)
            {
                _loadSettings.Load();
                _loaded = true;
            }
        }
    }
}
