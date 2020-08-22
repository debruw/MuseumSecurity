using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEdgeScroolling : MonoBehaviour
{
    public float edgeSize = 10;
    public float moveAmount = 10;
    public Vector2 RotationClamp;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isThiefFounded)
        {
            return;
        }
        if (Input.mousePosition.x > Screen.width - edgeSize)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + moveAmount * Time.deltaTime, transform.eulerAngles.z);
        }
        if (Input.mousePosition.x < edgeSize)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - moveAmount * Time.deltaTime, transform.eulerAngles.z);
        }
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Clamp(transform.eulerAngles.y, RotationClamp.x, RotationClamp.y), transform.eulerAngles.z);
    }
}
