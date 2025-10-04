using System.Security.Claims;

namespace FAST.Core
{
    /// <summary>
    /// Claim Helper Class
    /// </summary>
    public static class claimHelper
    {
        /// <summary>
        /// Gets a claim from a list of claims
        /// </summary>
        /// <param name="claims">The claims.</param>
        /// <param name="claimName">Name of the claim.</param>
        /// <returns>Claim.</returns>
        public static Claim getClaim(IEnumerable<Claim> claims, string claimName)
        {
            Claim rtn = null;

            if (!string.IsNullOrEmpty(claimName))
            {
                rtn = claims.FirstOrDefault(x => x.Type == claimName);
            }

            return rtn;
        }

        /// <summary>
        /// Gets the value of the requested claim if it exists
        /// </summary>
        /// <param name="claims">The claims.</param>
        /// <param name="claimName">Name of the claim.</param>
        /// <returns>System.String.</returns>
        public static string getClaimValue(IEnumerable<Claim> claims, string claimName)
        {
            string rtn = null;

            if (!string.IsNullOrEmpty(claimName))
            {
                Claim claim = getClaim(claims, claimName);
                if (claim != null)
                    rtn = claim.Value;
            }

            return rtn;
        }

        /// <summary>
        /// Determines whether the specified role name has role.
        /// </summary>
        /// <param name="claims">The claims.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <returns><c>true</c> if the specified role name has role; otherwise, <c>false</c>.</returns>
        public static bool hasRole(IEnumerable<Claim> claims, string roleName)
        {
            bool rtn = false;

            if (!string.IsNullOrEmpty(roleName))
            {
                rtn = claims.Where(c => c.Type == "role" && c.Value.Contains(roleName)).Count() > 0;
            }

            return rtn;
        }

        /// <summary>
        /// Determines whether the specified role name has roles.
        /// </summary>
        /// <param name="claims">The claims.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <returns><c>true</c> if the specified role name has roles; otherwise, <c>false</c>.</returns>
        public static bool hasRoles(IEnumerable<Claim> claims, string roleName)
        {
            bool rtn = false;

            string[] roles = roleName.Split(',');
            for (int i = 0; i < roles.Count(); i++)
                roles[i] = roles[i].Trim();

            if (!string.IsNullOrEmpty(roleName))
            {
                rtn = claims.Where(c => c.Type == "role" && roles.Contains(c.Value)).Count() > 0;
            }

            return rtn;
        }

        /// <summary>
        /// Updates a claim with a new value
        /// </summary>
        /// <param name="claims">The claims.</param>
        /// <param name="claimName">Name of the claim.</param>
        /// <param name="newValue">The new value.</param>
        public static void updateClaim(List<Claim> claims, string claimName, string newValue)
        {
            Claim claim = getClaim(claims,claimName);
            if (claim != null)
                claims.Remove(claim);

            claims.Add(new Claim(claimName, newValue));
        }
    }
}
