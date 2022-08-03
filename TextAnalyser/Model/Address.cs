namespace TextAnalyser.Model
{
    internal class Address
    {
        //TODO Strings are already nullable
        public string? Street { get; set; }
        //TODO Int?
        public string? Number { get; set; }
        public string? Addition { get; set; }
        public string? Code { get; set; }
        public string? Town { get; set; }
    }
}
