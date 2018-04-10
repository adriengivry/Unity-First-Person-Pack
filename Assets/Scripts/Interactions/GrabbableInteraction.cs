using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * A script to put on a Player to allow him to grab and throw objects (First person needed)
 */
public class GrabbableInteraction : AInteraction<Grabbable>
{
    [Header("OTHER INPUTS")]
    [SerializeField] private string m_dropInput;
    [SerializeField] private string m_throwInput;

    [Header("SMOOTHING")]
    [Range(0, 1)] [SerializeField] private float m_objectPositionSmoothing;
    [Range(0, 1)] [SerializeField] private float m_objectRotationSmoothing;

    [Header("THROW")]
    [SerializeField] private float m_throwStrength;

    private GameObject m_grabbedObject;
    private Grabbable m_grabbedObjectScript;

    private float m_distanceBetweenObjectAndCamera = 0.6f;
    private float m_distanceBetweenObjectAndCameraDueToMeshSize;

    private Vector3 m_objectVelocity;
    private Vector3 m_objectAngularVelocity;

    private bool m_grabbedDuringThisFrame;

    private void Update()
    {
        if (m_grabbedObject != null)
        {
            UpdateGrabbedObject();

            if (!m_grabbedDuringThisFrame)
            {
                if (Input.GetButtonDown(m_dropInput))
                    DropObject();
                else if (Input.GetButtonDown(m_throwInput))
                    ThrowObject(m_throwStrength);
            }
        }

        m_grabbedDuringThisFrame = false;
    }

    protected override bool CanInteract(GameObject p_target)
    {
        return true;
    }

    protected override void Interact(GameObject p_target)
    {
        Grab(p_target);
    }


    private void UpdateGrabbedObject()
    {
        var currentPosition = m_grabbedObject.transform.position;
        var targetedPosition = Camera.main.transform.position + Camera.main.transform.forward * (m_distanceBetweenObjectAndCamera + m_distanceBetweenObjectAndCameraDueToMeshSize);
        m_grabbedObject.transform.position = Vector3.MoveTowards(currentPosition, targetedPosition, m_objectPositionSmoothing);

        var currentRotation = m_grabbedObject.transform.rotation;
        var targetedRotation = transform.rotation;
        m_grabbedObject.transform.rotation = Quaternion.Slerp(currentRotation, targetedRotation, m_objectRotationSmoothing);

        if (m_grabbedObjectScript.IsLinkedLost())
            DropObject();
    }

    private void Grab(GameObject p_toGrab)
    {
        InteractEvent.Invoke();
        m_grabbedObject = p_toGrab;
        m_grabbedObjectScript = m_grabbedObject.GetComponent<Grabbable>();

        m_grabbedObjectScript.Grab(gameObject);
        m_distanceBetweenObjectAndCameraDueToMeshSize = m_grabbedObjectScript.CalculateDistanceToCameraOffset();
        m_grabbedDuringThisFrame = true;
    }

    private void DropObject()
    {
        ThrowObject(0);
    }

    private void ThrowObject(float p_strength)
    {
        InteractEvent.Invoke();

        m_grabbedObjectScript.Drop(gameObject);
        m_grabbedObjectScript.Throw(p_strength);

        m_grabbedObject = null;
        m_grabbedObjectScript = null;
    }
}
