using Systems.SimpleCore.Operations;

namespace Systems.SimpleEntities.Operations
{
    public static class StatusOperations
    {
        public const int INVALID_STATUS = 1;
        public const int MAX_STACK_REACHED = 2;
        public const int NOT_APPLIED = 3;
        public const int NOT_ENOUGH_STACKS = 4;
        
        public static OperationResult Permitted() => OperationResult.GenericSuccess;
        
        public static OperationResult InvalidStatus() => new(INVALID_STATUS);
        public static OperationResult MaxStackReached() => new(MAX_STACK_REACHED);
        public static OperationResult NotApplied() => new(NOT_APPLIED);
        public static OperationResult NotEnoughStacks() => new(NOT_ENOUGH_STACKS);
        
        public static OperationResult StatusApplied() => OperationResult.GenericSuccess;
        public static OperationResult StatusRemoved() => OperationResult.GenericSuccess;
        public static OperationResult StatusStackChanged() => OperationResult.GenericSuccess;
    }
}