using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class AInteraction<T> : MonoBehaviour where T : AInteractable
{
    [Header("INPUTS")]
    [SerializeField] private string m_interactInput;

    [Header("DETECTION")]
    [SerializeField] private float m_maximumDistance;

    public static UnityEvent CanInteractEvent = new UnityEvent();
    public static UnityEvent CannotInteractEvent = new UnityEvent();
    public static UnityEvent InteractEvent = new UnityEvent();

    private void Awake()
    {
        CreateDetector();
    }

    private void CreateDetector()
    {
        var detector = GetComponent<Detector>();

        if (detector == null)
        {
            detector = gameObject.AddComponent<Detector>();
        }

        if (detector != null)
        {
            detector.ObjectFoundEvent.AddListener(OnDetection);
        }
    }

    private void OnDetection(GameObject p_detected, float p_distanceTo)
    {
        if (IsValid(p_detected) && p_distanceTo <= m_maximumDistance)
        {
            if (CanInteract(p_detected))
            {
                CanInteractEvent.Invoke();
                if (InteractionTriggered())
                {
                    InteractEvent.Invoke();
                    Interact(p_detected);
                }
            }
            else
            {
                CannotInteractEvent.Invoke();
            }
        }
    }

    private bool IsValid(GameObject p_object)
    {
        return p_object.GetComponent<T>() != null || p_object.GetComponentInParent<T>() != null;
    }

    private bool InteractionTriggered()
    {
        return Input.GetButtonDown(m_interactInput);
    }

    protected abstract bool CanInteract(GameObject p_target);
    protected abstract void Interact(GameObject p_target);
}
