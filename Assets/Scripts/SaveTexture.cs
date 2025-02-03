using UnityEngine;
using System.IO;

public class SaveTexture : MonoBehaviour
{
    public Texture2D textureToSave; // Assign this in the inspector

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) // Press 'S' to save
        {
            SaveTextureToFile(textureToSave, "SavedTexture.png");
        }
    }

    void SaveTextureToFile(Texture2D texture, string fileName)
    {
        if (texture == null)
        {
            Debug.LogError("No texture assigned!");
            return;
        }

        byte[] bytes = texture.EncodeToPNG(); // Convert to PNG format
        string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), fileName);
        File.WriteAllBytes(path, bytes);
        Debug.Log("Texture saved to: " + path);
    }
}
