using System.Collections;
using System.Collections.Generic;
using Karpik.Movement.Platformer2D;
using UnityEngine;


[RequireComponent(typeof(Movement2D))]
public class PlayerInputMovement : MonoBehaviour
{
    private Movement2D _movement;

    private Vector2 _movementDirection;

    private void Awake()
    {
        _movement = GetComponent<Movement2D>();
    }

    private void Update()
    {
        _movementDirection = Vector2.zero;
        if (Input.GetKey(KeyCode.D))
        {
            _movementDirection.x += 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            _movementDirection.x -= 1;
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
        {
            _movement.Jump();
        }
    }

    private void FixedUpdate()
    {
        _movement.Move(_movementDirection);
    }
}

