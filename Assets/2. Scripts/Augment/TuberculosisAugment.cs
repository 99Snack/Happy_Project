
using UnityEngine;

public class TuberculosisAugment : IStatusCheckAugment
{
    private bool isApplied = false;

    public void UpdateStatus(Tower owner, AugmentData augment)
    {
        //본인 좌표 가져오기
        Vector2Int currentPos = owner.Coord;

        //다른 타워들이 있는지
        bool isAnyTowerExist = false;

        //(0,0)은 자기 위치이므로 제외
        int[] xCoord = { -1, 1 };
        int[] yCoord = { -1, 1 };

        //8방향 탐색
        for (int x = 0; x < xCoord.Length; x++)
        {
            for (int y = 0; y < yCoord.Length; y++)
            {
                Vector2Int checkPos = new Vector2Int(currentPos.x + x, currentPos.y + y);

                //해당 좌표에 타워가 있는지
                if (TileManager.Instance.map.tiles[(x,y)].isAlreadyTower)
                {
                    isAnyTowerExist = true;
                    break; 
                }
            }
            if (isAnyTowerExist) break;
        }

        bool lonely = !isAnyTowerExist;

        if (lonely && !isApplied)
        {
            owner.UpdateStatus(augment);
            isApplied = true;
            Debug.Log($"{owner.name}: {owner.CalcStageStat(augment)} 결벽증");
        }
        else if (!lonely && isApplied)
        {
            owner.atkPower.additiveStat -= owner.CalcStageStat(augment);
            isApplied = false;
            Debug.Log($"{owner.name}: 결벽증 해제.");
        }
    }
}
