using System.Collections.Generic;
using System.ComponentModel;

public class MyObservable<T>
        : INotifyPropertyChanging, INotifyPropertyChanged
{
    public MyObservable(T defaultValue = default(T),
                            IEqualityComparer<T> comparer = null)
    {
        this.value = defaultValue;
        this.comparer = comparer ?? EqualityComparer<T>.Default;
    }

    private T value;
    private IEqualityComparer<T> comparer;
    public T Value
    {
        get { return value; }
        set
        {
            if (!comparer.Equals(this.value, value))
            {
                OnValueChanging();
                this.value = value;
                OnValueChanged();
            }
        }
    }

    public event PropertyChangingEventHandler PropertyChanging;
    protected virtual void OnValueChanging()
    {
        var propertyChanging = PropertyChanging;
        if (propertyChanging != null)
            propertyChanging(this, new PropertyChangingEventArgs("Value"));
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnValueChanged()
    {
        var propertyChanged = PropertyChanged;
        if (propertyChanged != null)
            propertyChanged(this, new PropertyChangedEventArgs("Value"));
    }
}

//example
//var d = new ObservableObject<double>(10);
//d.PropertyChanged += (o, e) =>
//{
//    var obs = (ObservableObject<double>)o;
//Console.WriteLine("Value changed to: {0}", obs.Value);
//};
//Console.WriteLine("Value is: {0}", d.Value);
//d.Value = 20;
//Console.WriteLine("Value is: {0}", d.Value);