using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScrollTextureClass : MonoBehaviour
{
    
    public float ScrollSpeed = 1f;
    private Material _material;
    private GameObject parent;
    private Vector2 scrollPos;
    
    void Start()
    {
        _material = GetComponent<Image>().material;
        parent = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (parent.activeSelf)
        {
            scrollPos = _material.mainTextureOffset;
            scrollPos.y += ScrollSpeed * Time.deltaTime;
            _material.mainTextureOffset = scrollPos;
        }
    }
}
