using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    public Sprite UISprite;
    Camera cam;
    public Quaternion PlacingPosition;
    public bool isTurning = false;

    private void Start()
    {
        cam = GameManager.Instance.PlacingRoomCamera.GetComponent<Camera>();
    }

    private void Update()
    {
        if (isTurning)
        {
            transform.Rotate(0, 1, 0);
        }
    }


    private void OnMouseDrag()
    {
        if (GameManager.Instance.isPlaced || !GameManager.Instance.isAllowPlacing)
        {
            return;
        }
        transform.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 7));
    }

    private void OnMouseUp()
    {
        if (!GameManager.Instance.isAllowPlacing)
        {
            return;
        }
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
