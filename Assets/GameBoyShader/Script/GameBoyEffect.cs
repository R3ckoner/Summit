using UnityEngine;

[System.Serializable]
public class PaletteCollection
{
    [Tooltip("Place the palette Materials you want (by default GameBoyShader)")]
    public Material[] palettes;
}

public class GameBoyEffect : MonoBehaviour
{
    public PaletteCollection paletteCollection;
    public string taggedObjectTag = "TaggedObject";
    public float maxDistance = 10.0f; // Adjust as needed.
    public int minDownsampleSize = 1;
    public int maxDownsampleSize = 10; // Adjust as needed.

    private int currentPaletteIndex = 0;
    private int originalPaletteIndex = 0; // Store the original palette index.
    private Transform player;
    private Camera mainCamera;
    private int downsampleSize = 2; // Initial downsample size

    private void Start()
    {
        player = Camera.main.transform; // Assuming the camera is attached to the player.
        mainCamera = Camera.main;
        originalPaletteIndex = currentPaletteIndex; // Store the original palette index.
    }

    private void Update()
    {
        // Check the distance to the closest tagged object.
        GameObject closestTaggedObject = FindClosestTaggedObject();
        if (closestTaggedObject != null)
        {
            float distance = Vector3.Distance(player.position, closestTaggedObject.transform.position);

            // Map distance to downsample size within the specified range, but reverse it to make the resolution drop.
            downsampleSize = Mathf.Clamp(Mathf.RoundToInt(Mathf.Lerp(maxDownsampleSize, minDownsampleSize, distance / maxDistance)), minDownsampleSize, maxDownsampleSize);

            // Update the palette based on proximity.
            UpdatePalette(distance);
        }
        else
        {
            // Player is not in proximity of any tagged object, reset to the original values.
            downsampleSize = 2; // Reset downsample size to original value.
            RestoreOriginalPalette();
        }
    }

    private void UpdatePalette(float distance)
    {
        for (int i = 0; i < paletteCollection.palettes.Length; i++)
        {
            if (distance <= i * maxDistance / paletteCollection.palettes.Length)
            {
                currentPaletteIndex = i;
                return;
            }
        }

        // If no condition met, keep the current palette index
    }

    private void RestoreOriginalPalette()
    {
        currentPaletteIndex = originalPaletteIndex;
    }

    private GameObject FindClosestTaggedObject()
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(taggedObjectTag);
        GameObject closestObject = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject obj in taggedObjects)
        {
            float distance = Vector3.Distance(player.position, obj.transform.position);
            if (distance < closestDistance)
            {
                closestObject = obj;
                closestDistance = distance;
            }
        }

        return closestObject;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (paletteCollection.palettes == null || paletteCollection.palettes.Length == 0)
        {
            Debug.LogError("You must assign palette Materials to GameBoy Effect Script");
            return;
        }

        int width = mainCamera.pixelWidth / downsampleSize;
        int height = mainCamera.pixelHeight / downsampleSize;

        RenderTexture temp = RenderTexture.GetTemporary(width, height, 0, source.format);
        temp.filterMode = FilterMode.Point;

        Graphics.Blit(source, temp);

        Graphics.Blit(temp, destination, paletteCollection.palettes[currentPaletteIndex]);

        RenderTexture.ReleaseTemporary(temp);
    }
}
