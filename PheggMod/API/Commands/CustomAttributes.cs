using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PheggMod.Commands
{
    public enum RequirementType
    {
        Single = 0,
        All
    }


    #region Command and Alias
    /// <summary>
    /// Name of the command, and main alias to trigger
    /// </summary>
    public class PMCommand : Attribute
    {
        public string name;
        public PMCommand(string name) => this.name = name;
    }
    /// <summary>
    /// Additional aliases to trigger the command
    /// </summary>
    public class PMAlias : Attribute
    {
        public string[] alias;
        public PMAlias(params string[] alias) => this.alias = alias;
    }
    #endregion

    #region Parameters
    /// <summary>
    /// List of the parameters for the command. Will list them if there is not enough given parameters.
    /// </summary>
    public class PMParameters : Attribute
    {
        public string[] parameters;
        public PMParameters(params string[] parameters) => this.parameters = parameters;
    }
    /// <summary>
    /// Allows the command parameter list to be higher than the given parameter count. This will be useful for commands such as oban where reasons can be given.
    /// </summary>
    public class PMCanExtend : Attribute
    {
        public bool canExtend;
        public PMCanExtend(bool canExtend) => this.canExtend = canExtend;
    }
    #endregion

    #region Permissions and whitelist
    /// <summary>
    /// Permission (Single) required to run the command
    /// </summary>
    public class PMPermission : Attribute
    {
        public PlayerPermissions perm;
        public PMPermission(PlayerPermissions perm) => this.perm = perm;

        public bool CheckPermissions(CommandSender sender, PlayerPermissions playerPermission)
        {
            if (ServerStatic.IsDedicated && sender.FullPermissions)
                return true;

            if (PermissionsHandler.IsPermitted(sender.Permissions, perm))
                return true;

            return false;
        }
    }

    /// <summary>
    /// List of permissions required to run the command
    /// </summary>
    public class PMPermissions : Attribute
    {
        public PlayerPermissions[] perms;
        public RequirementType type;

        public PMPermissions(PlayerPermissions[] perms, RequirementType type = RequirementType.Single)
        {
            this.perms = perms;
            this.type = type;
        }

        public List<PlayerPermissions> CheckPermissions(CommandSender sender, PlayerPermissions[] pPerms)
        {
            List<PlayerPermissions> missingPerms = new List<PlayerPermissions>();

            if (type == RequirementType.Single)
            {
                if (ServerStatic.IsDedicated && sender.FullPermissions)
                    return null;

                foreach (PlayerPermissions check in pPerms)
                {
                    if (PermissionsHandler.IsPermitted(sender.Permissions, check))
                        return null;
                }

                return missingPerms = perms.ToList();
            }
            else
            {
                if (ServerStatic.IsDedicated && sender.FullPermissions)
                    return null;

                foreach (PlayerPermissions check in pPerms)
                {
                    if (!PermissionsHandler.IsPermitted(sender.Permissions, check))
                        missingPerms.Add(check);
                }

                if (missingPerms.Count > 0)
                    return missingPerms;
                else
                    return null;
            }
        }
    }

    /// <summary>
    /// List of whitelisted ranks who can run the command
    /// </summary>
    public class PMRankWhitelist : Attribute
    {
        public string[] ranks;
        public PMRankWhitelist(string[] ranks) => this.ranks = ranks;
    }
    /// <summary>
    /// List of blacklisted ranks who are not allowed to run the command
    /// </summary>
    public class PMRankBlacklist : Attribute
    {
        public string[] ranks;
        public PMRankBlacklist(string[] ranks) => this.ranks = ranks;
    }

    /// <summary>
    /// List of ranks that bypass all permissions checks for this command
    /// </summary>
    public class PMOverrideRanks : Attribute
    {
        public string[] ranks;
        public PMOverrideRanks(string[] ranks) => this.ranks = ranks;
    }
    #endregion
}
