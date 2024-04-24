using System.Collections;

namespace Search
{
    public interface IJsonDataManager
    {
        void SaveData<T>(T data, string path);
        T LoadData<T>(string path);
    }

    public interface IJsonSearchService
    {
        IEnumerator SearchPlacesCoroutine(string query);
        IEnumerator FetchPlaceDetails(string url);
    }
}
