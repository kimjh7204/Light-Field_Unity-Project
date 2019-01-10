using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateTexture2DArray : MonoBehaviour
{
    public int originalWidth, originalHeight;
    public int width, height, startnum, depth;
    public string directory;
    public string filePattern;
    public string resultPath;
    public string resultAssetname;

    public void CreateTexArray()
    {
        Texture2DArray textureArray = new Texture2DArray(width, height, depth + 2, TextureFormat.RGB24, false);

        string fullPath = directory + "/" + filePattern;
        byte[] textureData;

        Color[] black_solidColor = new Color[width * height];
        for (int i = 0; i < width * height; i++)
        {
            black_solidColor[i] = Color.black;
        }

        textureArray.SetPixels(black_solidColor, 0);

        for (int i = 1, j = startnum; i <= depth; i++, j += 1)
        {
            string file = string.Format(fullPath, j);

            Texture2D tex2D;

            if (File.Exists(file))
            {
                textureData = File.ReadAllBytes(file);
                tex2D = new Texture2D(originalWidth, originalHeight);
                tex2D.LoadImage(textureData);
                TextureScale.Point(tex2D, width, height);
                Debug.Log("Loaded : " + file);
                textureArray.SetPixels(tex2D.GetPixels(), i);
            }
            else
            {
                Debug.LogError("Failer : " + file);
            }

        }
        textureArray.SetPixels(black_solidColor, depth + 1);

        textureArray.Apply();

        // CHANGEME: Path where you want to save the texture array. It must end in .asset extension for Unity to recognise it.
        string result = resultPath + resultAssetname + ".asset";
        AssetDatabase.CreateAsset(textureArray, result);
        Debug.Log("Saved asset to " + fullPath);
    }
}
