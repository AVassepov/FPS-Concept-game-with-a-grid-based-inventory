using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WedgeTrigger : MonoBehaviour
{
    public Transform Target ;
    public float Radius = 1;
    public float Height = 1;
    [Range(0, 1)]
    public float AngThresh = 0.5f;

    private float VerticalOffset = -10;
    void OnDrawGizmos()
    {

        //gizmos relative to transform
       // Gizmos.matrix=Handles.matrix = transform.localToWorldMatrix;

        //Detection
        if (Constraints(Target.position))
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.white;
        }



        Vector3 top = new Vector3(0,Height,0);

        //discs
      /*  Handles.DrawWireDisc(default, Vector3.up, Radius);
        Handles.DrawWireDisc(top, Vector3.up, Radius);
        */
        //math
        float p =AngThresh;
        float x = Mathf.Sqrt(1 - p * p);
        Vector3 vLeft = new Vector3 (-x,0,p) * Radius;
        Vector3 vRight = new Vector3(x, 0, p) * Radius;

        // right side check
        Gizmos.DrawRay(default, vRight);
        Gizmos.DrawRay(top, vRight);

        // left side check
        Gizmos.DrawRay(default, vLeft);
        Gizmos.DrawRay(top, vLeft);



        // Vertical lines on edges
        Gizmos.DrawLine(default, top);
        Gizmos.DrawLine( vLeft, top + vLeft);
        Gizmos.DrawLine( vRight, top + vRight);


    }

    public bool Constraints(Vector3 position)
    {
       
        Vector3 vecToTargetWorld = (position - transform.position);

        // inverse transform is world to local
        Vector3 vecToTarget = transform.InverseTransformVector(vecToTargetWorld);
        Vector3 flatDirToTarget = vecToTarget;
        flatDirToTarget.y = 0;
        flatDirToTarget=  flatDirToTarget.normalized;

        float distanceToTarget = Vector3.Distance(transform.position, position);


        if (vecToTarget.y< VerticalOffset  || vecToTarget.y > Height) { return false; }//outside height range
        if (flatDirToTarget.z < AngThresh) { return false; }//outside angle
        if (distanceToTarget > Radius) { return false; }//out of range


        return true;
    }

}
