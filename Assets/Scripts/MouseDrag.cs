using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class MouseDrag : MonoBehaviour
{
    
    private void Update()
    {
        if (Input.mousePosition != Vector3.zero)
        {
            transform.position = GetComponentInParent<Camera>().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.5f));
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.isThiefArrested)
        {
            return;
        }
        if (GameManager.Instance.isThiefFounded)
        {
            return;
        }
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        //layerMask = ~layerMask;

        RaycastHit hit;
        Ray ray = GetComponentInParent<Camera>().ScreenPointToRay(Input.mousePosition);
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, ray.direction * 100, Color.yellow);
            //Debug.Log("Did Hit");
            if (hit.collider.CompareTag("Thief"))
            {
                Debug.Log("Thief founded");
                GameManager.Instance.ThiefFounded();
            }
        }
        else
        {
            Debug.DrawRay(transform.position, ray.direction * 100, Color.white);
            //Debug.Log("Did not Hit");
        }
    }
}
