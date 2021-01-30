using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Line : MonoBehaviour
{
    public GameObject[] vertices = new GameObject[2];

    [SerializeField] private Dictionary<GameObject, int> otherVertexIndex = new Dictionary<GameObject, int>();
    [SerializeField] public int vertexCnt = 0;
    private Vector3[] positions = new Vector3[2];

    // Start is called before the first frame update

    void update()
    {
        if(vertexCnt == 2)
        {
            positions[0] = vertices[0].transform.position;
            positions[1] = vertices[1].transform.position;

            transform.GetComponent<LineRenderer>().SetPositions(positions);
        }
    }



    /// <summary>
    /// Line 객체에 1개의 객체 저장합니다.
    /// </summary>
    /// <param name="obj">=저장할 객체</param>
    public void SetVertex(GameObject obj)
    {
        if (vertexCnt < 2)
        {
            vertices[vertexCnt] = obj;
            otherVertexIndex.Add(obj, -vertexCnt + 1);  // 0 -> 1, 1 -> 0
            vertexCnt++;
        }
        else
        {
            Debug.LogError(gameObject.name + ": 1개의 Line 객체에 3개 이상의 객체를 저장할 수 없습니다.");
        }
    }

    /// <summary>
    /// 인자로 받은 객체와 연결된 객체를 반환합니다.
    /// </summary>
    /// <param name="obj">반환받을 객체와 연결된 객체</param>
    /// <returns></returns>
    public GameObject GetOtherVertex(GameObject obj)
    {
        if (vertexCnt < 2 || (vertices[0] == null || vertices[1] == null))
            return null;

        return vertices[otherVertexIndex[obj]];
    }

    public int CompareObjectLevel()
    {
        if (vertexCnt < 2 || (vertices[0] == null || vertices[1] == null))
            return -1;

        int level1 = vertices[0].GetComponent<MyObject>().objectLevel;
        int level2 = vertices[1].GetComponent<MyObject>().objectLevel;

        if (level1 > level2)
            return 0;
        else if (level1 < level2)
            return 1;
        else
            return 2;
    }

    public void SetColor(Color color)
    {
        GetComponent<LineRenderer>().startColor = color;
        GetComponent<LineRenderer>().endColor = color;
    }
}
