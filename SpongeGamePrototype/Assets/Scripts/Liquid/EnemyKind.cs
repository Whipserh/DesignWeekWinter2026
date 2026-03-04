using UnityEngine;

public enum EnemyKind
{
    Water,
    Fertilizer,
    Herbicide
}

public class EnemyType : MonoBehaviour
{
    public EnemyKind kind;
}