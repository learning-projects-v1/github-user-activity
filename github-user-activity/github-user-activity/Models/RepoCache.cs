using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace github_user_activity.Models
{
    public interface ICachePathProvider
    {
        public static abstract string CachePath { get; }
    }
    public abstract class ACache
    {
        public DateTime LastUpdated { get; set; }
        public string Etag { get; set; }
    }
    // need a seperate eEtag for UsedLanguages
    public class RepoCache : ACache, ICachePathProvider
    {
        public List<Repository> AllRepos { get; set; } = new();

        public static string CachePath => "RepoCache.json";
    }

    public class LanguagesCache : ACache, ICachePathProvider
    {
        public Dictionary<string, Dictionary<string, int>> UsedLanguages { get; set; } = new();
        public static string CachePath => "LangugageCache.json";
    }
}
