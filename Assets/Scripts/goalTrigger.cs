using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalTrigger : MonoBehaviour
{
    enum Type {exit, exitCover};
    [SerializeField] Type type;

    private void OnTriggerEnter(Collider other) {

        if (other.CompareTag("Player")) {
            if (type == Type.exitCover) {
                GameManager.instance.KeyCheck();
            }
            if (type == Type.exit) {
                if (SceneManager.GetActiveScene().name.EndsWith("1") || 
                    SceneManager.GetActiveScene().name.EndsWith("2") ||
                    SceneManager.GetActiveScene().name.EndsWith("3"))
                    { 
                    LoadSave.instance.LevelLoad++; 
                }
                GameManager.instance.LoadNextLevel(LoadSave.instance.LevelLoad);
            }
        }
    }
  
}
