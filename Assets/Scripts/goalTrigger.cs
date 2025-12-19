using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalTrigger : MonoBehaviour
{
    enum Type {exit, exitCover, shop};
    [SerializeField] Type type;

    private void OnTriggerEnter(Collider other) {
        Debug.Log("trigger");
        Debug.Log(other);

        if (other.CompareTag("Player")) {
            Debug.Log("compared");
            if (type == Type.exitCover) {
                GameManager.instance.KeyCheck();
            }
            if (type == Type.exit) {
                LoadSave.instance.IncrementLevelLoad();
                GameManager.instance.LoadNextLevel(LoadSave.instance.GetLevelLoad());
            }
            if (type == Type.shop) {
                SceneManager.LoadScene("Shop");
            }
        }
    }
  
}
