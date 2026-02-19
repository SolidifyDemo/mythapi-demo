using MythApi.Common.Database.Models;
using System.Text;

namespace MythApi.Common.Extensions
{
    public static class GodExtensions
    {
        public static string GodFormatter(this God god)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Id: {god.Id}");
            builder.AppendLine($"Name: {god.Name}");
            builder.AppendLine($"Description: {god.Description}");
            builder.AppendLine($"MythologyId: {god.MythologyId}");
            return builder.ToString();
        }
    }
}
