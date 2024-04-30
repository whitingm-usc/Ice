using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Serializable]
    public class ControlValues
    {
        public float mTurnRate = 720.0f;
        public float mAccel = 1.0f;
        public float mDrag = 0.1f;
        public float mMaxSpeed = 5.0f;
    }
    public ControlValues mGroundControl;
    public ControlValues mIceControl;
    float mAng;
    Vector3 mVelocity;
    int mIsOnIce = 0;

    // Start is called before the first frame update
    void Start()
    {
        mAng = transform.localEulerAngles.y;
        mVelocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            move.z += 1.0f;
        if (Input.GetKey(KeyCode.S))
            move.z -= 1.0f;
        if (Input.GetKey(KeyCode.A))
            move.x -= 1.0f;
        if (Input.GetKey(KeyCode.D))
            move.x += 1.0f;

        //TODO camera-relative controls

        float targetAng = mAng;
        ControlValues control = mGroundControl;
        if (mIsOnIce > 0)
        {   // ICE MODE
            control = mIceControl;
        }

        // Turn to face the direction of input
        if (move.magnitude > 0.0f)
        {
            targetAng = Mathf.Rad2Deg * Mathf.Atan2(move.x, move.z);
        }

        // accelerate in the direction of input
        mVelocity += move * control.mAccel * Time.deltaTime;
        Vector3 drag = -mVelocity;
        float dragLen = drag.magnitude;
        if (dragLen > 0.01f)
        {
            float dragMax = control.mDrag * Time.deltaTime;
            drag = drag / dragLen;
            dragLen = Mathf.Clamp(dragLen, -dragMax, dragMax);
            drag = dragLen * drag;
            mVelocity += drag;
            float velLen = mVelocity.magnitude;
            if (velLen > control.mMaxSpeed)
            {
                mVelocity = mVelocity.normalized * control.mMaxSpeed;
            }
        }
        else
        {
            mVelocity = Vector3.zero;
        }

        float angDiff = targetAng - mAng;
        if (angDiff < -180.0f)
            angDiff += 360.0f;
        if (angDiff > 180.0f)
            angDiff -= 360.0f;
        float angSpdMax = control.mTurnRate * Time.deltaTime;
        angDiff = Mathf.Clamp(angDiff, -angSpdMax, angSpdMax);
        mAng = mAng + angDiff;
        if (mAng < -180.0f)
            mAng += 360.0f;
        if (mAng > 180.0f)
            mAng -= 360.0f;

        Vector3 pos = transform.localPosition;
        pos += mVelocity * Time.deltaTime;

        transform.localEulerAngles = new Vector3(0.0f, mAng, 0.0f);
        transform.localPosition = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        mIsOnIce++;
    }

    private void OnTriggerExit(Collider other)
    {
        mIsOnIce--;
    }
}
