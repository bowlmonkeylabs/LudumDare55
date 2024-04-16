using System;
using BML.ScriptableObjectCore.Scripts.Variables;
using BML.ScriptableObjectCore.Scripts.Variables.SafeValueReferences;
using Sirenix.Utilities;
using UnityEngine;

namespace BML.ScriptableObjectCore.Scripts.Variables.VariableWrappers
{
    public class SetActiveNew : MonoBehaviour
    {
        [SerializeField] private SafeBoolValueReference Enabled;
        [SerializeField] private GameObject[] Targets;

        private void Start()
        {
            Targets.ForEach(t => t.SetActive(Enabled.Value));
            Enabled.Subscribe(() =>
            {
                Targets.ForEach(t => t.SetActive(Enabled.Value));
            });
        }
    }
}