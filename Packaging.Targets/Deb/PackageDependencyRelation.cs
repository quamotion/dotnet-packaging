namespace Packaging.Targets.Deb
{
    public enum PackageDependencyRelation
    {
        StrictlyLower,
        LowerOrEqual,
        ExactlyEqual,
        GreaterOrEqual,
        StrictlyGreater
    }
}
