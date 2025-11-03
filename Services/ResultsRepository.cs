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
        // DbFile — имя файла базы данных.
        private const string DbFile = "potionpanic.sqlite3";
        // _conn — объект подключения (SQLiteAsyncConnection), через который выполняются запросы
        private SQLiteAsyncConnection? _conn;
        // _initialized — флаг, чтобы не пересоздавать таблицу каждый раз.
        private bool _initialized;

        // Проверяет, есть ли уже соединение. Если нет — создаёт.
        private async Task<SQLiteAsyncConnection> GetConnAsync()
        {
            if (_conn is not null) return _conn;

            // FileSystem.AppDataDirectory — это папка, где MAUI хранит данные
            var path = Path.Combine(FileSystem.AppDataDirectory, DbFile);
            _conn = new SQLiteAsyncConnection(path,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
            return _conn;
        }

        // Проверяет, инициализирована ли база.
        public async Task InitAsync()
        {
            if (_initialized) return;
            // Если нет — вызывает CreateTableAsync<GameResult>(),
            var conn = await GetConnAsync();
            await conn.CreateTableAsync<GameResult>();
            _initialized = true;
        }

        // Убеждается, что база и таблица готовы.
        public async Task<int> AddAsync(GameResult item)
        {
            // Добавляет новую запись (результат игрока).
            await InitAsync();
            var conn = await GetConnAsync();
            return await conn.InsertAsync(item);
        }

        // Берёт таблицу GameResult.
        public async Task<List<GameResult>> GetTopAsync(int count = 20)
        {
            await InitAsync();
            var conn = await GetConnAsync();
            // Сортирует по Score (чем выше — тем раньше).
            // Затем по DateUtc (новые сверху).
            return await conn.Table<GameResult>()
                             .OrderByDescending(r => r.Score)
                             .ThenByDescending(r => r.DateUtc)
                             .Take(count)
                             .ToListAsync();
        }

        // Возвращает все результаты
        public async Task<List<GameResult>> GetAllAsync()
        {
            await InitAsync();
            var conn = await GetConnAsync();
            return await conn.Table<GameResult>()
                             .OrderByDescending(r => r.DateUtc)
                             .ToListAsync();
        }

        // Удаляет конкретную запись по ID.
        public async Task<int> DeleteByIdAsync(int id)
        {
            await InitAsync();
            var conn = await GetConnAsync();
            return await conn.DeleteAsync<GameResult>(id);
        }

        // Полностью очищает таблицу GameResult.
        public async Task<int> DeleteAllAsync()
        {
            await InitAsync();
            var conn = await GetConnAsync();
            return await conn.DeleteAllAsync<GameResult>();
        }
    }
}
