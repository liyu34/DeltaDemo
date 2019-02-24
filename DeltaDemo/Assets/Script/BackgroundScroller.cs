using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    // 地图移动速度
    public float scrollSpeed = 1;
    // 一块地图的长度
    public float widthSize = 40;

    private Vector3 startPosition;
    private float lastPositionOffset = 0f;
    
    void Start()
    {
		startPosition = transform.position;
    }

    void Update()
    {
        if (Main.instance.isPaused)
        {
            return;
        }
        float newPositionOffset = Mathf.Repeat(Time.deltaTime * scrollSpeed + lastPositionOffset, widthSize);
        transform.position = startPosition + Vector3.right * newPositionOffset;
        lastPositionOffset = newPositionOffset;
    }

    public void SetMaterial(Material material)
    {
        GetComponent<MeshRenderer>().material = material;
    }
}
