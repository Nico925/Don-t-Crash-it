using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public float RotSpeed = 2.5F;
    public float minY = -90.0f;
    public float maxY = 90.0f;
    float RotLeftRight;
    float RotUpDown;
    public float MovementSpeed = 0.5f;
    bool canMoveFreeCam = true;
    bool _isMoveFreeCam;
    public bool isMoveFreeCam
    {
        get { return _isMoveFreeCam; }
        set
        {
            if (!canMoveFreeCam)
            {
                _isMoveFreeCam = false;
                return;
            }
            else
                _isMoveFreeCam = value;
        }
    }

    float startingFOV;

    public float ShakeMaxFrequence = 1;
    public float ShakeMaxForce = 1;
    public float ShakeFrequence = 1;
    public float ShakeForce = 1;
    public int ShakeVibrato = 1;

    GameObject origin;

    public void Init()
    {
        childCam = GetComponent<Camera>();

        startingFOV = childCam.fieldOfView;
        origin = new GameObject("CameraStartingPositon");
        origin.transform.position = transform.position;
        origin.transform.rotation = transform.rotation;

        origin.transform.SetParent(transform.parent);
        transform.SetParent(origin.transform);

        basicShake = origin.transform.DOPunchRotation(Vector3.forward * (ShakeForce + ShakeMaxForce * 0.1f), ShakeFrequence + ShakeMaxFrequence / 0.1f, ShakeVibrato).SetLoops(-1);
    }

    void Update()
    {
        if (isMoveFreeCam)
            RotateCamera();
    }

    Vector3 euler = new Vector3(0, 0, 0);
    void RotateCamera()
    {
        transform.localEulerAngles = euler;
        RotLeftRight = Input.GetAxis("Mouse X") * RotSpeed;
        RotUpDown = Input.GetAxis("Mouse Y") * RotSpeed;

        euler.y += RotLeftRight;
        euler.x -= RotUpDown;

        if (euler.x >= maxY)
            euler.x = maxY;
        if (euler.x <= minY)
            euler.x = minY;
    }
    Coroutine motionCoroutine;

    #region API
    /// <summary>
    /// Move the camera toward _target and rotate it as _target.forward
    /// </summary>
    /// <param name="_target"></param>
    public void FocusAt(Transform _target)
    {
        canMoveFreeCam = false;
        isMoveFreeCam = false;
        //transform.DORotate(_target.rotation.eulerAngles, MovementSpeed);
        ////transform.DORotateQuaternion(_target.rotation, MovementSpeed);
        //transform.DOMove(_target.position, MovementSpeed);
        //FocusAt(_target.position, _target.rotation);
        if (motionCoroutine != null)
            StopCoroutine(motionCoroutine);

        motionCoroutine = StartCoroutine(Move(_target));
    }
    ///// <summary>
    ///// Move the camera toward _targetPosition and rotate it as _forward
    ///// </summary>
    ///// <param name="_target"></param>
    //public void FocusAt(Vector3 _targetPosition, Quaternion _targetRotation)
    //{
    //    canMoveFreeCam = false;
    //    isMoveFreeCam = false;

    //    //transform.DORotate(_targetRotation.eulerAngles, MovementSpeed);
    //    //transform.DORotateQuaternion(originalRotation, MovementSpeed);
    //    //transform.DOMove(_targetPosition, MovementSpeed).OnComplete(()=> { if (_targetPosition == origin.transform.position) canMoveFreeCam = true; });
    //}

    /// <summary>
    /// Move the camera to her original position and orientation
    /// </summary>
    public void FocusReset()
    {
        FocusAt(origin.transform);
    }

    Camera childCam;
    Tween basicShake;
    Tween shake;
    Tween focusChange;
    public void Shake(TweenCallback callback)
    {
        shake = childCam.DOShakePosition(1f, .1f).OnComplete(callback);
    }
    public void AwarnessLook(TweenCallback _callback)
    {
        focusChange = childCam.DOFieldOfView(childCam.fieldOfView + 10, 0.5f).OnComplete(_callback);
        //focusChange.OnComplete(() =>
        //    {
        //        focusChange = childCam.DOFieldOfView(childCam.fieldOfView - 10, 1f);
        //    });
    }
    #endregion
    IEnumerator Move(Transform _transf)
    {
        childCam.fieldOfView = startingFOV;

        if (_transf == origin.transform)
            canMoveFreeCam = true;

        if (Vector3.Distance(transform.position, _transf.position) < Time.deltaTime / 10)
            yield break;

        if (shake == null || shake.IsPlaying())
            yield return null;

        basicShake.Kill(true);

        bool isMoving = true;
        while (isMoving)
        {
            if (Vector3.Distance(transform.position, _transf.position) > Time.deltaTime / 10)
            {
                transform.position = Vector3.Lerp(transform.position, _transf.position, 1 / MovementSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, _transf.rotation, 1 / MovementSpeed);
            }
            else
                isMoving = false;

            yield return null;
            euler = new Vector3(0, 0, 0);
        }
        if (_transf == origin.transform)
            basicShake = origin.transform.DOPunchRotation(Vector3.forward * (ShakeForce + ShakeMaxForce * 0.05f), ShakeFrequence + ShakeMaxFrequence / 0.1f, ShakeVibrato).SetLoops(-1);
        else
            basicShake = transform.DOPunchRotation(Vector3.forward * (ShakeForce + ShakeMaxForce * 0.05f), ShakeFrequence + ShakeMaxFrequence / 0.1f).SetLoops(-1);
    }
}