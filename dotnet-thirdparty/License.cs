namespace DotNet.ThirdParty
{
    class License
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public string LicenseId { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
