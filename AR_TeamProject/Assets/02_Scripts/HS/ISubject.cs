public interface ISubject
{
    // 구독 등록
    void ResisterStaticMapObserver(IStaticMapObserver observer);
    void ResisterDirectionMapObserver(IDirectionMapObserver observer);

    // 구독 해지
    void RemoveStaticMapObserver(IStaticMapObserver observer);
    void RemoveDirectionMapObserver(IDirectionMapObserver observer);

    // 내용 전달
    void NotifyStaticMapObservers();
    void NotifyDirectionMapObservers();
}

