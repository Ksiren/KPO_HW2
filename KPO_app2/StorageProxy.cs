namespace KPO_app2
{
    public interface IStorage<T> where T : class
    {
        T GetById(Guid id);
        IEnumerable<T> GetAll();
        void Add(T elem);
        void Update(T elem);
        void Delete(Guid id);
    }

    public class StorageProxy<T> : IStorage<T> where T : class
    {
        private readonly IStorage<T> realStorage;
        private readonly Dictionary<Guid, T> library = new();

        public StorageProxy(IStorage<T> _realStore)
        {
            realStorage = _realStore;
            foreach (var elem in realStorage.GetAll())
            {
                var id = (Guid)typeof(T).GetProperty("id").GetValue(elem);
                library[id] = elem;
            }
        }
        public T GetById(Guid id)
        {
            if (library.ContainsKey(id)) return library[id];

            var elem = realStorage.GetById(id);
            if (elem != null) library[id] = elem;
            return elem;
        }
        public IEnumerable<T> GetAll() => library.Values;
        public void Add(T elem)
        {
            var id = (Guid)typeof(T).GetProperty("id").GetValue(elem);
            realStorage.Add(elem);
            library[id] = elem;
        }
        public void Update(T elem)
        {
            var id = (Guid)typeof(T).GetProperty("id").GetValue(elem);
            realStorage.Update(elem);
            library[id] = elem;
        }
        public void Delete(Guid id)
        {
            realStorage.Delete(id);
            library.Remove(id);
        }
    }



    public class MemoryStorage<T> : IStorage<T> where T : class
    {
        private readonly Dictionary<Guid, T> _storage = new Dictionary<Guid, T>();

        public T GetById(Guid id)
        {
            _storage.TryGetValue(id, out var value);
            return value;
        }

        public IEnumerable<T> GetAll()
        {
            return _storage.Values;
        }

        public void Add(T elem)
        {
            var id = (Guid)typeof(T).GetProperty("id").GetValue(elem);
            _storage[id] = elem;
        }

        public void Update(T elem)
        {
            var id = (Guid)typeof(T).GetProperty("id").GetValue(elem);
            if (_storage.ContainsKey(id))
            {
                _storage[id] = elem;
            }
        }

        public void Delete(Guid id)
        {
            _storage.Remove(id);
        }
    }
}

