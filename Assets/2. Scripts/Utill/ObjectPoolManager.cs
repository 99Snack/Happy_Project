using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    private static ObjectPoolManager instance;
    public static ObjectPoolManager Instance { get => instance; set => instance = value; }

    [System.Serializable]
    public struct Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools; // 인스펙터에서 직접 넣은 프리팹 리스트
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
    }

    private IEnumerator Start()
    {
        foreach (Pool pool in pools)
        {
            yield return StartCoroutine(CreatePoolRoutine(pool.tag, pool.prefab, pool.size));
        }

        //몬스터 프리팹 생성
        foreach (var monster in DataManager.Instance.MonsterData)
        {
            int monsterID = monster.Key;
            MonsterData data = monster.Value;
            string tag = monsterID.ToString();

            //이미 생성된 태그라면
            if (poolDictionary.ContainsKey(tag)) continue;

            ResourceRequest request = Resources.LoadAsync<GameObject>($"Prefab/Enemy/{monsterID}");
            yield return request;

            GameObject prefab = request.asset as GameObject;
            if (prefab == null)
            {
                Debug.LogWarning($"[ObjectPool] 프리팹 로드 실패: Prefab/Enemy/{monsterID}");
                continue;
            }

            //타입이 2 이하라면 30마리, 3이면 5마리
            int poolSize = (data.MonsterType <= 2) ? 30 : 5;

            yield return StartCoroutine(CreatePoolRoutine(tag, prefab, poolSize));
        }

        Debug.Log("모든 오브젝트 풀 생성 완료");
    }

    private IEnumerator CreatePoolRoutine(string tag, GameObject prefab, int size)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            poolDictionary.Add(tag, new Queue<GameObject>());
        }

        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            obj.transform.SetParent(transform);

            if (obj.TryGetComponent(out PooledObject pooled))
            {
                pooled.Tag = tag;
            }

            poolDictionary[tag].Enqueue(obj);

            //프레임당 최대 10마리 생성 (병목현상 방지를 위해)
            if (i % 10 == 0) yield return null;
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag)) return null;

        //풀이 비어있다면 새로 하나 생성해서 반환
        if (poolDictionary[tag].Count == 0)
        {
            Pool pool = pools.Find(p => p.tag == tag);
            GameObject newObj = Instantiate(pool.prefab);
            newObj.GetComponent<PooledObject>().Tag = tag;
            newObj.SetActive(false);
            poolDictionary[tag].Enqueue(newObj);
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.transform.SetParent(null); // 부모 해제하여 독립적으로 존재

        return objectToSpawn;
    }

    public void ReturnToPool(string tag, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(tag)) return;

        Debug.Log($"{tag}반환");
        obj.SetActive(false);
        obj.transform.SetParent(transform); // 다시 매니저 밑으로 정렬
        poolDictionary[tag].Enqueue(obj);
    }
}