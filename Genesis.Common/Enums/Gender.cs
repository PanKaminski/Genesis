namespace Genesis.Common.Enums
{
    public enum Gender : byte
    {
        Man = 1,
        Woman
    }

    public static class GenderExtensions
    {
        public static string GetClientView(this Gender gender) => gender switch
        {
            Gender.Man => "male",
            Gender.Woman => "female",
            _ => throw new KeyNotFoundException("Invalid gender value"),
        };

        public static Gender GetOppositeGender(this Gender gender) => 
            gender == Gender.Man ? Gender.Woman : Gender.Man;
    }
}
