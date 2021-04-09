# EventValue
EventValue invokes automtically UnityEvent if you change the value. There is EventCallback as well now. Still in beta and any bugs will get fixed very soon.

## Example
```CSharp
public EventValue<int> Health;	

void ApplyDamage(int damage)
{
    Health.Value -= damage;
}
```

![](https://i.imgur.com/jis3CoF.png)