using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    public float speed = 1000.0f;
    private Vector3 _movement;
    [SerializeField] private GameObject _camera;
    private GameObject _theCam;
    
    public override void OnNetworkSpawn() {
        if (!IsOwner)
        {
            GetComponent<PlayerInput>().enabled = false;
            
        }
        else
        {
            _theCam = Instantiate(_camera, transform.position - Vector3.forward, Quaternion.identity);
        }
    }

    

    private void OnMove(InputValue input)
    {
        Vector2 inputVec = input.Get<Vector2>();
        AskToMoveServerRpc(inputVec);   
    }

    [ServerRpc]
    void AskToMoveServerRpc(Vector2 inputVector) {
        _movement = new Vector3(inputVector.x, 0, inputVector.y);
    }

    private void FixedUpdate()
    {
        if (!IsServer)
        {
            return;
        }
        
        var rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(_movement * Time.fixedDeltaTime * speed);
    }

    private void LateUpdate() {
        if (IsOwner)
        {
            var cameraOffset = new Vector3(0, 1f, -5f);
            _theCam.transform.rotation = Quaternion.identity;
            _theCam.transform.position = transform.position + cameraOffset;
        }
    }
}