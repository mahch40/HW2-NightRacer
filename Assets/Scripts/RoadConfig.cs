public static class RoadConfig
{
    public const int LaneCount = 3;
    public const float LaneWidth = 2.33f;
    public const float SegmentLength = 20f;

    // Top surface of the asphalt (the tile is 0.2 thick).
    public const float RoadSurfaceY = 0.2f;

    // Lane 0 = left, 1 = middle, 2 = right. Returns the lane's center X.
    public static float LaneX(int lane)
    {
        return (lane - 1) * LaneWidth;
    }
}