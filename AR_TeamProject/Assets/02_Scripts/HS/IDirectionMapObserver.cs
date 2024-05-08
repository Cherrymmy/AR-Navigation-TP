public interface IDirectionMapObserver
{
    // 내용 업데이트
    void UpdateData(float gpslat, float gpslon, float deslat, float deslon, int zoom);
}