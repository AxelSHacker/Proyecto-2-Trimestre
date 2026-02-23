using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PoolManager : CustomMonoBehaviour
{
   private static PoolManager _instance;
   public static PoolManager Instance => _instance;
   public Pool[] pools;
   public override void EditorInit()
   {

   }
   void Awake()
   {
      if (_instance == null)
      {
         _instance = this;

      }
      else
      {
         Destroy(this);
      }

   }

   void OnEnable()
   {
      PoolEntity.OnReturnToPool += Push;
   }

   void OnDisable()
   {
      PoolEntity.OnReturnToPool -= Push;
   }
   void Start()
   {
      InitializePools();
   }

   private Pool GetPool(string poolId)
   {
      Pool pool = pools.SingleOrDefault(p => p.id == poolId);
      if (pool.prefab == null)
      {
         Debug.LogWarning("No pool found with id: " + poolId);
         pool = pools.FirstOrDefault(p => p.id == poolId);
      }
      return pool;
   }

   private PoolEntity CreatePoolEntity(string poolId)
   {
      Pool pool = GetPool(poolId);
      PoolEntity entity = Instantiate(pool.prefab, transform);
      entity.PoolID = pool.id;

      return entity;
   }

   private void InitializePools()
   {
      foreach (Pool pool in pools)
      {
         for (int i = 0; i < pool.prewarm; i++)
         {
            PoolEntity entity = CreatePoolEntity(pool.id);
            entity.Deactivate();
            pool.pool.Enqueue(entity);
         }
      }
   }

   public PoolEntity Pull(string poolId, Vector3 position, Quaternion rotation)
   {
      Pool pool = GetPool(poolId);
      if (!pool.pool.TryDequeue(out PoolEntity entity))
      {
         entity = CreatePoolEntity(poolId);
      }
      
      if (entity != null)
      {
         entity.transform.position = position;
         entity.transform.rotation = rotation;

         entity.Initialize();
      }

      return entity;
   }

   public void Push(PoolEntity entity)
   {
      Pool pool = GetPool(entity.PoolID);
      pool.pool.Enqueue(entity);
   }

}





