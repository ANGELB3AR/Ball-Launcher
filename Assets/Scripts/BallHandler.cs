using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallHandler : MonoBehaviour
{
    [SerializeField] GameObject ballPrefab;
    [SerializeField] Rigidbody2D pivot;
    [SerializeField] float detachDelay;
    [SerializeField] float respawnDelay;

    private Rigidbody2D currentBallRigidbody;
    private SpringJoint2D currentBallSpringJoint;

    private Camera mainCamera;
    private bool isDragging;

    void Start()
    {
        mainCamera = Camera.main;

        SpawnNewBall();
    }

    void Update()
    {
        if (currentBallRigidbody == null) { return; }

        if (!Touchscreen.current.primaryTouch.press.isPressed) 
        {
            if (isDragging)
            {
                LaunchBall();
            }

            isDragging = false;

            return; 
        }

        isDragging = true;

        currentBallRigidbody.isKinematic = true;

        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);

        currentBallRigidbody.position = worldPosition;
    }

    private void LaunchBall()
    {
        currentBallRigidbody.isKinematic = false;
        currentBallRigidbody = null;

        Invoke(nameof(DetachBall), detachDelay);
    }

    private void DetachBall()
    {
        currentBallSpringJoint.enabled = false;
        currentBallSpringJoint = null;

        Invoke(nameof(SpawnNewBall), respawnDelay);
    }

    private void SpawnNewBall()
    {
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);

        currentBallRigidbody = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSpringJoint = ballInstance.GetComponent<SpringJoint2D>();

        currentBallSpringJoint.connectedBody = pivot;
    }
}
