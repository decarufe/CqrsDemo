using Microsoft.WindowsAzure.ServiceRuntime;

namespace Pyxis.Messaging.Azure
{
    public class ChannelNameUtils
    {
        public static string GenerateName(string installName, string topicName, bool appendRoleAsSuffix = false)
        {
            var finalInstallName = string.IsNullOrWhiteSpace(installName) ? "unnamed" : installName;
            var finalRoleSuffix = appendRoleAsSuffix ? GetRoleInstanceSuffix() : string.Empty;
            return finalInstallName + "_" + topicName + finalRoleSuffix;
        }

        private static string GetRoleInstanceSuffix()
        {
            var result = string.Empty;
            if (RoleEnvironment.IsAvailable)
            {
                var id = RoleEnvironment.CurrentRoleInstance.Id;
                var instanceLocation = id.LastIndexOf("_");
                if (instanceLocation != -1)
                {
                    result = id.Substring(instanceLocation);
                }
            }
            return result;
        }
    }
}
