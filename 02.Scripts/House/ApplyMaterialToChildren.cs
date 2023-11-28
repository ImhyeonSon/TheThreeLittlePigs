using UnityEngine;

public class ApplyMaterialToChildren : MonoBehaviour
{
    public Material sharedMaterial; // 일괄적으로 적용할 Material을 Inspector에서 설정
    // 자식들에게 material을 일관되게 적용하는 코드
    void Start()
    {
        // 부모 GameObject의 모든 자식 GameObject를 가져오기
        Transform[] children = GetComponentsInChildren<Transform>();

        // 각 자식 GameObject에 Material을 설정
        foreach (Transform child in children)
        {
            if (child != transform) // 자기 자신은 제외
            {
                Renderer renderer = child.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = sharedMaterial;
                }
            }
        }
    }
}
