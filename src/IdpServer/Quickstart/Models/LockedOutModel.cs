using System;

namespace IdentityServer4.Quickstart.Model
{
    public class LockedOutModel
    {
        public bool IsLocked { get; set; } = false;
        public int Duration { get; set; } = 15;
        public DateTime ExpiredTime { get; set; } = DateTime.UtcNow;
    }
}