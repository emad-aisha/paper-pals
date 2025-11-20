using System.Collections;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    enum Type {exit, exitCover};
    [SerializeField] Type type;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (type == Type.exitCover) {
                GameManager.instance.KeyCheck();
            }
            else if (type == Type.exit) {
                GameManager.instance.LoadNextLevel();
            }
        }
    }
  
}
