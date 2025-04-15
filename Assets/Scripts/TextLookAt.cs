using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextLookAt : MonoBehaviour
{
    public Transform target; // The target object to look at

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position+target.rotation*Vector3.forward,target.rotation*Vector3.up); // Make the text look at the target
        //transform.rotation = Quaternion.Euler(0, 180, 0); // Invert the y rotation to face the camera
    }
}
