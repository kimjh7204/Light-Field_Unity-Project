using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CoreImplemantaion : MonoBehaviour
{
   public Material front_plain, right_plain, back_plain, left_plain;
   public Transform cameraTransform;
   public Transform ViewPlainCenter;

   public List<Texture2DArray> cache;

   public Vector2 InitialPosition;

   private SegmentCache cache1;

   private Vector3 cameraPosOffset;

   private Vector3Int LFUCoor;

   private const float fixedAreaLength = 9.5f;
   private const float SegmentLength = 1.9f;

   private Vector2 _LF_GlobalPosition;
   private Vector2 LF_GlobalPosition
   {
      get { return _LF_GlobalPosition; }
      set
      {
         _LF_GlobalPosition = value;

         Vector3Int tempLFUCoor = Vector3Int.zero;
         tempLFUCoor.x = Mathf.FloorToInt(_LF_GlobalPosition.x / SegmentLength);
         tempLFUCoor.y = Mathf.FloorToInt(_LF_GlobalPosition.y / SegmentLength);

         Vector2 lfuLocalPosition;
         lfuLocalPosition.x = _LF_GlobalPosition.x - tempLFUCoor.x * SegmentLength;
         lfuLocalPosition.y = _LF_GlobalPosition.y - tempLFUCoor.y * SegmentLength;

         front_plain.SetFloat(Distance_property, SegmentLength - lfuLocalPosition.y);
         front_plain.SetFloat(HorizontalPositon_property, _LF_GlobalPosition.x);

         back_plain.SetFloat(Distance_property, lfuLocalPosition.y);
         back_plain.SetFloat(HorizontalPositon_property, fixedAreaLength - _LF_GlobalPosition.x);

         right_plain.SetFloat(Distance_property, SegmentLength - lfuLocalPosition.x);
         right_plain.SetFloat(HorizontalPositon_property, fixedAreaLength - _LF_GlobalPosition.y);

         left_plain.SetFloat(Distance_property, lfuLocalPosition.x);
         left_plain.SetFloat(HorizontalPositon_property, _LF_GlobalPosition.y);

         if (lfuLocalPosition.y > lfuLocalPosition.x)
            tempLFUCoor.z = 0;
         else
            tempLFUCoor.z = 1;

         if (lfuLocalPosition.y < -lfuLocalPosition.x + 1)
            tempLFUCoor.z += 2;

         if (tempLFUCoor != LFUCoor)
         {
            LFUCoor = tempLFUCoor;

            int tempX = LFUCoor.x;
            int tempY = LFUCoor.y;


            front_plain.SetFloat(PositionDelta_property, fixedAreaLength / imageDepth[0, 3 - LFUCoor.y]);
            right_plain.SetFloat(PositionDelta_property, fixedAreaLength / imageDepth[1, 3 - LFUCoor.x]);
            left_plain.SetFloat(PositionDelta_property, fixedAreaLength / imageDepth[2, LFUCoor.x - 1]);
            back_plain.SetFloat(PositionDelta_property, fixedAreaLength / imageDepth[3, LFUCoor.y - 1]);
            for (int i = 0; i < 4; i++)
            {
               cache1.LoadSegment(tempX, tempY, i);
            }
            back_plain.SetTexture(Texture_property, cache1.GetSegment(tempX, tempY, 3));
            left_plain.SetTexture(Texture_property, cache1.GetSegment(tempX, tempY, 2));
            front_plain.SetTexture(Texture_property, cache1.GetSegment(tempX, tempY, 0));
            right_plain.SetTexture(Texture_property, cache1.GetSegment(tempX, tempY, 1));



            if (LFUCoor.z == 0)
            {
               for (int i = 0; i < 4; i++)
               {
                  cache1.LoadSegment(tempX, tempY + 1, i);
               }

               cache1.LoadSegment(tempX - 1, tempY + 1, 0);
               cache1.LoadSegment(tempX + 1, tempY + 1, 0);
               cache1.LoadSegment(tempX - 1, tempY, 0);
               cache1.LoadSegment(tempX + 1, tempY, 0);
               cache1.LoadSegment(tempX - 1, tempY, 3);
               cache1.LoadSegment(tempX + 1, tempY, 3);
               cache1.LoadSegment(tempX, tempY - 1, 1);
               cache1.LoadSegment(tempX, tempY - 1, 2);

               back_plain.SetTexture(Texture_property_L, cache1.GetSegment(tempX + 1, tempY, 3));
               back_plain.SetTexture(Texture_property_R, cache1.GetSegment(tempX - 1, tempY, 3));
               right_plain.SetTexture(Texture_property_L, cache1.GetSegment(tempX, tempY + 1, 1));
               left_plain.SetTexture(Texture_property_R, cache1.GetSegment(tempX, tempY + 1, 2));
            }
            if (LFUCoor.z == 1)
            {
               for (int i = 0; i < 4; i++)
               {
                  cache1.LoadSegment(tempX + 1, tempY, i);
               }

               cache1.LoadSegment(tempX + 1, tempY - 1, 1);
               cache1.LoadSegment(tempX + 1, tempY + 1, 1);
               cache1.LoadSegment(tempX, tempY - 1, 1);
               cache1.LoadSegment(tempX, tempY + 1, 1);
               cache1.LoadSegment(tempX, tempY - 1, 2);
               cache1.LoadSegment(tempX, tempY + 1, 2);
               cache1.LoadSegment(tempX - 1, tempY, 0);
               cache1.LoadSegment(tempX - 1, tempY, 3);

               left_plain.SetTexture(Texture_property_R, cache1.GetSegment(tempX, tempY + 1, 2));
               left_plain.SetTexture(Texture_property_L, cache1.GetSegment(tempX, tempY - 1, 2));
               front_plain.SetTexture(Texture_property_L, cache1.GetSegment(tempX + 1, tempY, 0));
               back_plain.SetTexture(Texture_property_R, cache1.GetSegment(tempX + 1, tempY, 3));
            }
            if (LFUCoor.z == 2)
            {
               for (int i = 0; i < 4; i++)
               {
                  cache1.LoadSegment(tempX - 1, tempY, i);
               }

               cache1.LoadSegment(tempX - 1, tempY + 1, 2);
               cache1.LoadSegment(tempX - 1, tempY - 1, 2);
               cache1.LoadSegment(tempX, tempY - 1, 1);
               cache1.LoadSegment(tempX, tempY + 1, 1);
               cache1.LoadSegment(tempX, tempY - 1, 2);
               cache1.LoadSegment(tempX, tempY + 1, 2);
               cache1.LoadSegment(tempX + 1, tempY, 0);
               cache1.LoadSegment(tempX + 1, tempY, 3);

               right_plain.SetTexture(Texture_property_L, cache1.GetSegment(tempX, tempY + 1, 1));
               right_plain.SetTexture(Texture_property_R, cache1.GetSegment(tempX, tempY - 1, 1));
               front_plain.SetTexture(Texture_property_L, cache1.GetSegment(tempX - 1, tempY, 0));
               back_plain.SetTexture(Texture_property_R, cache1.GetSegment(tempX - 1, tempY, 3));
            }
            if (LFUCoor.z == 3)
            {
               for (int i = 0; i < 4; i++)
               {
                  cache1.LoadSegment(tempX, tempY - 1, i);
               }

               cache1.LoadSegment(tempX - 1, tempY - 1, 3);
               cache1.LoadSegment(tempX + 1, tempY - 1, 3);
               cache1.LoadSegment(tempX - 1, tempY, 0);
               cache1.LoadSegment(tempX + 1, tempY, 0);
               cache1.LoadSegment(tempX - 1, tempY, 3);
               cache1.LoadSegment(tempX + 1, tempY, 3);
               cache1.LoadSegment(tempX, tempY + 1, 1);
               cache1.LoadSegment(tempX, tempY + 1, 2);

               front_plain.SetTexture(Texture_property_L, cache1.GetSegment(tempX - 1, tempY, 0));
               front_plain.SetTexture(Texture_property_R, cache1.GetSegment(tempX + 1, tempY, 0));
               right_plain.SetTexture(Texture_property_L, cache1.GetSegment(tempX, tempY - 1, 1));
               left_plain.SetTexture(Texture_property_R, cache1.GetSegment(tempX, tempY - 1, 2));
            }

         }
      }
   }

   //쉐이더에서 사용할 프로퍼티 변수를 ID형태로 저장 합니다.
   private int Distance_property;
   private int HorizontalPositon_property;
   private int Texture_property;
   private int Texture_property_R;
   private int Texture_property_L;
   private int PositionDelta_property;

   private float[,] imageDepth;

   void Start()
   {
      // Front[0] = Resources.Load("Back_0") as Texture2DArray;
      // Debug.Log(sw.ElapsedMilliseconds.ToString());
      // Front[1] = Resources.Load("Back_1") as Texture2DArray;
      // Debug.Log(sw.ElapsedMilliseconds.ToString());
      // Front[2] = Resources.Load("Back_2") as Texture2DArray;

      cache = new List<Texture2DArray>();
      cache1 = new SegmentCache(16);

      cameraPosOffset = cameraTransform.position;
      cameraTransform.position = new Vector3(InitialPosition.x, 0f, InitialPosition.y);

      ViewPlainCenter.rotation = Quaternion.Euler(0, cameraTransform.rotation.eulerAngles.y, 0);

      // [0,X] : front, [1,X] : right, [2,X] : left, [3,X] : back
      imageDepth = new float[4, 3]{ {662f, 660f, 657f},
                                      {640f, 645f, 646f},
                                      {624f, 633f, 639f},
                                      {653f, 663f, 660f} };


      //프로퍼티 이름으로 부터 ID를 추출 합니다.
      //프로퍼티 이름은 해당 쉐이더를 클릭하여 나오는 오른쪽 Inspector창에서 확인해야 합니다.
      Distance_property = Shader.PropertyToID("Vector1_A1C90C0E");
      HorizontalPositon_property = Shader.PropertyToID("Vector1_DF5BD731");
      Texture_property = Shader.PropertyToID("Texture2DArray_F092712C");
      Texture_property_L = Shader.PropertyToID("Texture2DArray_5C3CC0FC");
      Texture_property_R = Shader.PropertyToID("Texture2DArray_4CAF86FE");
      PositionDelta_property = Shader.PropertyToID("Vector1_14AEA0F9");

      //front_plain.SetTexture(Texture_property, Front[1]);
      //front_plain.SetTexture(Texture_property_L, Front[0]);
      //front_plain.SetTexture(Texture_property_R, Front[2]);
   }

   void Update()
   {
      ViewPlainCenter.position = cameraTransform.position;

      LF_GlobalPosition = new Vector2(cameraTransform.position.x - cameraPosOffset.x, cameraTransform.position.z - cameraPosOffset.z);



      if (Input.GetKeyDown(KeyCode.A))
      {
         LoadSegment1();
      }
      if (Input.GetKeyDown(KeyCode.S))
      {

         LoadSegment2();

      }
      if (Input.GetKeyDown(KeyCode.D))
      {
         LoadSegment3();

      }
   }

   private void LoadSegment1()
   {
      Debug.Log("StartLoad");
      System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
      sw.Start();
      cache.Add(Resources.Load<Texture2DArray>("BackTest_1"));
      Debug.Log("Segment Load Time : " + sw.ElapsedMilliseconds.ToString() + "ms/segment");
      sw.Reset();
      sw.Start();
      front_plain.SetTexture(Texture_property_L, cache[0]);
      Debug.Log("Segment Apply Time : " + sw.ElapsedMilliseconds.ToString() + "ms/segment");
      sw.Stop();
   }
   private void LoadSegment2()
   {
      Debug.Log("StartLoad");
      System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
      sw.Start();
      cache.Add(Resources.Load<Texture2DArray>("BackTest_2"));
      Debug.Log("Segment Load Time : " + sw.ElapsedMilliseconds.ToString() + "ms/segment");
      sw.Reset();
      sw.Start();
      front_plain.SetTexture(Texture_property, cache[1]);
      Debug.Log("Segment Apply Time : " + sw.ElapsedMilliseconds.ToString() + "ms/segment");
      sw.Stop();
   }
   private void LoadSegment3()
   {
      Debug.Log("StartLoad");
      System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
      sw.Start();
      cache.Add(Resources.Load<Texture2DArray>("BackTest_3"));
      Debug.Log("Segment Load Time : " + sw.ElapsedMilliseconds.ToString() + "ms/segment");
      sw.Reset();
      sw.Start();
      front_plain.SetTexture(Texture_property_R, cache[2]);
      Debug.Log("Segment Apply Time : " + sw.ElapsedMilliseconds.ToString() + "ms/segment");
      sw.Stop();
   }

   private void OnDestroy()
   {
      front_plain.SetTexture(Texture_property_R, null);
      front_plain.SetTexture(Texture_property_L, null);
      front_plain.SetTexture(Texture_property, null);
      right_plain.SetTexture(Texture_property_R, null);
      right_plain.SetTexture(Texture_property_L, null);
      right_plain.SetTexture(Texture_property, null);
      left_plain.SetTexture(Texture_property_R, null);
      left_plain.SetTexture(Texture_property_L, null);
      left_plain.SetTexture(Texture_property, null);
      back_plain.SetTexture(Texture_property_R, null);
      back_plain.SetTexture(Texture_property_L, null);
      back_plain.SetTexture(Texture_property, null);
   }
}
