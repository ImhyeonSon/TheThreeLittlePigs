using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    이 스크립트는 MasterSlingshot의 크기가 변경될 때 고무줄의 길이를 줄이거나 늘리는데 도움을 줍니다.
    잘 작동하려면 인스펙터 창에서 모든 뼈가 설정되어 있는지 확인하세요.
    스크립트는 PlayMode에 들어갈 때 한 번 매개변수를 설정합니다.
*/

public class ElasticManager : MonoBehaviour
{
    [Range(1, 15)] // 1은 고무줄이 작동하기 위한 최소 뼈의 수이고 15는 FBX 파일 내의 전체 뼈의 수입니다.
    [SerializeField] private int numberOfBones;

    [Range(0, 7)] // 고무줄의 저항력, 이 값은 Slingshot 스크립트에서 가져옵니다.
    public float elasticResistance;

    // 뼈의 SpringJoint 컴포넌트의 원래 값들입니다.
    public float spring = 3000f;
    public float minDistance = 0f;
    public float maxDistance = 0.01f;
    public float tolerance = 0.05f;
    public float elasticLinesWidth = 0.4f; // 선 렌더러의 너비를 제어합니다.

    // 고무줄의 뼈 객체 배열입니다.
    [SerializeField] private GameObject[] rightBones = new GameObject[15];
    [SerializeField] private GameObject[] leftBones = new GameObject[15];
    [SerializeField] private GameObject rightBoneEnd;
    [SerializeField] private GameObject leftBoneEnd;

    // 마지막 고무줄 뼈에 영향을 받는 관절들입니다.
    [SerializeField] private GameObject leather;
    [SerializeField] private GameObject leatherLine;
    [SerializeField] private SpringJoint knotR;
    [SerializeField] private SpringJoint knotL;
    private FixedJoint[] leatherJoints;
    private FixedJoint[] leatherLineJoints;

    // 늘어날 때의 고무줄 효과에 사용되는 LineRenderers입니다.
    [SerializeField] private LineRenderer rightElasticLine;
    [SerializeField] private LineRenderer leftElasticLine;

    private int i = 1; // 인덱스로 사용됩니다.

    void Awake()
    {
        // 마지막 고무줄 뼈를 참조하는 fixed joints를 설정합니다.
        leatherJoints = leather.GetComponents<FixedJoint>();
        leatherLineJoints = leatherLine.GetComponents<FixedJoint>();

        // 늘어날 때의 고무줄 선의 너비를 설정합니다.
        rightElasticLine.SetWidth(elasticLinesWidth, elasticLinesWidth);
        leftElasticLine.SetWidth(elasticLinesWidth, elasticLinesWidth);

        // 오른쪽 고무줄의 각 뼈를 설정합니다.
        foreach (GameObject bone in rightBones)
        {
            if (i <= numberOfBones)
            {
                // 사용된 뼈에 SpringJoint 값들을 설정합니다.
                SpringJoint bsj = bone.GetComponent<SpringJoint>();
                bsj.spring = spring;
                bsj.minDistance = minDistance;
                bsj.maxDistance = maxDistance;
                bsj.tolerance = tolerance;

                // 마지막 뼈인지 확인합니다.
                if (i == numberOfBones)
                {
                    // 선택된 뼈의 수에 따라 마지막 뼈를 설정하고 그 뼈의 리지드바디를 고무줄과 가죽 관절에 연결합니다.
                    rightBoneEnd.transform.parent = bone.transform;
                    leatherJoints[1].connectedBody = bone.GetComponent<Rigidbody>();
                    leatherLineJoints[1].connectedBody = bone.GetComponent<Rigidbody>();
                    knotR.connectedBody = bone.GetComponent<Rigidbody>();
                }
            }
            else
            {
                // 사용되지 않는 뼈를 비활성화합니다.
                bone.SetActive(false);
            }

            i++;
        }

        // 왼쪽 고무줄에서 동일한 절차를 반복하기 위해 i를 최소 뼈 수로 설정합니다.
        i = 1;

        // 왼쪽 고무줄의 각 뼈를 설정합니다.
        foreach (GameObject bone in leftBones)
        {
            if (i <= numberOfBones)
            {
                // 사용된 뼈에 SpringJoint 값들을 설정합니다.
                SpringJoint bsj = bone.GetComponent<SpringJoint>();
                bsj.spring = spring;
                bsj.minDistance = minDistance;
                bsj.maxDistance = maxDistance;
                bsj.tolerance = tolerance;

                // 마지막 뼈인지 확인합니다.
                if (i == numberOfBones)
                {
                    // 선택된 뼈의 수에 따라 마지막 뼈를 설정하고 그 뼈의 리지드바디를 고무줄과 가죽 관절에 연결합니다.
                    leftBoneEnd.transform.parent = bone.transform;
                    leatherJoints[0].connectedBody = bone.GetComponent<Rigidbody>();
                    leatherLineJoints[0].connectedBody = bone.GetComponent<Rigidbody>();
                    knotL.connectedBody = bone.GetComponent<Rigidbody>();
                }
            }
            else if (i > numberOfBones)
            {
                // 사용되지 않는 뼈를 비활성화합니다.
                bone.SetActive(false);
            }

            i++;
        }
    }
}
