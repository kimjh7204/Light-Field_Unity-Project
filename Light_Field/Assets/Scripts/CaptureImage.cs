using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Text;

public class CaptureImage : MonoBehaviour
{

	public enum Direction { front, right, left, back };


	public Vector2Int LFU_Position;
	public Direction direction;
	public string resultPath;
	public float divisionNumber;
	public Material mat;

	private const float length = 9.5f;
	private const float segmentLength = length * 0.2f;

	private int Distance_property = Shader.PropertyToID("Vector1_A1C90C0E");
	private int HorizontalPositon_property = Shader.PropertyToID("Vector1_DF5BD731");
	private int PositionDelta_property = Shader.PropertyToID("Vector1_14AEA0F9");

	private float[,] imageDepth = new float[4, 3]{ {660f, 660f, 655f},
									  {640f, 645f, 645f},
									  {620f, 630f, 635f},
									  {650f, 660f, 660f} };

	public void TakeImage()
	{
		int num;
		if(direction == Direction.front)
		{
			num = 3 - LFU_Position.y;
		}
		else if(direction == Direction.right)
		{
			num = 3 - LFU_Position.x;
		}
		else if (direction == Direction.left)
		{
			num = LFU_Position.x - 1;
		}
		else
		{
			num = LFU_Position.y - 1;
		}


		mat.SetFloat(PositionDelta_property, length / imageDepth[(int)direction, num]);
		float divisionInterval = segmentLength / divisionNumber;
		StringBuilder resultFileName = new StringBuilder("\\");

		for (int i = 0; i < divisionNumber; i++)
		{
			mat.SetFloat(Distance_property, (i + 0.5f) * divisionInterval);
			for (int j = 0; j < divisionNumber; j++)
			{
				mat.SetFloat(HorizontalPositon_property, (j + 0.5f) * divisionInterval);
				if (direction == Direction.front)
				{
					resultFileName.Append(((int)((LFU_Position.x - 1) * divisionNumber + j)).ToString("D3"));
					resultFileName.Append("_");
					resultFileName.Append(((int)(LFU_Position.y * divisionNumber - i - 1)).ToString("D3"));
					resultFileName.Append("_");
					resultFileName.Append("F");
				}
				else if (direction == Direction.right)
				{
					resultFileName.Append(((int)(LFU_Position.x * divisionNumber - i - 1)).ToString("D3"));
					resultFileName.Append("_");
					resultFileName.Append(((int)(LFU_Position.y * divisionNumber - j - 1)).ToString("D3"));
					resultFileName.Append("_");
					resultFileName.Append("R");
				}
				else if (direction == Direction.left)
				{
					resultFileName.Append(((int)((LFU_Position.x - 1) * divisionNumber + i)).ToString("D3"));
					resultFileName.Append("_");
					resultFileName.Append(((int)((LFU_Position.y - 1) * divisionNumber + j)).ToString("D3"));
					resultFileName.Append("_");
					resultFileName.Append("L");
				}
				else
				{
					resultFileName.Append(((int)(LFU_Position.x * divisionNumber - j - 1)).ToString("D3"));
					resultFileName.Append("_");
					resultFileName.Append(((int)((LFU_Position.y - 1) * divisionNumber + i)).ToString("D3"));
					resultFileName.Append("_");
					resultFileName.Append("B");
				}

				resultFileName.Append(".jpg");
				BakeTexture(resultFileName);
				resultFileName.Remove(1, resultFileName.Length - 1);
			}
		}
	}

	void BakeTexture(StringBuilder path)
	{
		StringBuilder fullPath = new StringBuilder(resultPath);
		fullPath.Append(path.ToString());
		//render material to rendertexture
		RenderTexture renderTexture = RenderTexture.GetTemporary(2048, 1024);
		Graphics.Blit(null, renderTexture, mat, 0);

		//transfer image from rendertexture to texture
		Texture2D texture = new Texture2D(2048, 1024);
		RenderTexture.active = renderTexture;
		texture.ReadPixels(new Rect(Vector2.zero, new Vector2Int(2048, 1024)), 0, 0);
		//Debug.Log(texture.height);
		//save texture to file
		byte[] jpg = texture.EncodeToJPG();
		File.WriteAllBytes(fullPath.ToString(), jpg);
		AssetDatabase.Refresh();

		//clean up variables
		RenderTexture.active = null;
		RenderTexture.ReleaseTemporary(renderTexture);
		DestroyImmediate(texture);
	}
}
