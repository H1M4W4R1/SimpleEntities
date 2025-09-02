using Systems.SimpleCore.Operations;

namespace Systems.SimpleEntities.Operations
{
    public static class EntityOperations
    {
        public const int SAVE_FROM_DEATH = 1;
        
        public static OperationResult Permitted() => OperationResult.GenericSuccess;
        
        public static OperationResult Killed() => OperationResult.GenericSuccess;
        public static OperationResult Damaged() => OperationResult.GenericSuccess;
        public static OperationResult Healed() => OperationResult.GenericSuccess;

        public static OperationResult SavedFromDeath() => new(SAVE_FROM_DEATH);

    }
}