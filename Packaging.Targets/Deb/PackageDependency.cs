namespace Packaging.Targets.Deb
{
    public class PackageDependency
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public PackageDependencyRelation Relation { get; set; } = PackageDependencyRelation.GreaterOrEqual;
        public override string ToString()
        {
            var rel = Relation == PackageDependencyRelation.StrictlyLower
                ? "<<"
                : Relation == PackageDependencyRelation.LowerOrEqual
                    ? "<="
                    : Relation == PackageDependencyRelation.ExactlyEqual
                        ? "=="
                        : Relation == PackageDependencyRelation.GreaterOrEqual
                            ? ">="
                            : ">>";

            if (Version == null)
                return Name;
            return $"{Name} ({rel} {Version})";
        }
    }

    public enum PackageDependencyRelation
    {
        StrictlyLower,
        LowerOrEqual,
        ExactlyEqual,
        GreaterOrEqual,
        StrictlyGreater
    }
}