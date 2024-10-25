using sbwpf.Core;
using SQLite;
using System.IO;
using System.Linq.Expressions;

namespace sbsqlite.net.helpers.wpf
{
    public class Datastore<T> where T : class, new()
    {
        ///////////////////////////////////////////////////////////
        #region Fields

        protected string Name = string.Empty;
        protected string DbFolder = string.Empty;
        protected string DbName = string.Empty;
        protected string DbPath = string.Empty;
        protected SQLiteAsyncConnection DB = default;

        #endregion Fields
        ///////////////////////////////////////////////////////////



        ///////////////////////////////////////////////////////////
        #region Interface

        public Datastore(string folder, string name)
        {
            try
            {
                Name = name;
                DbName = $"{name}.db";
                DbFolder = folder;
                DbPath = Path.Combine(DbFolder, $"{name}.db");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public Datastore(string folder, string[]? subfolders, string name)
        {
            try
            {
                Name = name;
                DbName = $"{name}.db";
                DbFolder = folder;
                if (subfolders is not null)
                {
                    foreach (string subfolder in subfolders)
                    {
                        DbFolder = Path.Combine(DbFolder, subfolder);
                    }
                }
                DbPath = Path.Combine(DbFolder, $"{name}.db");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public async void Open()
        {
            try
            {
                IoUtil.EnsureFolder(DbFolder);

                var dboptions = new SQLiteConnectionString(DbPath, true);
                DB = new SQLiteAsyncConnection(dboptions);
                await DB.CreateTableAsync<T>();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public async void Close()
        {
            try
            {
                if (DB is not null)
                {
                    await DB.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public async Task<int> Count()
        {
            try
            {
                return await DB.ExecuteScalarAsync<int>($"select count() from {nameof(T)}");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return 0;
            }
        }

        public async void Add(T item)
        {
            try
            {
                await DB.InsertOrReplaceAsync(item);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public async void AddUnique(T item)
        {
            try
            {
                if (Contains(item).Result is true)
                {
                    return;
                }
                await DB.InsertOrReplaceAsync(item);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public async void AddRange(List<T> list)
        {
            try
            {
                foreach (T item in list)
                {
                    await DB.InsertOrReplaceAsync(item);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public async void AddRangeUnique(List<T> list)
        {
            try
            {
                foreach (T item in list)
                {
                    if (Contains(item).Result is true)
                    {
                        continue;
                    }
                    await DB.InsertOrReplaceAsync(item);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public async void Delete(T item)
        {
            try
            {
                await DB.DeleteAsync(item);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public async void DeleteRange(List<T> list)
        {
            try
            {
                foreach (var item in list)
                {
                    try
                    {
                        await DB.DeleteAsync(item);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public async void Update(T item)
        {
            try
            {
                await DB.UpdateAsync(item);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public async Task<bool> Contains(T t)
        {
            try
            {
                return await DB.Table<T>().Where(tt => tt.Equals(t)).FirstOrDefaultAsync() is not null;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        public async Task<bool> Contains(Expression<Func<T, bool>> lambda)
        {
            try
            {
                return await DB.Table<T>().Where(lambda).FirstOrDefaultAsync() is not null;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        public async Task<T?> FindOne(Expression<Func<T, bool>> lambda)
        {
            try
            {
                return await DB.Table<T>().Where(lambda).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return default;
            }
        }

        public async Task<List<T>?> FindAll(Expression<Func<T, bool>> lambda)
        {
            try
            {
                return await DB.Table<T>().Where(lambda).ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return default;
            }
        }

        #endregion Interface
        ///////////////////////////////////////////////////////////



        ///////////////////////////////////////////////////////////
        #region Internal

        #endregion Internal
        ///////////////////////////////////////////////////////////
    }
}
