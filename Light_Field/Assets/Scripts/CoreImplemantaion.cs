using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class CoreImplemantaion : MonoBehaviour
{
    public Material front_plain, right_plain, back_plain, left_plain;
    public Transform cameraTransform;
    public Transform ViewPlainCenter;

    public float miniMapSize;
    public RectTransform viewPointSprite_Global;
    public RectTransform viewPointSprite_Local;
    public Text LFUCoorText;
    private char[] indexToText = { 'F', 'R', 'L', 'B' };
    public Text[] cacheAllocationText;
    public Vector2 InitialPosition;

    public Texture2DArray blackTexture;

    private SegmentCache cache;

    private const float fixedAreaLength = 9.5f;
    private const float SegmentLength = fixedAreaLength * 0.2f;

    private Vector3 cameraPosOffset;
    private Vector3Int LFUCoor;

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
            lfuLocalPosition.x = _LF_GlobalPosition.x % SegmentLength;
            lfuLocalPosition.y = _LF_GlobalPosition.y % SegmentLength;

            viewPointSprite_Local.anchoredPosition = lfuLocalPosition * miniMapSize / SegmentLength;

            front_plain.SetFloat(Distance_property, SegmentLength - lfuLocalPosition.y);
            front_plain.SetFloat(HorizontalPositon_property, lfuLocalPosition.x);

            back_plain.SetFloat(Distance_property, lfuLocalPosition.y);
            back_plain.SetFloat(HorizontalPositon_property, SegmentLength - lfuLocalPosition.x);

            right_plain.SetFloat(Distance_property, SegmentLength - lfuLocalPosition.x);
            right_plain.SetFloat(HorizontalPositon_property, SegmentLength - lfuLocalPosition.y);

            left_plain.SetFloat(Distance_property, lfuLocalPosition.x);
            left_plain.SetFloat(HorizontalPositon_property, lfuLocalPosition.y);

            if (lfuLocalPosition.y > lfuLocalPosition.x)
                tempLFUCoor.z = 0;
            else
                tempLFUCoor.z = 1;

            if (lfuLocalPosition.y < -lfuLocalPosition.x + SegmentLength)
                tempLFUCoor.z += 2;

            if (tempLFUCoor != LFUCoor)
            {

                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                LFUCoor = tempLFUCoor;
                LFUCoorText.text = "(" + LFUCoor.x + "," + LFUCoor.y + "," + indexToText[LFUCoor.z] + ")";
                //Debug.Log(LFUCoor);
                int tempX = LFUCoor.x;
                int tempY = LFUCoor.y;

                List<Vector3Int> loadSegs = new List<Vector3Int>();

                front_plain.SetFloat(PositionDelta_property, fixedAreaLength / imageDepth[0, 3 - LFUCoor.y]);
                right_plain.SetFloat(PositionDelta_property, fixedAreaLength / imageDepth[1, 3 - LFUCoor.x]);
                left_plain.SetFloat(PositionDelta_property, fixedAreaLength / imageDepth[2, LFUCoor.x - 1]);
                back_plain.SetFloat(PositionDelta_property, fixedAreaLength / imageDepth[3, LFUCoor.y - 1]);

                for (int i = 0; i < 4; i++)
                {
                    loadSegs.Add(new Vector3Int(tempX, tempY, i));
                }

                if (LFUCoor.z == 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        loadSegs.Add(new Vector3Int(tempX, tempY + 1, i));
                    }

                    loadSegs.Add(new Vector3Int(tempX - 1, tempY + 1, 0));
                    loadSegs.Add(new Vector3Int(tempX + 1, tempY + 1, 0));
                    loadSegs.Add(new Vector3Int(tempX - 1, tempY, 0));
                    loadSegs.Add(new Vector3Int(tempX + 1, tempY, 0));
                    loadSegs.Add(new Vector3Int(tempX - 1, tempY, 3));
                    loadSegs.Add(new Vector3Int(tempX + 1, tempY, 3));
                    loadSegs.Add(new Vector3Int(tempX, tempY - 1, 1));
                    loadSegs.Add(new Vector3Int(tempX, tempY - 1, 2));

                    cache.LoadSegments(loadSegs);

                    back_plain.SetTexture(Texture_property_L, cache.GetSegment(tempX + 1, tempY, 3));
                    back_plain.SetTexture(Texture_property_R, cache.GetSegment(tempX - 1, tempY, 3));
                    right_plain.SetTexture(Texture_property_L, cache.GetSegment(tempX, tempY + 1, 1));
                    left_plain.SetTexture(Texture_property_R, cache.GetSegment(tempX, tempY + 1, 2));
                    front_plain.SetTexture(Texture_property_L, blackTexture);
                    front_plain.SetTexture(Texture_property_R, blackTexture);
                    right_plain.SetTexture(Texture_property_R, blackTexture);
                    left_plain.SetTexture(Texture_property_L, blackTexture);
                }
                else if (LFUCoor.z == 1)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        loadSegs.Add(new Vector3Int(tempX + 1, tempY, i));
                    }

                    loadSegs.Add(new Vector3Int(tempX + 1, tempY - 1, 1));
                    loadSegs.Add(new Vector3Int(tempX + 1, tempY + 1, 1));
                    loadSegs.Add(new Vector3Int(tempX, tempY - 1, 1));
                    loadSegs.Add(new Vector3Int(tempX, tempY + 1, 1));
                    loadSegs.Add(new Vector3Int(tempX, tempY - 1, 2));
                    loadSegs.Add(new Vector3Int(tempX, tempY + 1, 2));
                    loadSegs.Add(new Vector3Int(tempX - 1, tempY, 0));
                    loadSegs.Add(new Vector3Int(tempX - 1, tempY, 3));

                    cache.LoadSegments(loadSegs);

                    left_plain.SetTexture(Texture_property_R, cache.GetSegment(tempX, tempY + 1, 2));
                    left_plain.SetTexture(Texture_property_L, cache.GetSegment(tempX, tempY - 1, 2));
                    front_plain.SetTexture(Texture_property_R, cache.GetSegment(tempX + 1, tempY, 0));
                    back_plain.SetTexture(Texture_property_L, cache.GetSegment(tempX + 1, tempY, 3));
                    right_plain.SetTexture(Texture_property_L, blackTexture);
                    right_plain.SetTexture(Texture_property_R, blackTexture);
                    back_plain.SetTexture(Texture_property_R, blackTexture);
                    front_plain.SetTexture(Texture_property_L, blackTexture);
                }
                else if (LFUCoor.z == 2)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        loadSegs.Add(new Vector3Int(tempX - 1, tempY, i));
                    }

                    loadSegs.Add(new Vector3Int(tempX - 1, tempY + 1, 2));
                    loadSegs.Add(new Vector3Int(tempX - 1, tempY - 1, 2));
                    loadSegs.Add(new Vector3Int(tempX, tempY - 1, 1));
                    loadSegs.Add(new Vector3Int(tempX, tempY + 1, 1));
                    loadSegs.Add(new Vector3Int(tempX, tempY - 1, 2));
                    loadSegs.Add(new Vector3Int(tempX, tempY + 1, 2));
                    loadSegs.Add(new Vector3Int(tempX + 1, tempY, 0));
                    loadSegs.Add(new Vector3Int(tempX + 1, tempY, 3));

                    cache.LoadSegments(loadSegs);

                    right_plain.SetTexture(Texture_property_L, cache.GetSegment(tempX, tempY + 1, 1));
                    right_plain.SetTexture(Texture_property_R, cache.GetSegment(tempX, tempY - 1, 1));
                    front_plain.SetTexture(Texture_property_L, cache.GetSegment(tempX - 1, tempY, 0));
                    back_plain.SetTexture(Texture_property_R, cache.GetSegment(tempX - 1, tempY, 3));
                    left_plain.SetTexture(Texture_property_L, blackTexture);
                    left_plain.SetTexture(Texture_property_R, blackTexture);
                    front_plain.SetTexture(Texture_property_R, blackTexture);
                    back_plain.SetTexture(Texture_property_L, blackTexture);
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        loadSegs.Add(new Vector3Int(tempX, tempY - 1, i));
                    }

                    loadSegs.Add(new Vector3Int(tempX - 1, tempY - 1, 3));
                    loadSegs.Add(new Vector3Int(tempX + 1, tempY - 1, 3));
                    loadSegs.Add(new Vector3Int(tempX - 1, tempY, 0));
                    loadSegs.Add(new Vector3Int(tempX + 1, tempY, 0));
                    loadSegs.Add(new Vector3Int(tempX - 1, tempY, 3));
                    loadSegs.Add(new Vector3Int(tempX + 1, tempY, 3));
                    loadSegs.Add(new Vector3Int(tempX, tempY + 1, 1));
                    loadSegs.Add(new Vector3Int(tempX, tempY + 1, 2));

                    cache.LoadSegments(loadSegs);

                    front_plain.SetTexture(Texture_property_L, cache.GetSegment(tempX - 1, tempY, 0));
                    front_plain.SetTexture(Texture_property_R, cache.GetSegment(tempX + 1, tempY, 0));
                    right_plain.SetTexture(Texture_property_R, cache.GetSegment(tempX, tempY - 1, 1));
                    left_plain.SetTexture(Texture_property_L, cache.GetSegment(tempX, tempY - 1, 2));
                    back_plain.SetTexture(Texture_property_L, blackTexture);
                    back_plain.SetTexture(Texture_property_R, blackTexture);
                    left_plain.SetTexture(Texture_property_R, blackTexture);
                    right_plain.SetTexture(Texture_property_L, blackTexture);
                }

                back_plain.SetTexture(Texture_property, cache.GetSegment(tempX, tempY, 3));
                left_plain.SetTexture(Texture_property, cache.GetSegment(tempX, tempY, 2));
                front_plain.SetTexture(Texture_property, cache.GetSegment(tempX, tempY, 0));
                right_plain.SetTexture(Texture_property, cache.GetSegment(tempX, tempY, 1));
                cache.ShowCacheAllocation(cacheAllocationText);

                sw.Stop();
                Debug.Log("Segment Load시간 : " + sw.ElapsedMilliseconds + " ms");
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

        cache = new SegmentCache(16);

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

        viewPointSprite_Global.anchoredPosition = _LF_GlobalPosition * miniMapSize / fixedAreaLength;
        viewPointSprite_Global.rotation = Quaternion.Euler(0f, 0f, -cameraTransform.rotation.eulerAngles.y);

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
