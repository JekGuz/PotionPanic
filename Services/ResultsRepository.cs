using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Maui.Storage;   // FileSystem.AppDataDirectory
using SQLite;
using PotionPanic.Models;

namespace PotionPanic.Services
{
    public sealed class ResultsRepository : IResultsRepository
    {
        private const string DbFile = "potionpanic.sqlite3";
        private SQLiteAsyncConnection? _conn;
        private bool _initialized;

        private async Task<SQLiteAsyncConnection> GetConnAsync()
        {
            if (_conn is not null) return _conn;

            var path = Path.Combine(FileSystem.AppDataDirectory, DbFile);
            _conn = new SQLiteAsyncConnection(path,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
            return _conn;
        }

        public async Task InitAsync()
        {
            if (_initialized) return;
            var conn = await GetConnAsync();
            await conn.CreateTableAsync<GameResult>();
            _initialized = true;
        }

        public async Task<int> AddAsync(GameResult item)
        {
            await InitAsync();
            var conn = await GetConnAsync();
            return await conn.InsertAsync(item);
        }

        public async Task<List<GameResult>> GetTopAsync(int count = 20)
        {
            await InitAsync();
            var conn = await GetConnAsync();
            return await conn.Table<GameResult>()
                             .OrderByDescending(r => r.Score)
                             .ThenByDescending(r => r.DateUtc)
                             .Take(count)
                             .ToListAsync();
        }

        public async Task<List<GameResult>> GetAllAsync()
        {
            await InitAsync();
            var conn = await GetConnAsync();
            return await conn.Table<GameResult>()
                             .OrderByDescending(r => r.DateUtc)
                             .ToListAsync();
        }

        public async Task<int> DeleteByIdAsync(int id)
        {
            await InitAsync();
            var conn = await GetConnAsync();
            return await conn.DeleteAsync<GameResult>(id);
        }

        public async Task<int> DeleteAllAsync()
        {
            await InitAsync();
            var conn = await GetConnAsync();
            return await conn.DeleteAllAsync<GameResult>();
        }
    }
}
