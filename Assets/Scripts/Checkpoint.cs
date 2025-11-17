using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{

    [SerializeField] Renderer model;

    Color colorOrig;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.instance.playerSpawnPos.transform.position != transform.position)
        {
            GameManager.instance.playerSpawnPos.transform.position = transform.position;
            StartCoroutine(Feedback());
        }
    }

    IEnumerator Feedback()
    {
        model.material.color = Color.red;
        GameManager.instance.checkpointPopup.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        GameManager.instance.checkpointPopup.SetActive(false);
        model.material.color = colorOrig;
    }

}
