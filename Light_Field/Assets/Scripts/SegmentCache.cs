using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SegmentCache
{
   private Vector3Int segmentcoor;

   private Texture2DArray[] cache;
   private Dictionary<Vector3Int, int> cacheDictionary;
   private int cacheSize;
   public SegmentCache(int count)
   {
      cacheSize = count;
      cache = new Texture2DArray[count];
      cacheDictionary = new Dictionary<Vector3Int, int>(count);
   }

   public void LoadSegment(Vector3Int seg)
   {
      if (checkcacheHit(seg))
         return;

      StringBuilder sb = new StringBuilder("Segment_");
      sb.Append(seg.x.ToString());
      sb.Append(seg.y.ToString());
      sb.Append(seg.z.ToString());

      if (cacheDictionary.Count >= cacheSize)
      {
         RemoveSegment();
      }

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

   public Texture2DArray GetSegment(Vector3Int seg)
   {
      if (!checkcacheHit(seg))
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

   private bool checkcacheHit(Vector3Int seg)
   {
      if (cacheDictionary.ContainsKey(seg))
         return true;
      else
         return false;
   }

   private void RemoveSegment()
   {

   }
}
