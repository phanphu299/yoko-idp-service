using IdpServer.Application.Model;
using IdpServer.Application.User.Model;

namespace IdpServer.Application.Extension
{
    public static class UserNameExtension
    {
        public static string GetDisplayName(this UserDto user)
        {
            if (user == null)
                return null;

            return $"{user.FirstName} {user.LastName}".Trim();
        }

        public static string GetDisplayName(this UserInfoDto userInfo)
        {
            if (userInfo == null)
                return null;
            
            return $"{userInfo.FirstName} {userInfo.LastName}".Trim();
        }

        public static string GetDisplayName(this Domain.Entity.User userEntity)
        {
            if (userEntity == null)
                return null;

            return $"{userEntity.FirstName} {userEntity.LastName}".Trim();
        }
    }
}