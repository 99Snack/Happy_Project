using UnityEngine;

public class PooledObject : MonoBehaviour
{
    public string Tag{ get; set; }
    bool isReturned;
    
    private void OnEnable()
    {
        isReturned = false;
        Invoke(nameof(Return), 3f);
    }

    public void Return()
    {
        if (isReturned) return;

        isReturned = true;
        CancelInvoke();

        ObjectPoolManager.Instance.ReturnToPool(Tag, gameObject);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}
