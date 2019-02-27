using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateTexture2DArray : MonoBehaviour
{
    public Texture2D blacktex;
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
        for (int k = 0; k < depth.Length; k++)
        {
            Texture2DArray textureArray = new Texture2DArray(width, height, depth[k] + 2, TextureFormat.DXT1, false);

            string fullPath = directory + "/" + filePattern;
            byte[] textureData;

            // Color[] black_solidColor = new Color[width * height];
            // Texture2D tempTex = new Texture2D(width, height);
            // for (int i = 0; i < width * (height); i++)
            // {
            //     black_solidColor[i] = new Color(0, 0, 0);
            // }
            // tempTex.SetPixels(black_solidColor);
            Graphics.CopyTexture(blacktex, 0, 0, textureArray, 0, 0);
            //textureArray.SetPixels(black_solidColor, 0);

            for (int i = 1, j = startnum; i <= depth[k]; i++, j += 1)
            {
                string file = string.Format(fullPath, j);

                Texture2D tex2D;

                if (File.Exists(file))
                {
                    textureData = File.ReadAllBytes(file);
                    tex2D = new Texture2D(originalWidth, originalHeight);
                    tex2D.LoadImage(textureData);
                    TextureScale.Point(tex2D, width, height);
                    tex2D.Compress(false);
                    //Debug.Log("Loaded : " + file);
                    Graphics.CopyTexture(tex2D, 0, 0, textureArray, i, 0);
                    //textureArray.SetPixels(tex2D.GetPixels(), i);
                }
                else
                {
                    Debug.LogError("Failer : " + file);
                }

            }

            Graphics.CopyTexture(blacktex, 0, 0, textureArray, depth[k] + 1, 0);
            //textureArray.SetPixels(black_solidColor, depth + 1);

            textureArray.Apply();

            // CHANGEME: Path where you want to save the texture array. It must end in .asset extension for Unity to recognise it.
            string result = resultPath + "TESTT" + ".asset";
            AssetDatabase.CreateAsset(textureArray, result);
            Debug.Log("Saved asset to " + result);

            startNum += depth[k];
        }
    }
}
