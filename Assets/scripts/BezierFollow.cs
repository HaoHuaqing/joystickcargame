using UnityEngine;
using System.Collections;
[RequireComponent(typeof(LineRenderer))]
public class BezierFollow : MonoBehaviour
{

    public LineRenderer lineRenderer;
    public GameObject[] CreatPoint;
    public Transform[] controlPoints;
    public Transform startPosition;
    public GameObject RandomPoint;
    Vector3 uppoint = new Vector3(0, 10, 0);
    private int curveCount = 0;
    private int layerOrder = -1;
    private int SEGMENT_COUNT = 100;
    private bool readpoint = true;

    void Start()
    {
        if (!lineRenderer)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        lineRenderer.sortingLayerID = layerOrder;
        curveCount = (int)controlPoints.Length / 2;//draw line every 3 points.0,1,2;2,3,4;


    }
    void Update()
    {
        if (readpoint)
        {
            for (int i = 1; i < 99; i = i + 2)
            {
                Vector3 RandX = new Vector3(Bezier.Rand[i], 0, 0);
                //Debug.Log(Bezier.Rand[i]);
                CreatPoint[i] = Instantiate(RandomPoint, startPosition.transform.position + i * uppoint + RandX, Quaternion.identity) as GameObject;
                CreatPoint[i + 1] = Instantiate(RandomPoint, startPosition.transform.position + (i + 1) * uppoint, Quaternion.identity) as GameObject;
                controlPoints[i] = CreatPoint[i].transform;
                controlPoints[i + 1] = CreatPoint[i + 1].transform;
            }
            readpoint = false;
        }

        DrawCurve();
    }
    void DrawCurve()
    {

        for (int j = 0; j < curveCount; j++)
        {
            for (int i = 1; i <= SEGMENT_COUNT; i++)
            {
                float t = i / (float)SEGMENT_COUNT;
                int nodeIndex = j * 2;
                Vector3 pixel = CalculateCubicBezierPoint(t, controlPoints[nodeIndex].position, controlPoints[nodeIndex + 1].position, controlPoints[nodeIndex + 2].position);
                lineRenderer.SetVertexCount(((j * SEGMENT_COUNT) + i));
                lineRenderer.SetPosition((j * SEGMENT_COUNT) + (i - 1), pixel);
            }

        }
    }

    Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;

        return p;
    }

}
