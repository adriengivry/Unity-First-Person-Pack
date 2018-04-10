using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Detector : MonoBehaviour
{
    [System.Serializable]
    public class GameObjectEvent : UnityEvent<GameObject> {}
    public class DetectionEvent : UnityEvent<GameObject, float> { }

    public DetectionEvent ObjectFoundEvent = new DetectionEvent();

    private void Update()
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
}
