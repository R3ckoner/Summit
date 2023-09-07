using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public float typingSpeed = 30f;
    public bool clearTextAfterTyping = true; // Checkbox to clear text after typing
    public float startDelay = 2.0f; // Time to wait before typing starts
    public float reverseDelay = 2.0f; // Time to wait before reversing the text

    private string currentText = "";

    private void Start()
    {
        textMeshPro.text = ""; // Clear the text initially
    }

    public void StartTypingEffect(string message)
    {
        StartCoroutine(StartTypingAfterDelay(message));
    }

    private IEnumerator StartTypingAfterDelay(string message)
    {
        // Wait for the startDelay before starting to type
        yield return new WaitForSeconds(startDelay);

        // Start the typing effect coroutine
        StartCoroutine(TypeText(message));
    }

    private IEnumerator TypeText(string message)
    {
        for (int i = 0; i < message.Length; i++)
        {
            currentText += message[i];
            textMeshPro.text = currentText;

            // Adjust typing speed by deltaTime for smoother animation
            yield return new WaitForSeconds(1f / typingSpeed * Time.deltaTime);
        }

        // Check if clearTextAfterTyping is true, and clear the text if it is
        if (clearTextAfterTyping)
        {
            yield return new WaitForSeconds(reverseDelay); // Wait before reversing

            // Reverse the text by removing characters one by one
            for (int i = currentText.Length; i >= 0; i--)
            {
                currentText = currentText.Substring(0, i);
                textMeshPro.text = currentText;

                // Adjust typing speed by deltaTime for smoother animation
                yield return new WaitForSeconds(1f / typingSpeed * Time.deltaTime);
            }
        }
    }
}
