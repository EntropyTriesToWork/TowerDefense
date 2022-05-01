using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance
    {
        get
        {
            return Instance;
        }
        set => Instance = value;
    }
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {

    }
    void Update()
    {

    }
}
