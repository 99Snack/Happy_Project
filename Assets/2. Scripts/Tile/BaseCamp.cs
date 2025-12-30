using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEditor.Build.Content;

public class BaseCamp : MonoBehaviour
{
    private static BaseCamp instance;
    public static BaseCamp Instance { get => instance; private set => instance = value; }

    public int basecampHp;

    private int currentHp;
    public int CurrentHp
    {
        get => currentHp;
        set
        {
            currentHp = value;
            UIManager.Instance.UpdateAllyBaseCampHp();
        }
    }

    //▼ 실제로 몬스터가 배치될 위치 값 조정용 오프셋
    public float xOffset = 0.1f;

    private Queue<int> baseCampOrder = new Queue<int>();

    [SerializeField] GameObject enemyBaseObj;
    [SerializeField] GameObject allyBaseObj;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }
        else
        {
            Instance = this;
        }
    }

    //BaseCamp Object 세우기 
    public void EstablishBaseObj()
    {
        Vector2Int enemyBasePos = TileManager.Instance.enemyBasePosition;
        Vector2Int allyBasePos = TileManager.Instance.allyBasePosition;

        Vector3 RealEnemyBase = TileManager.Instance.GetWorldPosition(enemyBasePos);
        Vector3 RealAllyBase = TileManager.Instance.GetWorldPosition(allyBasePos);

        Instantiate(enemyBaseObj, RealEnemyBase, Quaternion.identity);
        Instantiate(allyBaseObj, RealAllyBase, Quaternion.identity);
    }

    public void SetHealthPoint(WaveData waveData)
    {
        basecampHp = waveData.BasecampHp;
        foreach (var aug in AugmentManager.Instance.activeAugments)
        {
            if (aug.Tag == 0 && aug.Category == 1)
            {
                int stage = GameManager.Instance.StageInfo.Index - 10000;
                basecampHp += aug.Value_N * (int)aug.CalcGrowValue(stage);
            }
        }
        CurrentHp = basecampHp;
    }


    //▼ baseCamp 데미지 받기 
    public void TakeDamage(int damage)
    {
        CurrentHp -= damage;

        //Debug.Log($"{currentHp}");

        if (currentHp <= 0)
        {
            SpawnManager.Instance.OnWaveDefeat();
        }
    }

}
