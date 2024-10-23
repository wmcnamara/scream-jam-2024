using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject menuPrefab;

    public static PauseManager Instance { get { return localInstance; } }
    private static PauseManager localInstance;

    public bool IsPaused { get; private set; }

    private void Start()
    {
        localInstance = this;
        menuPrefab.SetActive(false);
    }

    public void TogglePause() 
    {
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0.0f : 1.0f;
        menuPrefab.SetActive(IsPaused);
        Cursor.visible = IsPaused;
        Cursor.lockState = IsPaused ? CursorLockMode.None : CursorLockMode.Confined;
    }
}
