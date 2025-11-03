using System.Collections.Generic;
using System.Threading.Tasks;
using PotionPanic.Models;

namespace PotionPanic.Services
{
    public interface IResultsRepository
    {
        Task InitAsync();

        // добавить результат
        Task<int> AddAsync(GameResult item);

        Task<List<GameResult>> GetTopAsync(int count = 20);

        // получить все результаты
        Task<List<GameResult>> GetAllAsync();

        // очистить базу
        Task<int> DeleteByIdAsync(int id);
        Task<int> DeleteAllAsync();
    }
}
