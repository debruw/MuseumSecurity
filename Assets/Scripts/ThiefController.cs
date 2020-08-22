using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ThiefController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GetComponent<NavMeshAgent>().remainingDistance < 1f)
        {
            //Debug.Log();
            GetComponent<NavMeshAgent>().isStopped = true;
            GetComponent<Animator>().SetTrigger("StopMove");
        }
    }
}
