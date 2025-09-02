using Systems.SimpleCore.Operations;

namespace Systems.SimpleEntities.Operations
{
    public static class StatusOperations
    {
        public const ushort SYSTEM_STATUS = 0x0002;

        public const ushort ERROR_INVALID_STATUS = 1;
        public const ushort ERROR_APPLY_MAX_STACK_REACHED = 2;
        public const ushort ERROR_REMOVE_NOT_ENOUGH_STACKS = 3;
        public const ushort ERROR_REMOVE_NOT_APPLIED = 4;

        public const ushort SUCCESS_STATUS_APPLIED = 1;
        public const ushort SUCCESS_STATUS_REMOVED = 2;
        public const ushort SUCCESS_STATUS_STACK_CHANGED = 3;

        public static OperationResult Permitted()
            => OperationResult.Success(SYSTEM_STATUS, OperationResult.SUCCESS_PERMITTED);

        public static OperationResult InvalidStatus()
            => OperationResult.Error(SYSTEM_STATUS, ERROR_INVALID_STATUS);

        public static OperationResult MaxStackReached()
            => OperationResult.Error(SYSTEM_STATUS, ERROR_APPLY_MAX_STACK_REACHED);

        public static OperationResult NotApplied()
            => OperationResult.Error(SYSTEM_STATUS, ERROR_REMOVE_NOT_APPLIED);

        public static OperationResult NotEnoughStacks()
            => OperationResult.Error(SYSTEM_STATUS, ERROR_REMOVE_NOT_ENOUGH_STACKS);

        public static OperationResult StatusApplied()
            => OperationResult.Success(SYSTEM_STATUS, SUCCESS_STATUS_APPLIED);

        public static OperationResult StatusRemoved()
            => OperationResult.Success(SYSTEM_STATUS, SUCCESS_STATUS_REMOVED);

        public static OperationResult StatusStackChanged()
            => OperationResult.Success(SYSTEM_STATUS, SUCCESS_STATUS_STACK_CHANGED);
    }
}