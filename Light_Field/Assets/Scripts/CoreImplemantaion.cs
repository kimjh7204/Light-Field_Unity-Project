using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreImplemantaion : MonoBehaviour
{
	public Material front_plain, right_plain, back_plain, left_plain;
	public Transform cameraTransform;
	public Transform ViewPlainCenter;

	public Texture2DArray[] Front;
	public Texture2DArray[] Right;
	public Texture2DArray[] Back;
	public Texture2DArray[] Left;

	private Vector3 initialCameraPos;

	private enum SmallAreaPosition_y { top, mid, bottom };
	private enum SmallAreaPosition_x { left, mid, right };

	private const float fixedAreaLength = 9.5f;

	private SmallAreaPosition_x smallArea_x;
	private SmallAreaPosition_y smallArea_y;
	private Vector2 offset;
		
	private Vector2 position;
	private Vector2 AdjustedPosition
	{
		get { return position + offset; }
		set
		{
			position = value;

			if(position.y > fixedAreaLength * 0.1f)
			{
				if(smallArea_y != SmallAreaPosition_y.top)
				{
					smallArea_y = SmallAreaPosition_y.top;
					offset.y = -fixedAreaLength * 0.2f;

					front_plain.SetTexture(Texture_property, Front[0]);
					back_plain.SetTexture(Texture_property, Back[2]);

					front_plain.SetFloat(PositionDelta_property, fixedAreaLength / imageDepth[0, 0]);
					back_plain.SetFloat(PositionDelta_property, fixedAreaLength / imageDepth[2, 2]);
				}
			}
			else if(position.y < -fixedAreaLength * 0.1f)
			{
				if(smallArea_y != SmallAreaPosition_y.bottom)
				{
					smallArea_y = SmallAreaPosition_y.bottom;
					offset.y = fixedAreaLength * 0.2f;

					front_plain.SetTexture(Texture_property, Front[2]);
					back_plain.SetTexture(Texture_property, Back[0]);

					front_plain.SetFloat(PositionDelta_property, fixedAreaLength / imageDepth[0, 2]);
					back_plain.SetFloat(PositionDelta_property, fixedAreaLength / imageDepth[2, 0]);
				}
			}
			else
			{
				if(smallArea_y != SmallAreaPosition_y.mid)
				{
					smallArea_y = SmallAreaPosition_y.mid;
					offset.y = 0.0f;

					front_plain.SetTexture(Texture_property, Front[1]);
					back_plain.SetTexture(Texture_property, Back[1]);

					front_plain.SetFloat(PositionDelta_property, fixedAreaLength / imageDepth[0, 1]);
					back_plain.SetFloat(PositionDelta_property, fixedAreaLength / imageDepth[2, 1]);
				}
			}

			if (position.x > fixedAreaLength * 0.1f)
			{
				if (smallArea_x != SmallAreaPosition_x.right)
				{
					smallArea_x = SmallAreaPosition_x.right;
					offset.x = -fixedAreaLength * 0.2f;

					right_plain.SetTexture(Texture_property, Right[0]);
					left_plain.SetTexture(Texture_property, Left[2]);

					right_plain.SetFloat(PositionDelta_property, fixedAreaLength / imageDepth[1, 0]);
					left_plain.SetFloat(PositionDelta_property, fixedAreaLength / imageDepth[3, 2]);
				}
			}
			else if (position.x < -fixedAreaLength * 0.1f)
			{
				if (smallArea_x != SmallAreaPosition_x.left)
				{
					smallArea_x = SmallAreaPosition_x.left;
					offset.x = fixedAreaLength * 0.2f;

					right_plain.SetTexture(Texture_property, Right[2]);
					left_plain.SetTexture(Texture_property, Left[0]);

					right_plain.SetFloat(PositionDelta_property, fixedAreaLength / imageDepth[1, 2]);
					left_plain.SetFloat(PositionDelta_property, fixedAreaLength / imageDepth[3, 0]);
				}
			}
			else
			{
				if (smallArea_x != SmallAreaPosition_x.mid)
				{
					smallArea_x = SmallAreaPosition_x.mid;
					offset.x = 0.0f;

					right_plain.SetTexture(Texture_property, Right[1]);
					left_plain.SetTexture(Texture_property, Left[1]);

					right_plain.SetFloat(PositionDelta_property, fixedAreaLength / imageDepth[1, 1]);
					left_plain.SetFloat(PositionDelta_property, fixedAreaLength / imageDepth[3, 1]);
				}
			}
		}
	}	

	//쉐이더에서 사용할 프로퍼티 변수를 ID형태로 저장 합니다.
	private int Distance_property;
	private int HorizontalPositon_property;
	private int Texture_property;
	private int PositionDelta_property;

	private float[,] imageDepth;

	void Start()
	{
		initialCameraPos = cameraTransform.position;
		ViewPlainCenter.rotation = Quaternion.Euler(0, cameraTransform.rotation.eulerAngles.y,0);

		// [0,X] : front, [1,X] : right, [2,X] : back, [3,X] : left
		imageDepth = new float[4, 3]{ {66.2f, 66.3f, 65.7f},
									  {64.0f, 64.5f, 64.6f},
									  {65.3f, 66.3f, 66.0f},
									  {62.4f, 63.3f, 63.9f} };
		

		//프로퍼티 이름으로 부터 ID를 추출 합니다.
		//프로퍼티 이름은 해당 쉐이더를 클릭하여 나오는 오른쪽 Inspector창에서 확인해야 합니다.
		Distance_property = Shader.PropertyToID("Vector1_A1C90C0E");
		HorizontalPositon_property = Shader.PropertyToID("Vector1_DF5BD731");
		Texture_property = Shader.PropertyToID("Texture2DArray_F092712C");
		PositionDelta_property = Shader.PropertyToID("Vector1_14AEA0F9");
	}

	void Update()
	{
		ViewPlainCenter.position = cameraTransform.position;

		AdjustedPosition = new Vector2(cameraTransform.position.x - initialCameraPos.x, cameraTransform.position.z - initialCameraPos.z) * 10f;
		
		front_plain.SetFloat(Distance_property, fixedAreaLength * 0.1f - AdjustedPosition.y);
		front_plain.SetFloat(HorizontalPositon_property, fixedAreaLength * 0.5f + position.x);

		back_plain.SetFloat(Distance_property, fixedAreaLength * 0.1f + AdjustedPosition.y);
		back_plain.SetFloat(HorizontalPositon_property, fixedAreaLength * 0.5f - position.x);

		right_plain.SetFloat(Distance_property, fixedAreaLength * 0.1f - AdjustedPosition.x);
		right_plain.SetFloat(HorizontalPositon_property, fixedAreaLength * 0.5f - position.y);

		left_plain.SetFloat(Distance_property, fixedAreaLength * 0.1f + AdjustedPosition.x);
		left_plain.SetFloat(HorizontalPositon_property, fixedAreaLength * 0.5f + position.y);
	}
}
