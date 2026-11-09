using System;
using System.Collections.Concurrent;

namespace Requestrr.WebApi.RequestrrBot.TvShows
{
    internal static class TvShowInteractionCache
    {
        private const int MaximumEntries = 256;
        private static readonly TimeSpan EntryLifetime = TimeSpan.FromMinutes(5);
        private static readonly ConcurrentDictionary<string, CacheEntry> Entries = new();

        public static string CreateKey(string userId, int categoryId, int tvDbId)
        {
            return $"{userId}/{categoryId}/{tvDbId}";
        }

        public static bool TryGet(string key, out TvShow tvShow)
        {
            if (Entries.TryGetValue(key, out var entry) && entry.ExpiresAt > DateTime.UtcNow)
            {
                tvShow = entry.Value;
                return true;
            }

            Entries.TryRemove(key, out _);
            tvShow = null;
            return false;
        }

        public static void Set(string key, TvShow tvShow)
        {
            if (tvShow == null)
            {
                return;
            }

            Entries[key] = new CacheEntry(tvShow, DateTime.UtcNow.Add(EntryLifetime));

            if (Entries.Count > MaximumEntries)
            {
                foreach (var item in Entries)
                {
                    if (item.Value.ExpiresAt <= DateTime.UtcNow || Entries.Count > MaximumEntries)
                    {
                        Entries.TryRemove(item.Key, out _);
                    }
                }
            }
        }

        private sealed record CacheEntry(TvShow Value, DateTime ExpiresAt);
    }
}
