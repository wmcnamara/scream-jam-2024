using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeToLeave : MonoBehaviour
{
    float timeToAllowLeaving = 5.0f;

    void Update()
    {
        timeToAllowLeaving -= Time.deltaTime;
        if (timeToAllowLeaving <=0)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}
