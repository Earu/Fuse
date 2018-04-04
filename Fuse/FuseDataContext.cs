using Fuse.Datas;
using System.Data.Entity;

namespace Fuse
{
    internal class FuseDataContext : DbContext
    {
        internal DbSet<UserData>       Users       { get; set; }
        internal DbSet<DiscussionData> Discussions { get; set; }
        internal DbSet<MessageData>    Messages    { get; set; }
        internal DbSet<SettingsData>   Settings    { get; set; }

        internal static FuseDataContext _Context = new FuseDataContext();

        internal static FuseDataContext GetDataContext()
        {
            return _Context;
        }
    }
}