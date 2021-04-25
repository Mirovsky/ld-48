using UnityEngine;

public static class ObjectPlacementHelper
{
    public const float kBlockSize = 1.0f;

    public static Vector2 GetBlockOffset(float width, float height)
    {
        return new Vector2(-(width - 1) * kBlockSize * 0.5f, -(height - 1) * kBlockSize * 0.5f);
    }

    public static Vector2 GetObjectPosition(float x, float y)
    {
        return new Vector2(x * kBlockSize, y * kBlockSize);
    }
}
