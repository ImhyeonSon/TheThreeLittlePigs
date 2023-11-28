using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Transform center; // 중심점
    public float orbitSpeed = 30.0f; // 궤도 속도
    public float rotationSpeed = 80.0f; // 자체 회전 속도
    public Vector3 orbitAxis = Vector3.up; // 궤도 축 (기본적으로 위쪽을 향함)
    public float orbitRadius = 5.0f; // 궤도 반지름

    private void Update()
    {
        OrbitAround(); // 궤도 회전 함수 호출
        UseRotate(); // 자체 회전 함수 호출
    }

    void OrbitAround()
    {
        if (center != null)
        {
            transform.RotateAround(center.position, orbitAxis, orbitSpeed * Time.deltaTime);
        }
    }

    void UseRotate()
    {
        transform.Rotate(new Vector3(0f, rotationSpeed, 0f) * Time.deltaTime);
    }
}
