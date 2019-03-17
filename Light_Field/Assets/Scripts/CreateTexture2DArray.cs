using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateTexture2DArray : MonoBehaviour
{
	public Texture2D blacktex;
	public int width, height, startnum;
	public int depth;
	public string directory;
	public string filePattern;
	public string resultPath;
	public string resultAssetname;

	private int depth_segment;

	public void CreateTexArray()
	{
		depth_segment = depth / 5;
		int startNumber = startnum;

		for(int d = 0; d < 5; d++)
		{
			Texture2DArray textureArray = new Texture2DArray(width, height, depth_segment + 2, TextureFormat.RGB24, false);

			string fullPath = directory + "/" + filePattern;
			byte[] textureData;

			Graphics.CopyTexture(blacktex, 0, 0, textureArray, 0, 0);

			for (int i = 1, j = startNumber; i <= depth_segment; i++, j += 1)
			{
				string file = string.Format(fullPath, j);

				Texture2D tex2D;

				if (File.Exists(file))
				{
					textureData = File.ReadAllBytes(file);
					tex2D = new Texture2D(width, height);
					tex2D.LoadImage(textureData);
					//TextureScale.Point(tex2D, width, height);
					//tex2D.Compress(false);
					//Debug.Log("Loaded : " + file);
					Graphics.CopyTexture(tex2D, 0, 0, textureArray, i, 0);
					//textureArray.SetPixels(tex2D.GetPixels(), i);
				}
				else
				{
					Debug.LogError("Failer : " + file);
				}

			}

			Graphics.CopyTexture(blacktex, 0, 0, textureArray, depth_segment + 1, 0);

			textureArray.Apply();

			// CHANGEME: Path where you want to save the texture array. It must end in .asset extension for Unity to recognise it.
			string result = resultPath + resultAssetname + "_" + d.ToString() + ".asset";
			AssetDatabase.CreateAsset(textureArray, result);
			Debug.Log("Saved asset to " + result);

			startNumber += depth_segment;
		}
		
	}
}
