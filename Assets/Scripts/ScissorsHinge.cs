using UnityEngine;

public class ScissorsHinge : MonoBehaviour
{
    private HingeJoint hinge;
    private bool isOpen = false;
    void Start()
    {
        hinge = GetComponent<HingeJoint>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isOpen = !isOpen;
            JointSpring spring = hinge.spring;

            if (isOpen)
                spring.targetPosition = 45f; // Open angle
            else
                spring.targetPosition = 0f; // Closed angle

            hinge.spring = spring;
            hinge.useSpring = true;
        }
    }
}
