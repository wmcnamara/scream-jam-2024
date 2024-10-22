using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeOutTransition : MonoBehaviour
{
    public float fadeDuration = 2f; // Duration of the fade effect
    private Image fadeImage; // Reference to the UI Image for the fade effect

    void Start()
    {
        // Create and set up the fade Image
        fadeImage = gameObject.AddComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 1); // Start as black (fully opaque)

        // Set RectTransform to cover the entire screen
        RectTransform rectTransform = fadeImage.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.offsetMin = Vector2.zero; // Set left, bottom to 0
        rectTransform.offsetMax = Vector2.zero; // Set right, top to 0

        // Start the fade out effect immediately
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        // Immediately set the fade image to black
        fadeImage.gameObject.SetActive(true); // Ensure the image is active
        fadeImage.color = new Color(0, 0, 0, 1); // Fully opaque

        float startAlpha = fadeImage.color.a;
        float fadeSpeed = 1f / fadeDuration;

        // Fade out the image
        while (fadeImage.color.a > 0)
        {
            Color newColor = fadeImage.color;
            newColor.a -= fadeSpeed * Time.deltaTime; // Decrease alpha
            fadeImage.color = newColor;
            yield return null;
        }

        // Ensure it is completely transparent at the end
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.gameObject.SetActive(false); // Optionally deactivate the image after fading out
    }
}
