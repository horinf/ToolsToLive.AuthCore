namespace ToolsToLive.AuthCore.Model
{
    public enum TokenTransport
    {
        /// <summary>
        /// Default value, should not be used (if it is presented, it means, someone forgot to set the proper value)
        /// </summary>
        Undefined,

        /// <summary>
        /// Token comes from header (usual way)
        /// </summary>
        Header,

        /// <summary>
        /// Token comes from cookies (reserve way for special cases)
        /// </summary>
        Cookie,
    }
}
