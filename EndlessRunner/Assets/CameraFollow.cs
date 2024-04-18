using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float trailDistance = 5.0f;
    public float heightOffset = 3.0f;
    public float turnTime = 0f;


    public float rotationSpeed = 90f; // degrees per second
    private bool isRotating = false;

  

    // Update is called once per frame
    void Update()
    {
        if (turnTime > 0)
            turnTime -= Time.deltaTime;
        if (turnTime < 0.1f)
        {
            //transform.RotateAround(target.transform.position, Vector3.up, 20 * Time.deltaTime);
            Vector3 followPos = target.position - target.forward * trailDistance;
            followPos.y += heightOffset;
            transform.position += (followPos - transform.position);
        }
        if (isRotating)
        {
            transform.RotateAround(target.position, Vector3.up, rotationSpeed * (Time.deltaTime));
            if (Mathf.Abs(transform.eulerAngles.y) > Mathf.Abs(target.eulerAngles.y))
            {
                isRotating = false;
            }
        }

    }
    public void RotateAroundPlayer()
    {
        isRotating = true;
    }
}
