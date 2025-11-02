using System.Collections.Generic;
using System.Threading.Tasks;
using PotionPanic.Models;

namespace PotionPanic.Services
{
    public interface IResultsRepository
    {
        Task InitAsync();

        Task<int> AddAsync(GameResult item);

        Task<List<GameResult>> GetTopAsync(int count = 20);
        Task<List<GameResult>> GetAllAsync();

        Task<int> DeleteByIdAsync(int id);
        Task<int> DeleteAllAsync();
    }
}
