namespace Packaging.Targets.Deb
{
    public class PackageDependency
    {
        public string Name { get; set; }

        public string Version { get; set; }

        public PackageDependencyRelation Relation { get; set; } = PackageDependencyRelation.GreaterOrEqual;

        public override string ToString()
        {
            var rel = this.Relation == PackageDependencyRelation.StrictlyLower
                ? "<<"
                : this.Relation == PackageDependencyRelation.LowerOrEqual
                    ? "<="
                    : this.Relation == PackageDependencyRelation.ExactlyEqual
                        ? "=="
                        : this.Relation == PackageDependencyRelation.GreaterOrEqual
                            ? ">="
                            : ">>";

            if (this.Version == null)
            {
                return this.Name;
            }

            return $"{this.Name} ({rel} {this.Version})";
        }
    }
}