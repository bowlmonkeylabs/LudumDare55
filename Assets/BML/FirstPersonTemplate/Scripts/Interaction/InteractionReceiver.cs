using UnityEngine;
using UnityEngine.Events;

namespace BML.Scripts
{
    public class InteractionReceiver : MonoBehaviour
    {
        public string HoverText = "";
        public UnityEvent OnInteract;
        [SerializeField] private UnityEvent OnVacuumed;
        [SerializeField] private UnityEvent OnSprayed;
        
        public void ReceiveInteraction()
        {
            OnInteract.Invoke();
        }

        public void ReceiveVacuum() {
            OnVacuumed.Invoke();
        }

        public void ReceiveSpray() {
            OnSprayed.Invoke();
        }
    }
}