using UnityEngine;

public class FakeInstantiate
{
    public static GameObject Instantiate(Transform parent, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null)
    {
        if (position == null) position = parent.position;
        if (scale == null) scale = Vector3.one;
        if (rotation == null) rotation = Quaternion.identity;
        GameObject obj = new GameObject();
        obj.transform.SetParent(parent);
        obj.transform.position = (Vector3)position;
        obj.transform.rotation = (Quaternion)rotation;
        obj.transform.localScale = (Vector3)scale;
        return obj;
    }
}
