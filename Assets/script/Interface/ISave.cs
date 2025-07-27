public interface ISave
{
    public void Save(string key, string data);
    public string Load(string key);
}