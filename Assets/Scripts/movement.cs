using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    public float HorizontalMove;
    public float VerticalMove;
    public CharacterController Player;
    public float playerSpeed;
    // Start is called before the first frame update
    void Start()
    {
        Player = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        HorizontalMove = Input.GetAxis("Horizontal");
        VerticalMove = Input.GetAxis("Vertical");
    }
    private void FixedUpdate()
    {
        Player.Move(new Vector3(HorizontalMove, 0.0f, VerticalMove) * playerSpeed);
    }
}
