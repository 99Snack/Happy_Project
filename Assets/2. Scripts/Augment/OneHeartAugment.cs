using UnityEngine;

public class OneHeartAugment : IStatusCheckAugment
{
    private bool isApplied = false;
    private int currentNearTowerCount = 0; //현재 적용된 버프 기준 타워 수
    private int lastAppliedStat = 0;       //마지막으로 추가했던 스탯 값 저장
    private AugmentData data;

    public OneHeartAugment(AugmentData data)
    {
        this.data = data;
        this.isApplied = false;
        this.currentNearTowerCount = 0;
        this.lastAppliedStat = 0;
    }

    public void UpdateStatus(Tower owner)
    {
        int detectedTowers = 0;
        Vector2Int currentPos = owner.Coord;

        //8방향 탐색
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue; // 자기 자신 제외

                Vector2Int checkPos = new Vector2Int(currentPos.x + x, currentPos.y + y);

                if (TileManager.Instance.map.tiles.ContainsKey((checkPos.x, checkPos.y)))
                {
                    if (TileManager.Instance.map.tiles[(checkPos.x, checkPos.y)].isAlreadyTower)
                    {
                        detectedTowers++;
                    }
                }
            }
        }

        if (detectedTowers != currentNearTowerCount)
        {
            //기존에 적용된 버프가 있다면 먼저 제거
            if (isApplied)
            {
                owner.atkPower.additiveStat -= lastAppliedStat;
                isApplied = false;
            }

            //새로운 타워 개수가 0보다 크면 버프 적용
            if (detectedTowers > 0)
            {
                currentNearTowerCount = detectedTowers;
                lastAppliedStat = CalcAdditiveStat(data, detectedTowers);

                owner.atkPower.additiveStat += lastAppliedStat;
                isApplied = true;

                Debug.Log($"{owner.name}: 주변 타워 {detectedTowers}개 감지. 공격력 {lastAppliedStat} 증가.");
            }
            else
            {
                //주변에 타워가 없으면 카운트 초기화
                currentNearTowerCount = 0;
                lastAppliedStat = 0;
                Debug.Log($"{owner.name}: 주변에 타워가 없어 버프가 해제되었습니다.");
            }
        }
    }

    //nearTower를 인자로 받아 유연하게 계산하도록 변경
    int CalcAdditiveStat(AugmentData augment, int towerCount)
    {
        return Mathf.FloorToInt(augment.CalcGrowValue() * towerCount);
    }
}