namespace SST.Data
{

    [System.Serializable]
    public struct StateData
    {
        public MetadataStateData metadataState;
        public LogStateData logState;
        public TimeStateData timeState;
        public AssetStateData assetState;
        public IdStateData idState;
        public ResourceStateData resourceState;
        public BuildStateData buildState;
        public EconomyStateData economyState;
        public IdBindingStateData idBindingState;
    }
}