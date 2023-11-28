using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TriangleCreator : MonoBehaviour
{
    public float scaleFactor = 1.0f; // 삼각형의 크기를 조절할 변수

    private void Start()
    {
        // Mesh 객체 생성
        Mesh mesh = new Mesh();

        // 버텍스 배열 생성 (삼각형의 꼭짓점)
        Vector3[] vertices = new Vector3[3];
        vertices[0] = new Vector3(0, 0, 0) * scaleFactor;      // 첫 번째 꼭짓점
        vertices[1] = new Vector3(1, 0, 0) * scaleFactor;      // 두 번째 꼭짓점
        vertices[2] = new Vector3(0.5f, 1, 0) * scaleFactor;   // 세 번째 꼭짓점

        // 삼각형을 이루는 인덱스 배열 생성
        int[] triangles = new int[3];
        triangles[0] = 0; // 첫 번째 꼭짓점
        triangles[1] = 1; // 두 번째 꼭짓점
        triangles[2] = 2; // 세 번째 꼭짓점

        // Mesh에 버텍스와 삼각형 데이터 설정
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Mesh를 GameObject의 Mesh Filter 컴포넌트에 할당
        GetComponent<MeshFilter>().mesh = mesh;

        // Mesh 렌더링을 위한 Mesh Renderer 컴포넌트 활성화
        GetComponent<MeshRenderer>().enabled = true;
    }
}
