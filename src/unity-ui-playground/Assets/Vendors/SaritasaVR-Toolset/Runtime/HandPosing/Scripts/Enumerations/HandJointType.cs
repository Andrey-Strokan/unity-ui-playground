namespace HandPosing.Enumerations
{
    /// <summary>
    /// Hand joint type.
    /// </summary>
    public enum HandJointType
    {
        Invalid = -1,
        Wrist,

        ThumbTrapezium,
        ThumbMeta,
        ThumbProximal,
        ThumbDistal,

        IndexProximal,
        IndexIntermediate,
        IndexDistal,

        MiddleProximal,
        MiddleIntermediate,
        MiddleDistal,

        RingProximal,
        RingIntermediate,
        RingDistal,

        PinkyMeta,
        PinkyProximal,
        PinkyIntermediate,
        PinkyDistal,

        Count
    }
}