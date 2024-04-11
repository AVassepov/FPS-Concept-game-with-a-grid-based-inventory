using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    public Vector3 Target;

    private LineRenderer lineRenderer;

    private Vector3 FirstPoint;
    private Vector3 SecondPoint;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        ChangeLine();
        
    }


    void ChangeLine()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, Target);
    }
}
