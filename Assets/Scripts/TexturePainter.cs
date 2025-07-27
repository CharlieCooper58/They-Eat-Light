using UnityEngine;

public class TexturePainter : MonoBehaviour
{
    public int textureWidth = 1024;
    public int textureHeight = 1024;
    private Texture2D texture;
    private Renderer objectRenderer;

    [SerializeField] private int brushSize = 2;

    void Start()
    {
        // Get the renderer of the object
        objectRenderer = GetComponent<Renderer>();

        // Create a blank texture with the specified dimensions
        texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);

        // Initialize the texture to be fully transparent (0, 0, 0, 0)
        ClearTexture();

        // Assign the texture to the object's material
        objectRenderer.material.mainTexture = texture;
    }

    // Method to clear the texture (fill with transparent pixels)
    void ClearTexture()
    {
        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                texture.SetPixel(x, y, new Color(0, 0, 0, 0)); // Transparent
            }
        }

        // Apply the changes to the texture
        texture.Apply();
    }

    // Method to paint the texture at a given UV coordinate
    public void PaintTextureAtUV(RaycastHit hit)
    {
        // Get the UV coordinates of the hit point and clamp them to the [0, 1] range
        Vector2 uv = hit.textureCoord;
        uv.x = Mathf.Clamp01(uv.x);
        uv.y = Mathf.Clamp01(uv.y);

        // Convert UV coordinates to pixel coordinates on the texture
        int x = Mathf.FloorToInt(uv.x * textureWidth);
        int y = Mathf.FloorToInt(uv.y * textureHeight);

        // Paint a square area of pixels around the hit point
        for (int i = -brushSize; i <= brushSize; i++)
        {
            for (int j = -brushSize; j <= brushSize; j++)
            {
                // Ensure we don't go out of texture bounds
                int newX = Mathf.Clamp(x + i, 0, textureWidth - 1);
                int newY = Mathf.Clamp(y + j, 0, textureHeight - 1);

                // Set the pixel color to white (1, 1, 1, 1)
                texture.SetPixel(newX, newY, Color.white);
            }
        }

        // Apply the changes to the texture
        texture.Apply();
        
    }

}
