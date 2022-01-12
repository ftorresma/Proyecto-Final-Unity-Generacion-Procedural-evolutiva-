using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class visited : MonoBehaviour
{
    public Material material;
    public bool pintado = false;
    GameObject generacionMapa;
    GeneracionMapa GeneracionMapa;
    Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        generacionMapa = GameObject.Find("Generador");
        GeneracionMapa = generacionMapa.GetComponent<GeneracionMapa>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            try
            {
                if (!pintado)
                {
                    rend.sharedMaterial = material;
                    //Debug.Log("Do something here");
                    GeneracionMapa.ExploUser();
                    pintado = true;
                }
            }
            catch (Exception e)
            {
                //print("error de pintado - ignorar");
            }
        }
    }
}
