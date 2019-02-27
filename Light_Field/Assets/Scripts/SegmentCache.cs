using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SegmentCache
{
    private Vector3Int segmentcoor;

    private Texture2DArray[] cache;
    private Dictionary<Vector3Int, int> cacheDictionary;
    private int cacheSize;
    private char[] indexToText = { 'F', 'R', 'L', 'B' };
    private int indexPointer = 0;

    public SegmentCache(int count)
    {
        cacheSize = count;
        cache = new Texture2DArray[count];
        cacheDictionary = new Dictionary<Vector3Int, int>(count);
    }

    public void LoadSegment(Vector3Int seg)
    {
        if (CheckcacheHit(seg))
            return;

        StringBuilder sb = new StringBuilder("Segment_");
        sb.Append(seg.x.ToString());
        sb.Append(seg.y.ToString());
        sb.Append(seg.z.ToString());

        for (int i = 0; i < cacheSize; i++)
        {
            if (cache[i] == null)
            {
                cache[i] = Resources.Load<Texture2DArray>(sb.ToString());
                if (cache[i] == null)
                    break;
                cacheDictionary.Add(seg, i);
                break;
            }
        }
    }

    public void LoadSegment(int x, int y, int side)
    {
        Vector3Int seg = new Vector3Int(x, y, side);
        LoadSegment(seg);
    }

    public void LoadSegments(List<Vector3Int> segs)
    {
        List<Vector3Int> loadingSegs = new List<Vector3Int>();
        List<Vector3Int> removingSegs = new List<Vector3Int>(cacheDictionary.Keys);

        for (int i = 0; i < segs.Count; i++)
        {
            if (CheckcacheHit(segs[i]))
            {
                removingSegs.Remove(segs[i]);
            }
            else
            {
                loadingSegs.Add(segs[i]);
            }
        }

        RemoveSegments(removingSegs);

        foreach (Vector3Int seg in loadingSegs)
        {
            LoadSegment(seg);
        }
    }

    public Texture2DArray GetSegment(Vector3Int seg)
    {
        if (!CheckcacheHit(seg))
        {
            LoadSegment(seg);
        }
        return cache[cacheDictionary[seg]];
    }

    public Texture2DArray GetSegment(int x, int y, int side)
    {
        Vector3Int seg = new Vector3Int(x, y, side);
        return GetSegment(seg);
    }

    private bool CheckcacheHit(Vector3Int seg)
    {
        if (cacheDictionary.ContainsKey(seg))
            return true;
        else
            return false;
    }

    private void RemoveSegment(Vector3Int seg)
    {
        cache[cacheDictionary[seg]] = null;
        cacheDictionary.Remove(seg);
        Resources.UnloadUnusedAssets();
    }

    private void RemoveSegments(List<Vector3Int> segs)
    {
        foreach (Vector3Int seg in segs)
        {
            RemoveSegment(seg);
        }
    }

    public void ShowCacheAllocation(Text[] tex)
    {
        List<Vector3Int> cacheCoor = new List<Vector3Int>(cacheDictionary.Keys);

        for (int i = 0; i < cacheSize; i++)
        {
            if (cache[i] == null)
            {
                tex[i].text = "";
            }
            else
            {
                tex[i].text = "(" + cacheCoor[i].x + "," + cacheCoor[i].y + "," + indexToText[cacheCoor[i].z] + ")";
                //tex[i].GetComponent<Animation>().Play();
            }
        }
    }
}
