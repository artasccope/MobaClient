namespace GameFW.Persistence
{
    public interface IPersistence
    {
        void SaveInt(string name, int value);
        void SaveFloat(string name, float value);
        void SaveString(string name, string value);
        void SaveBoolean(string name, bool value);
        int LoadInt(string name);
        float LoadFloat(string name);
        string LoadString(string name);
        bool LoadBoolean(string name);
    }
}
