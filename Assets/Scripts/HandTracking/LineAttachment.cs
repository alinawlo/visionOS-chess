// Connect the mesh between two points

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineAttachment : MonoBehaviour
{
    public Transform start;
    public Transform end;

    private Vector3 localScale;

    void Start(){
        localScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = start.position;
        transform.LookAt(end, Vector3.up);
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, Vector3.Distance(end.transform.position, start.transform.position)/2);
    }
}
