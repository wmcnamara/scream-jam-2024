using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameTrigger : MonoBehaviour
{
    public GameObject entity; // Reference to the entity
    public float fadeDuration = 2f; // How long the fade should take
    public string nextSceneName = "Credits"; // Name of the scene to load

    private Image fadeImage; // Reference to the dynamically created UI Image for the fade effect
    private bool isFading = false;

    void Start()
    {
        // Create a Canvas
        GameObject canvasObject = new GameObject("FadeCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>(); // Optional: Adds the ability to scale with screen size
        canvasObject.AddComponent<GraphicRaycaster>(); // Optional: If you need UI interaction

        // Create the fade Image
        GameObject imageObject = new GameObject("FadeImage");
        imageObject.transform.SetParent(canvasObject.transform, false);
        fadeImage = imageObject.AddComponent<Image>();

        // Set image properties (black color, fully transparent)
        fadeImage.color = new Color(0, 0, 0, 0);

        // Set RectTransform to cover the entire screen
        RectTransform rectTransform = fadeImage.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.offsetMin = Vector2.zero; // Set left, bottom to 0
        rectTransform.offsetMax = Vector2.zero; // Set right, top to 0

        // Disable the Image until triggered
        fadeImage.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isFading)
        {
            // Stop the entity and begin the end game sequence
            StopEntity();
            StartCoroutine(FadeAndSwitchScene());
        }
    }

    void StopEntity()
    {
        if (entity != null)
        {
            // Call the StopEntity method from TheEntity script
            var entityScript = entity.GetComponent<TheEntity>();
            if (entityScript != null)
            {
                entityScript.StopEntity(); // Call the method to stop the entity
                Debug.Log("Called StopEntity method on TheEntity.");
            }
            else
            {
                Debug.LogError("TheEntity script not found on the entity!");
            }
        }
        else
        {
            Debug.LogError("Entity reference is null!");
        }
    }


    IEnumerator FadeAndSwitchScene()
    {
        isFading = true;

        // Enable the Image for the fade effect
        fadeImage.gameObject.SetActive(true);

        // Gradually fade to black
        float fadeSpeed = 1f / fadeDuration;
        Color fadeColor = fadeImage.color;
        fadeColor.a = 0f;
        fadeImage.color = fadeColor;

        while (fadeImage.color.a < 1f)
        {
            fadeColor.a += fadeSpeed * Time.deltaTime;
            fadeImage.color = fadeColor;
            yield return null;
        }

        // After fade completes, load the new scene
        SceneManager.LoadScene(nextSceneName);
    }
}
