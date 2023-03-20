namespace VGDevs
{
    public interface IPersistenceService : IDataService
    {
        T Get<T>(params string[] keys) where T : IPersistentElement;
        T Get<T>(T toOverride, params string[] keys) where T : IPersistentElement;
        void Set<T>(T element, params string[] path) where T : IPersistentElement;
        void Flush();
        void Flush(params string[] path);
    }
    
    public interface IPersistentElement
    {
        string PersistenceID { get; }
        void OnAfterDeserialize();
        void OnBeforeSerialize();
    }
}