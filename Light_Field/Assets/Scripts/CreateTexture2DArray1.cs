using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateTexture2DArray1 : MonoBehaviour
{
    public int originalWidth, originalHeight;
    public int width, height, startnum;
    public int[] depth;
    public string directory;
    public string filePattern;
    public string resultPath;
    public string resultAssetname;

    public void CreateTexArray()
    {
        int startNum = startnum;
        for (int k = 0; k < 5; k++)
        {
            Texture2DArray textureArray = new Texture2DArray(width, height, depth[k] + 2, TextureFormat.RGB24, false);

            string fullPath = directory + "/" + filePattern;
            byte[] textureData;

            Color[] black_solidColor = new Color[width * height];
            for (int i = 0; i < width * height; i++)
            {
                black_solidColor[i] = Color.black;
            }

            textureArray.SetPixels(black_solidColor, 0);

            for (int i = 1, j = startNum; i <= depth[k]; i++, j += 1)
            {
                string file = string.Format(fullPath, j);

                Texture2D tex2D;

                if (File.Exists(file))
                {
                    textureData = File.ReadAllBytes(file);
                    tex2D = new Texture2D(originalWidth, originalHeight);
                    tex2D.LoadImage(textureData);
                    TextureScale.Point(tex2D, width, height);
                    //Debug.Log("Loaded : " + file);
                    textureArray.SetPixels(tex2D.GetPixels(), i);
                }
                else
                {
                    Debug.LogError("Failer : " + file);
                }

            }
            textureArray.SetPixels(black_solidColor, depth[k] + 1);

            textureArray.Apply();

            // CHANGEME: Path where you want to save the texture array. It must end in .asset extension for Unity to recognise it.
            string result = resultPath + "Segment_" + "1" + (4 - k) + "1" + ".asset";
            AssetDatabase.CreateAsset(textureArray, result);
            Debug.Log("Saved asset to " + fullPath);

            startNum += depth[k];
        }

    }
}
