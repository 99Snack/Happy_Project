using UnityEngine;

public class TuberculosisAugment : IStatusCheckAugment
{
    private bool isApplied = false;
    private int lastAppliedStat = 0; //적용했던 수치를 저장하여 안전하게 해제
    private AugmentData data;

    public TuberculosisAugment(AugmentData data)
    {
        this.data = data;
        this.isApplied = false;
    }

    public void UpdateStatus(Tower owner)
    {
        Vector2Int currentPos = owner.Coord;

        bool isAnyTowerExist = false;

        // 8방향 탐색 (-1, 0, 1 오프셋 사용)
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue; 

                Vector2Int checkPos = new Vector2Int(currentPos.x + x, currentPos.y + y);

                if (TileManager.Instance.map.tiles.ContainsKey((checkPos.x, checkPos.y)))
                {
                    if (TileManager.Instance.map.tiles[(checkPos.x, checkPos.y)].isAlreadyTower)
                    {
                        isAnyTowerExist = true;
                        break;
                    }
                }
            }
            if (isAnyTowerExist) break;
        }

        bool lonely = !isAnyTowerExist;

        //주변에 아무도 없고, 아직 버프가 적용되지 않은 경우
        if (lonely && !isApplied)
        {
            //나중에 해제할 때를 대비해 현재 시점의 증가량을 저장
            lastAppliedStat = owner.CalcStageStat(data);

            owner.atkPower.additiveStat += lastAppliedStat;
            isApplied = true;

            Debug.Log($"{owner.name}: {lastAppliedStat} 결벽증 발동 (고립됨)");
        }
        //주변에 누군가 생겼고, 현재 버프가 적용 중인 경우
        else if (!lonely && isApplied)
        {
            //저장해두었던 수치만큼 정확히 차감
            owner.atkPower.additiveStat -= lastAppliedStat;
            isApplied = false;
            lastAppliedStat = 0;

            Debug.Log($"{owner.name}: 결벽증 해제 (이웃 발생)");
        }
    }
}