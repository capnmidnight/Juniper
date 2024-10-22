namespace Microsoft.AspNetCore.Identity;

public static class IdentityResultExtensions
{
    public static void Check(this IdentityResult result)
    {
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToArray().Join(", ");
            throw new Exception(errors);
        }
    }
}
