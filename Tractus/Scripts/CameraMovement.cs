using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject Target;
    public GameObject LeftEdge;
    public GameObject RightEdge;
    public GameObject BottomEdge; 
    public GameObject TopEdge;

    void LateUpdate()
    {
        if (Target == null || LeftEdge == null || RightEdge == null || BottomEdge == null || TopEdge == null)
        {
            return;
        }
        
        float targetX = Target.transform.position.x;
        float targetY = Target.transform.position.y;
        
        targetX = Mathf.Clamp(targetX, LeftEdge.transform.position.x, RightEdge.transform.position.x);
        targetY = Mathf.Clamp(targetY, BottomEdge.transform.position.y, TopEdge.transform.position.y);
        transform.position = new Vector3(targetX, targetY, transform.position.z);
    }
}