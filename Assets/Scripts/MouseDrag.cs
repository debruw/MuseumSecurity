using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class MouseDrag : MonoBehaviour
{
    public FloatingJoystick floatingJoystick;
    private float AxisX;         // The current value of the movement input.
    private float AxisY;             // The current value of the turn input.
    public float m_Speed = 20f;                 // How fast the tank moves forward and back.
    public float oldY = 0;
    Vector3 dir, movementVector = Vector3.right;
    float angle;

    private void Update()
    {
        if (Input.mousePosition != Vector3.zero)
        {
            transform.position = GetComponentInParent<Camera>().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.5f));
        }

        //// Store the value of both input axes.
        //AxisX = floatingJoystick.Horizontal;
        //AxisY = floatingJoystick.Vertical;

        //Vector3 movement = new Vector3(AxisX, AxisY, 0);
        //if (movement != Vector3.zero)
        //{
        //    transform.position += movement * Time.deltaTime;
        //    transform.position = new Vector3(Mathf.Clamp(transform.position.x, -.14f, .14f), Mathf.Clamp(transform.position.y, -.25f, .25f), transform.position.z);
        //}
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.isThiefArrested)
        {
            //if (Input.GetMouseButtonDown(0))
            //{
            //    Debug.Log("Go to Investigate");
            //    StartCoroutine(WaitAndInvestigate());
            //}
            return;
        }
        if (GameManager.Instance.isThiefFounded)
        {
            //if (Input.GetMouseButtonDown(0))
            //{
            //    Debug.Log("Arrest");
            //    GameManager.Instance.ArrestUI.GetComponent<Animator>().enabled = false;
            //    GameManager.Instance.CheckArrestSuccess();
            //}

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
