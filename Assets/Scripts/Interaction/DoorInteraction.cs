using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * A script to place on any gameObject able to open doors
 */
public class DoorInteraction : MonoBehaviour
{
    public static UnityEvent CanInteractWithDoorEvent = new UnityEvent();
    public static UnityEvent CannotInteractWithDoorEvent = new UnityEvent();
    public static UnityEvent InteractWithDoorEvent = new UnityEvent();

    [Header("DOOR INTERACTION PARAMETERS")]
    [SerializeField] private bool m_canUseTriggers;
    [SerializeField] private bool m_isAbleToManuallyInteract;
    [SerializeField] private string m_interactInput;
    [SerializeField] private float m_minimumDistanceToOpen;

    private void Awake()
    {
        if (m_isAbleToManuallyInteract)
        {
            GetComponent<Detector>().ObjectFound.AddListener(OnDetection);
        }
    }

    private void OnDetection(GameObject p_detected, float p_distanceTo)
    {
        Door attachedDoor = p_detected.GetComponentInParent<Door>();

        if (attachedDoor != null && p_distanceTo <= m_minimumDistanceToOpen)
        {
            if (attachedDoor.CanInteractWith())
            {
                CanInteractWithDoorEvent.Invoke();
                if (Input.GetButtonDown(m_interactInput))
                {
                    InteractWithDoorEvent.Invoke();
                    attachedDoor.Interact(gameObject);
                }
            }
            else
            {
                CannotInteractWithDoorEvent.Invoke();
            }
        }
    }

    public bool CanUseTriggers()
    {
        return m_canUseTriggers;
    }
}
