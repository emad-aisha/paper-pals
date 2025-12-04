using UnityEngine;

public class ScissorsRotate : MonoBehaviour
{
    public Transform bladeA;   
    public Transform bladeB;   

    public float openAngle = 45f;
    public float speed = 200f;

    private bool isOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            isOpen = !isOpen;

        
        float targetA = isOpen ? openAngle : 0f;
        float targetB = isOpen ? -openAngle : 0f;

        RotateBlade(bladeA, targetA);
        RotateBlade(bladeB, targetB);
    }

    void RotateBlade(Transform blade, float targetAngle)
    {
        Quaternion targetRot = Quaternion.Euler(0, 0, targetAngle);

        blade.localRotation = Quaternion.RotateTowards(
            blade.localRotation,
            targetRot,
            speed * Time.deltaTime
        );
    }
}