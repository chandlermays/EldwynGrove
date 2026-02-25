namespace EldwynGrove.Tools
{
    public enum PredicateType
    {
        kSelect,
        kHasQuest,
        kDoesNotHaveQuest,
        kCompletedObjective,
        kCompletedQuest,
        kHasLevel,
        kHasItem,
        kHasItems
    }

    public interface IConditionChecker
    {
        bool? Evaluate(PredicateType predicate, string[] parameters);
    }
}