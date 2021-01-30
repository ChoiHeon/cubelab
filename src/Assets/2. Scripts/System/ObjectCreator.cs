using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Globalization;

public static class ObjectCreator
{
    private static TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

    /// <summary>
    /// 오브젝트를 최상위 하이라키에 생성 및 반환
    /// </summary>
    /// <param name="objectType"></param>
    public static GameObject CreateObject(ObjectType objectType)
    {
        GameObject obj = (GameObject)Object.Instantiate(Resources.Load("Prefabs/" + ti.ToTitleCase(objectType.ToString())));

        return obj;
    }

    /// <summary>
    /// 오브젝트를 인자로 받은 부모 오브젝트의 자식으로 생성 및 반환
    /// </summary>
    /// <param name="objectType"></param>
    /// <param name="parent"></param>
    public static GameObject CreateObject(ObjectType objectType, GameObject parent)
    {
        GameObject obj = CreateObject(objectType);

        // 인자로 받은 부모 오브젝트의 자식을 설정
        obj.transform.SetParent(parent.transform);

        return obj;
    }
}
