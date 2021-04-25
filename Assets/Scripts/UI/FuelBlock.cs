using UnityEngine;
using Zenject;

public class FuelBlock : MonoBehaviour
{
    public class Pool : MonoMemoryPool<FuelBlock> { }
}
