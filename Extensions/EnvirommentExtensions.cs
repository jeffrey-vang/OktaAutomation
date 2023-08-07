namespace OktaAutomation.Extensions
{
    public static class EnvirommentExtensions
    {
        public static string ToPrefix(this Enums.Environment env) => env switch
        {
            Enums.Environment.Development => "dev",
            Enums.Environment.Integration => "int",
            Enums.Environment.Training => "trn",
            Enums.Environment.Production => "prd",
            _ => "dev"
        };

        public static string ToRoutingPrefix(this Enums.Environment env) => env switch
        {
            Enums.Environment.Development => "dev-",
            Enums.Environment.Integration => "int-",
            Enums.Environment.Training => "trn-",
            Enums.Environment.Production => string.Empty,
            _ => "dev"
        };
    }
}
