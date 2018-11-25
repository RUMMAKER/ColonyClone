using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour {

    public Sprite forwardImg;
    public Sprite holdImg;
    public Sprite backImg;
    public Sprite chargeImg;

    public GameObject marinePrefab;

    public static ResourceManager singleton;
    private void Awake()
    {
        if (singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        singleton = this;
    }
}
