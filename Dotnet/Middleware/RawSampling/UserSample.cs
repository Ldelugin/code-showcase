using System.Linq;
using System.Security.Claims;

namespace -- redacted --;

/// <summary>
/// Sample container for the user.
/// </summary>
public class UserSample
{
    /// <summary>
    /// Creates new instance of <see cref="UserSample"/>.
    /// </summary>
    /// <param name="user">
    /// The <see cref="ClaimsPrincipal"/> related to the call.
    /// </param>
    public UserSample(ClaimsPrincipal user)
    {
        if (user == null)
        {
            return;
        }

        var userIdentity = (ClaimsIdentity)user.Identity;
        this.Name = userIdentity.Name;
        this.IsAuthenticated = userIdentity.IsAuthenticated;
    }

    /// <summary>
    /// The name of the user.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///  Whether the user is autenticated or not.
    /// </summary>
    public bool IsAuthenticated { get; set; }
}
