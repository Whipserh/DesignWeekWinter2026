using UnityEngine;

public enum EnemyKind
{
    Water,
    Fertilizer,
    Herbicide,
    Paint
}

public class EnemyType : MonoBehaviour
{
    public Color paintColor;
    public EnemyKind kind;
}