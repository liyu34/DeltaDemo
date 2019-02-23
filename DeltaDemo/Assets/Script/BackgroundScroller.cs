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
    
    void Start()
    {
		startPosition = transform.position;
    }

    void Update()
    {
        //float newPosition = Mathf.Repeat(Time.time * scrollSpeed, widthSize);
        //transform.position = startPosition + Vector3.forward * newPosition;
    }
}
