using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class BaseCamp : MonoBehaviour
{
    private static BaseCamp instance;
    public static BaseCamp Instance{ get => instance; private set => instance = value;}

    WaveData waveData;
    public int basecampHp;
    //▼ 실제로 몬스터가 배치될 위치 값 조정용 오프셋
    public float xOffset = 0.1f; 

    private Queue<int> baseCampOrder = new Queue<int>(); 

    [SerializeField] GameObject enemyBaseObj;
    [SerializeField] GameObject allyBaseObj;


    private void Awake() 
    {
        if(Instance != null && Instance != this )
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

        Instantiate(enemyBaseObj,RealEnemyBase, Quaternion.identity);
        Instantiate(allyBaseObj,RealAllyBase,Quaternion.identity);
    }

    //WaveID 번호를 통해 baseCampHp 가져오기 
    public void SetUp(int waveID)
    {
        waveData = DataManager.Instance.WaveData[waveID];
        basecampHp = waveData.BasecampHp;
        
        if(baseCampOrder.Count == 0)
        {
            baseCampOrder.Enqueue(1);
            baseCampOrder.Enqueue(0);
            baseCampOrder.Enqueue(-1);
        }

        if(baseCampOrder.Count > 0  && baseCampOrder.Peek() != 1)
        {
            baseCampOrder.Clear();
            baseCampOrder.Enqueue(1);
            baseCampOrder.Enqueue(0);
            baseCampOrder.Enqueue(-1);
        }
    }

    //▼ baseCamp 데미지 받기 
    public void TakeDamage(int damage)
    {
        basecampHp -= damage;

        Debug.Log($"{basecampHp}");

        if(basecampHp <= 0)
        {
            //todo: 패배 연결  
        }
    }

    //▼ Monster가 BaseCamp 내에서 실제로 가아하는 좌표 받기

    public Vector3 GetMonsterAttackWorldPos()
    {
        Vector3 destination;
        
        float zOffset = Random.Range(-0.5f, 0.5f);

        int posOrder = baseCampOrder.Dequeue();
        baseCampOrder.Enqueue(posOrder);
        
        Vector2Int allyBasePos = TileManager.Instance.allyBasePosition;
        Vector2Int tilePos = new Vector2Int(allyBasePos.x, allyBasePos.y + posOrder); 
        Vector3 tileWorldPos = TileManager.Instance.GetWorldPosition(tilePos); 

        destination = new Vector3(tileWorldPos.x - xOffset, tileWorldPos.y, tileWorldPos.z + zOffset);

        return destination;
    }

    //▼ 테스트용 연결 애니메이션 연결해주고 GetMonsterAttackWorldPos를 통해 position 받아야함
     
    public void MoveMonsterToAttackPos(GameObject monster)
    {
        Vector3 toPos = GetMonsterAttackWorldPos();

        StartCoroutine(MoveMonster(monster, toPos));
    }
    
    public IEnumerator MoveMonster(GameObject monster, Vector3 toPos)
    {
        Vector3 basedir = (toPos - monster.transform.position).normalized;

        while (true)
        {
            float currentDis = Vector3.Distance(monster.transform.position, toPos);

            if (Time.deltaTime >= currentDis)
            {
                monster.transform.position = toPos;
                break;
            }

            monster.transform.position += basedir * Time.deltaTime;

            yield return null;   
        } 
    }
}
