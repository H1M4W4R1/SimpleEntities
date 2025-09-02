using Systems.SimpleCore.Operations;

namespace Systems.SimpleEntities.Operations
{
    public static class EntityOperations
    {
        public const ushort SYSTEM_ENTITY = 0x0003;

        public const ushort ERROR_ENTITY_SAVED_FROM_DEATH = 1;
        
        public const ushort SUCCESS_ENTITY_DAMAGED = 1;
        public const ushort SUCCESS_ENTITY_HEALED = 2;
        public const ushort SUCCESS_ENTITY_KILLED = 3;
        
        // Shifted for a reason
        public const ushort SUCCESS_ENTITY_TICK = 32;


        public static OperationResult Permitted() =>
            OperationResult.Success(SYSTEM_ENTITY, OperationResult.SUCCESS_PERMITTED);

        public static OperationResult Killed() =>
            OperationResult.Success(SYSTEM_ENTITY, SUCCESS_ENTITY_KILLED);

        public static OperationResult Damaged() =>
            OperationResult.Success(SYSTEM_ENTITY, SUCCESS_ENTITY_DAMAGED);

        public static OperationResult Healed() => OperationResult.Success(SYSTEM_ENTITY, SUCCESS_ENTITY_HEALED);

        public static OperationResult TickExecuted() => 
            OperationResult.Success(SYSTEM_ENTITY, SUCCESS_ENTITY_TICK);
        
        public static OperationResult SavedFromDeath()
            => OperationResult.Error(SYSTEM_ENTITY, ERROR_ENTITY_SAVED_FROM_DEATH);
        
    }
}