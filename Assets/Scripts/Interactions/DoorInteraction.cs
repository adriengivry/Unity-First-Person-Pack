using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * A script to place on any gameObject able to open doors
 */
public class DoorInteraction : AInteraction<Door>
{
    [Header("TRIGGER")]
    [SerializeField] private bool m_canUseTriggers;
    [SerializeField] private bool m_isAbleToManuallyInteract;

    public bool CanUseTriggers()
    {
        return m_canUseTriggers;
    }

    protected override bool CanInteract(GameObject p_target)
    {
        return p_target.GetComponentInParent<Door>().CanInteractWith();
    }

    protected override void Interact(GameObject p_target)
    {
        p_target.GetComponentInParent<Door>().Interact(gameObject);
    }
}
