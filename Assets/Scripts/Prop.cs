using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    public Sprite UISprite;
    Camera cam;
    public Quaternion PlacingPosition;

    private void Start()
    {
        cam = GameManager.Instance.PlacingRoomCamera.GetComponent<Camera>();
    }

    private void OnMouseDrag()
    {
        if(GameManager.Instance.isPlaced)
        {
            return;
        }
        transform.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5));
    }

    private void OnMouseUp()
    {
        int layerMask = 1 << 8;
        //layerMask = ~layerMask;
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, ray.direction * 100, Color.yellow);
            //Debug.Log("Did Hit");
            if (hit.collider.CompareTag("GlassContainer"))
            {
                Debug.Log("Object placed!");
                transform.position = GameManager.Instance.placePosition.position;
                GameManager.Instance.isPlaced = true;
            }
        }
        else
        {
            Debug.Log("cant hit");
        }
    }
}
