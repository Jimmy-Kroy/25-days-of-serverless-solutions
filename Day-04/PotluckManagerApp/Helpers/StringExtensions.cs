using System.Text.Json;


namespace PotluckManagerApp.Helpers
{
    public static class StringExtensions
    {
        public static bool IsValidJson(this string jsonString, out string errorMessage)
        {
            try
            {
                errorMessage = string.Empty;
                JsonDocument.Parse(jsonString);
                return true;
            }
            catch (System.Text.Json.JsonException ex)
            {
                errorMessage = $"Error in IsValid(string jsonString): {ex.ToString()}";
                return false;
            }
        }
    }
}
