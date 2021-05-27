using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardKinematic : MonoBehaviour
{
    public ForwardKinematicSegamant original;
    public int count = 5;
    [Range(0.1f, 3.0f)] public float size = 1;
    [Range(0.1f, 3.0f)] public float length = 1;

    List<ForwardKinematicSegamant> segments = new List<ForwardKinematicSegamant>();

    private void Start()
    {
        KinematicSegment parent = null;
        for (int i = 0; i < count; i++)
        {
            var segement = Instantiate(original, transform);
            segement.Initialize(parent, transform.position, 0, length, 1);

            segments.Add(segement);

            parent = segement;
        }
    }
    void Update()
    {
        foreach (ForwardKinematicSegamant segment in segments)
        {
            segment.length = length;
            segment.size = size;
            if (segment.parent != null)
            {
                segment.start = segment.parent.end;
            }
            //segment.CalculateEnd();
        }
    }
}
