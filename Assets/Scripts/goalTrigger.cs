using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalTrigger : MonoBehaviour
{
    enum Type {exit, exitCover, shop};
    [SerializeField] Type type;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (type == Type.exitCover) {
                GameManager.instance.KeyCheck();
            }
            if (type == Type.exit) {
                LoadSave.instance.IncrementLevelLoad();
                Debug.Log("level to load: " + LoadSave.instance.GetLevelLoad());
                GameManager.instance.LoadNextLevel(LoadSave.instance.GetLevelLoad());
            }
            if (type == Type.shop) {
                SceneManager.LoadScene("Shop");
            }
        }
    }
  
}
