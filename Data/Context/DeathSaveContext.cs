namespace Systems.SimpleEntities.Data.Context
{
    /// <summary>
    ///     Context for death save - used to determine if entity should be saved from death
    /// </summary>
    public readonly ref struct DeathSaveContext
    {
        /// <summary>
        ///     If true, entity should be saved
        /// </summary>
        public readonly bool shouldBeSaved;
        
        /// <summary>
        ///     Health to set for the entity after saving from death
        /// </summary>
        public readonly long healthToSet;

        public DeathSaveContext(bool shouldBeSaved, long healthToSet)
        {
            this.shouldBeSaved = shouldBeSaved;
            this.healthToSet = healthToSet;
        }
    }
}