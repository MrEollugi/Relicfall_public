using UnityEngine;

[CreateAssetMenu(menuName = "BehaviorTree/Decorator/LookDetected")]
public class LookDetectedConditionSO : ConditionSO
{
    [Range(0f, 1f)]
    public float viewThreshold = 0.8f;  // 시야 각도 조절용
    public float detectRange = 10f;      // 감지 거리

    public override bool Check(Blackboard blackboard)
    {
        if (blackboard.Player == null)
            return false;

        // 마우스 커서 방향으로 Ray 생성
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        // 플레이어의 위치를 기준으로 수평 평면 생성
        Plane groundPlane = new Plane(Vector3.up, blackboard.Player.position);  

        if (groundPlane.Raycast(ray, out float enter))  // 평면과 레이의 교차 지점 계산
        {
            Vector3 hitPoint = ray.GetPoint(enter); // 교차 지점의 월드 좌표

            // 마우스 방향
            Vector3 mouseDir = (hitPoint - blackboard.Player.position).normalized;  

            // 적 방향
            Vector3 toEnemy = (blackboard.transform.position - blackboard.Player.position).normalized;  

            float dot = Vector3.Dot(mouseDir, toEnemy); // 마우스 방향과 적 방향이 얼마나 같은지 계산

            // 거리 계산
            float distance = Vector3.Distance(blackboard.Player.position, blackboard.transform.position);   

            // 거리 안에 있고 시야각 내에 있어야 감지
            return dot > viewThreshold && distance <= detectRange;  
        }

        return false;
    }
}
