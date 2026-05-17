namespace _.Scripts.Data
{
    public interface IData
    {
        public string Identity { get; }
        public bool   IsSigned { get; }
    }

    public interface IDataPreset {}
    
    public interface IData<T> : IData
    where T : IDataPreset
    {
        public IData<T> Fill(T presetData);
    }
}