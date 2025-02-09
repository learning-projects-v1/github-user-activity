using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using github_user_activity.Models;
namespace github_user_activity;

public class GithubApiService
{
    public readonly HttpClient _httpClient = new HttpClient();
    private string GithubUrl = "https://api.github.com";
    public GithubApiService()
    {
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("C# App");
    }
    public async Task ShowAllRepo(string username)
    {

        var allRepos = await GetAllRepos(username);
        if(allRepos == null || !allRepos.Any()) return;
        foreach (var item in allRepos)
        {
            Console.WriteLine(item);
        }
    }

    public async Task<List<string>> GetAllRepos(string username)
    {
        var endpoint = GithubUrl + $"/users/{username}/repos";
        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        var cache = CacheService.GetUserCache<RepoCache>(username);
        request.Headers.Add("If-None-Match", cache.Etag);
        var (responseBody, etag) = await GetApiResponse(request);
        if(!string.IsNullOrEmpty(responseBody))
        {
            CacheService.SaveAllReposCache(username, responseBody, etag);
            var res = JsonConvert.DeserializeObject<List<Repository>>(responseBody);
            return res.Select(r => r.Name).ToList();
        }
        Console.WriteLine("All repos found in cache");
        return cache.AllRepos.Select(repo => repo.Name).ToList();
    }

    public async Task ShowAllRepoLanguages(string username)
    {
        var repoLanguages = await GetAllRepoLanguages(username);
        var dict = new Dictionary<string, int>();
        foreach(var (repo, languages) in repoLanguages)
        {
            foreach(var (k, v) in languages)
            {
                if (!dict.ContainsKey(k)) dict.Add(k, 0);
                dict[k] += v;
            }
        }

        Console.WriteLine("All Languages used:");
        foreach(var (k,v) in dict)
        {
            Console.WriteLine($"{k} : {v}");
        }
    }
    public async Task<Dictionary<string, Dictionary<string, int>>> GetAllRepoLanguages(string username)
    {
        var endpoint = GithubUrl + $"/repos/{username}/repo/languages";
        var allRepos = await GetAllRepos(username);
        if (allRepos == null || allRepos.Count == 0)
        {
            return new();
        }
       
        var allRepoLanguages = await GetRepoLanguages(username, allRepos);
        return allRepoLanguages;
    }

    private bool IsCacheExpired(LanguagesCache cache)
    {
        var maxTime = 30;
        return cache.LastUpdated.AddMinutes(maxTime) < DateTime.UtcNow;
    }

    private async Task<Dictionary<string, int>> GetRepoLanguagesDummy(string username, string repo)
    {
        var languages = new List<string> { "C#", "C++", "Java", "Scala", "Python", "javascript" };
        var random = new Random();
        var lanDict = new Dictionary<string, int>();
        var idx = 0;
        while (idx < languages.Count) {
            var lineCount = random.Next(1000) + 100;
            lanDict.Add(languages[idx], lineCount);

            var inc = random.Next(5);
            idx += inc + 1;
        }
        return lanDict;
    }
    private async Task<Dictionary<string, Dictionary<string, int>>> GetRepoLanguages(string username, List<string>allRepos)
    {
        var cache = CacheService.GetUserCache<LanguagesCache>(username);
        if (!IsCacheExpired(cache))
        {
            Console.WriteLine("Langugages found on cache");
            return cache.UsedLanguages;
        }
        var cnt = 0;
        var allRepoLanguages = new Dictionary<string, Dictionary<string, int>>();
        foreach (var repo in allRepos)
        {
            try
            {
                var endpoint = GithubUrl + $"/repos/{username}/{repo}/languages";
                var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
                var (responseBody, etag) = await GetApiResponse(request);
                var languages = JsonConvert.DeserializeObject<Dictionary<string, int>>(responseBody);
                allRepoLanguages[repo] = languages;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception occured: " + e);
            }
            //cnt++;
            //if (cnt > 1) break;
        }
        CacheService.SaveAllLanguageCache(username, allRepoLanguages);
        return allRepoLanguages;
    }

    public async Task<(string, string)> GetApiResponse(HttpRequestMessage request)
    {
        try
        {
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            if(response.StatusCode == HttpStatusCode.NotModified)
            {
                return (await response.Content.ReadAsStringAsync(), response.Headers.ETag?.Tag);
            }
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: Unable to fetch data for this endpoint. Status code: {response.StatusCode}");
                return ("", "");
            }
            return (await response.Content.ReadAsStringAsync(), response.Headers.ETag?.Tag);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception occurred: {ex.Message}");
            return ("", "");
        }
    }
}

public static class CacheService
{
    //public static void SaveLangugageCache(string username, string repo, Dictionary<string, int> langugages)
    //{

    //    if (!File.Exists(CachePath))
    //    {
    //        File.WriteAllText(CachePath, "");
    //    }
    //    var rawData = File.ReadAllText(CachePath);
    //    var parsedData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, int>>>>(rawData);
    //    if (parsedData == null) parsedData = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();
    //    if (parsedData.TryGetValue(username, out var userData))
    //    {
    //        userData[repo] = langugages;
    //    }
    //    else
    //    {
    //        var userDataDict = new Dictionary<string, Dictionary<string, int>>();
    //        userDataDict.Add(repo, langugages);
    //        parsedData.Add(username, userDataDict);
    //    }
    //    var serializedString = JsonConvert.SerializeObject(parsedData);
    //    File.WriteAllText(CachePath, serializedString);
    //}
    public static TCache GetUserCache<TCache> (string username) where TCache : ACache, ICachePathProvider, new()
    {
        if (!File.Exists(TCache.CachePath))
        {
            File.WriteAllText(TCache.CachePath, "");
        }
        var rawData = File.ReadAllText(TCache.CachePath);
        var existingCache = JsonConvert.DeserializeObject<Dictionary<string, TCache>>(rawData);
        if (existingCache == null)
        {
            existingCache = new Dictionary<string, TCache>();
        }
        var existingUserCache = existingCache.GetValueOrDefault(username);
        if (existingUserCache == null)
        {
            existingCache.Add(username, new TCache());
        }
        return existingCache[username];
    }
    public static void SaveUserCache<TCache>(string username, TCache cache) where TCache: ACache, ICachePathProvider,new()
    {
        if (!File.Exists(TCache.CachePath))
        {
            File.WriteAllText(TCache.CachePath, "");
        }
        var rawData = File.ReadAllText(TCache.CachePath);
        var existingCache = JsonConvert.DeserializeObject<Dictionary<string, TCache>>(rawData);
        if (existingCache == null)
        {
            existingCache = new Dictionary<string, TCache>();
        }
        existingCache[username] = cache;
        var serializedString = JsonConvert.SerializeObject(existingCache);
        File.WriteAllText(TCache.CachePath, serializedString);
    }
    public static void SaveAllReposCache(string username, string responseBody, string etag)
    {
        var input = JsonConvert.DeserializeObject<List<Repository>>(responseBody);
        var existingUserCache = GetUserCache<RepoCache>(username);
        existingUserCache.AllRepos = input;
        existingUserCache.Etag = etag;
        existingUserCache.LastUpdated = DateTime.UtcNow;
        SaveUserCache(username, existingUserCache);
    }

    public static void SaveAllLanguageCache(string username, Dictionary<string, Dictionary<string, int>> allRepoLanguages, string etag = "")
    {
        var existingUserCache = GetUserCache<LanguagesCache>(username);
        existingUserCache.UsedLanguages = allRepoLanguages;
        existingUserCache.LastUpdated = DateTime.UtcNow;
        SaveUserCache(username, existingUserCache);
    }
}
