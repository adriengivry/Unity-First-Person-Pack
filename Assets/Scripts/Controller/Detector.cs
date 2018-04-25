using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Detector : MonoBehaviour
{
    public class GameObjectEvent : UnityEvent<GameObject> {}
    public class DetectionEvent : UnityEvent<GameObject, float> { }

    private Collider m_collider;
    private float m_distanceToGround;

    [HideInInspector] public DetectionEvent ObjectFoundEvent = new DetectionEvent();
    [HideInInspector] public UnityEvent GroundedEvent = new UnityEvent();
    [HideInInspector] public UnityEvent NotGroundedEvent = new UnityEvent();

    private void Awake()
    {
        m_collider = GetComponent<Collider>();
        m_distanceToGround = m_collider.bounds.extents.y;
    }

    private void Update()
    {
        FrontCheck();
        GroundCheck();
    }

    private void FrontCheck()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            var detected = hit.transform.gameObject;
            Collider detectedCollider = detected.GetComponent<Collider>();
            Vector3 cameraPosition = Camera.main.transform.position;
            Vector3 detectedHitClosestPoint = detectedCollider.ClosestPointOnBounds(cameraPosition);
            float distanceToCollision = Vector3.Distance(cameraPosition, detectedHitClosestPoint);
            ObjectFoundEvent.Invoke(hit.transform.gameObject, distanceToCollision);
        }
    }

    private void GroundCheck()
    {
        Vector3 colliderEnd = transform.position + Vector3.down * m_distanceToGround;
        Vector3 groundedOffset = transform.position + Vector3.down * (m_distanceToGround + 0.05f);
        float colliderScaledWidth = m_collider.bounds.extents.x * 0.8f;
        uint collisions = 0;

        Collider[] hits = Physics.OverlapCapsule(colliderEnd, groundedOffset, colliderScaledWidth);

        foreach (Collider hit in hits)
            if (hit.tag != gameObject.tag)
                ++collisions;

        if (collisions > 0)
            GroundedEvent.Invoke();
        else
            NotGroundedEvent.Invoke();
    }
}
