using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFinish : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger");
        if (other.CompareTag("player"))
        {
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Level3"))
            {
                SceneManager.LoadScene("MainMenu");
            }
            else
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        }
    }
}
