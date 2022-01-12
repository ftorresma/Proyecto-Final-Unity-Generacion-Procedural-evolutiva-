using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float HorizontalMove;
    public float VerticalMove;
    public CharacterController Player;
    public GameObject self;
    public GeneracionMapa GeneracionMapa;

    int[] final;

    public float playerSpeed;

    private Vector3 posicion;

    bool entro;

    int width;

    int height;

    void Start()
    {
        Player = GetComponent<CharacterController>();
        final = new int[2];
        entro = false;

        width = GeneracionMapa.tam()[0];
        height = GeneracionMapa.tam()[1];
    }

    void Update()
    {
        if (Input.GetKeyUp("r"))
        {
            self.transform.position = new Vector3(self.transform.position.x, 10f, self.transform.position.z);
        }

        HorizontalMove = Input.GetAxis("Horizontal");
        VerticalMove = Input.GetAxis("Vertical");


        posicion = transform.position;
        if (posicion.x < 0 || posicion.x >= width || posicion.z < 0 || posicion.z >= height || posicion.y >= 0.5)
        {
            Debug.Log("entro"+posicion.x.ToString()+" "+posicion.y.ToString());
            int[] pair = new int[2];
            pair = GeneracionMapa.arregla();
            
            Player.enabled = false;
            self.transform.position = new Vector3(pair[0], 0, pair[1]);
            self.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            Player.enabled = true;
        }
        

        if (entro==false)
        {
            entro = GeneracionMapa.Llego();
        }
        else if (entro == true)
        {
            entro = GeneracionMapa.Inicio();
            if (entro == false)
            {
                width = GeneracionMapa.tam()[0];
                height = GeneracionMapa.tam()[1];
            }
        }



        /*
        if (posicion.y != 0)
        {
            Player.enabled = false;
            self.transform.position = new Vector3(self.transform.position.x, 0, self.transform.position.z);
            Player.enabled = true;
        }*/
    }

    private void LateUpdate()
    {
        Player.Move(new Vector3(HorizontalMove, 0.0f, VerticalMove) * playerSpeed * Time.deltaTime);
    }

    /*
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "Llegada(clone)")
        {
            Debug.Log("Do something here");
        }
        //Debug.Log(collision.collider.name);
    }*/
}