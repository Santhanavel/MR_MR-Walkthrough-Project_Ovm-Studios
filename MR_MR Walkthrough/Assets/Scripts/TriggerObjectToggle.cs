using UnityEngine;
using UnityEngine.Events;

public class TriggerObjectToggle : MonoBehaviour
{

    [Header("Player Reference")]
    [SerializeField] private GameObject player;

    [Header("Events")]
    public UnityEvent onEnableObjects;
    public UnityEvent onDisableObjects;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != player)
            return;

        onEnableObjects?.Invoke();
        onDisableObjects?.Invoke();
    }
}
