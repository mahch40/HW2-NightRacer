public static class RoadConfig
{
    public const int LaneCount = 3;
    public const float LaneWidth = 2.33f;
    public const float SegmentLength = 20f;

    public const float RoadSurfaceY = 0f;

    // Lane 0 = left, 1 = middle, 2 = right. Returns the lane's center X.
    public static float LaneX(int lane)
    {
        return (lane - 1) * LaneWidth;
    }
}