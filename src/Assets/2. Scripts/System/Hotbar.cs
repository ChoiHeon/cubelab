using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotbar : MonoBehaviour
{
    public int selected = 1;
    public int slotSize;


    public GameObject emptyImage;
    public List<GameObject> AllSlot;
    public GameObject selectedFrame;
    public GameObject Brush;
    private RectTransform selectedFramePos;

    private Dictionary<KeyCode, int> keyDictionary;
   
    private int posX(int i)
    {
        return (i - (int)(this.slotSize/2))*100;
    }


    // Start is called before the first frame update
    void Start()
    {
        selectedFramePos = selectedFrame.GetComponent<RectTransform>();

        keyDictionary = new Dictionary<KeyCode, int>
        {
            { KeyCode.Alpha1, 1 },
            { KeyCode.Alpha2, 2 },
            { KeyCode.Alpha3, 3 },
            { KeyCode.Alpha4, 4 },
            { KeyCode.Alpha5, 5 },
            { KeyCode.Alpha6, 6 },
            { KeyCode.Alpha7, 7 },
            { KeyCode.Alpha8, 8 },
            { KeyCode.Alpha9, 9 }            
        };

        for(int i=0; i<slotSize; i++)
        {
            GameObject slot = Instantiate(AllSlot[i], new Vector3(posX(i), 0, 0), Quaternion.identity);
            slot.transform.SetParent(this.GetComponent<Transform>(), false);
        }

        //selectHotBar(1);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown)
        {
            foreach (var dic in keyDictionary)
            {
                if(Input.GetKeyDown(dic.Key))
                {
                    selected = dic.Value-1;
                    selectHotBar(dic.Value-1);                   
                }
            }
        }
    }

    void selectHotBar(int index)
    {
        Brush.GetComponent<BrushControl>().selectedIndex = index;
        Brush.GetComponent<BrushControl>().brushOff();
        selectedFramePos.localPosition = new Vector3(posX(index), 0, 0);
    }

}
