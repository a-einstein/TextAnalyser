namespace TextAnalyser.Model
{
    internal class Address
    {
        // Note the strings need to be nullable reference types, since C# 8.0.

        public string? Street { get; set; }
        public int? Number { get; set; }
        public string? Addition { get; set; }
        public string? Code { get; set; }
        public string? Town { get; set; }
    }
}
