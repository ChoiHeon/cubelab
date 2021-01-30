using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushControl : MonoBehaviour
{
    public GameObject[] brushes;
    public int selectedIndex = 0;
    public GameObject brush
    {
        get
        {
            return brushes[selectedIndex];
        }
    }
    private GameObject[] brush_instances;
  
    // Start is called before the first frame update
    void Start()
    {
        brush_instances = new GameObject[brushes.Length];
        for(int i=0; i<brushes.Length; i++)
        {
            brush_instances[i] = Instantiate(brushes[i], Vector3.zero, Quaternion.identity);
            brush_instances[i].transform.parent = transform;
        }        
        brushOff();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void brushOff()
    {
        foreach(GameObject obj in brush_instances)
        {
            obj.SetActive(false);
            //obj.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public void brushOn()
    {
        brush_instances[selectedIndex].SetActive(true);
        //brushes[selectedIndex].GetComponent<MeshRenderer>().enabled = true;
    }
}
