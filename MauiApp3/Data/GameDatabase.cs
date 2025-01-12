using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiApp3
{
    public class Game
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Date { get; set; }
        public string Players { get; set; }
        public string DrinksPerPlayer { get; set; } // JSON format
    }

    public class GameDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public GameDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Game>().Wait();
        }

        // Method to save the game automatically
        public Task<int> SaveGameAsync(Game game)
        {
            return _database.InsertAsync(game);
        }

        // Get saves games
        public Task<List<Game>> GetSavedGamesAsync()
        {
            return _database.Table<Game>().ToListAsync();
        }
    }
}