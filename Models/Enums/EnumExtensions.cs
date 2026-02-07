using Models.Attributes;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Models.Enums;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        return value.GetType()
                    .GetMember(value.ToString())
                    .FirstOrDefault()?
                    .GetCustomAttribute<MutationDescriptionAttribute>()?
                    .Description ?? value.ToString();
    }

    public static string ToReadableString(this Enum value)
    {
        return Regex.Replace(value.ToString(), "([a-z])([A-Z])", "$1 $2");
    }
}