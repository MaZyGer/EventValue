# EventValue
EventValue invokes automatically UnityEvent if you change the value. There is EventCallback as well now. Still in beta and any bugs will get fixed very soon.

## Example
```CSharp
public EventValue<int> Health;	

void ApplyDamage(int damage)
{
    Health.Value -= damage;
}
```

![](https://i.imgur.com/jis3CoF.png)


## EventCallback (Experimental)
This is still in progress. This something like Func-Delegate. You can return any value which is matching to your type. 

```CSharp
[SerializeField]
EventCallback<int> getHealthEvent;	

public int Health {get; set;} = 100;
```

Now, you need to assign in the inspector. Properties are shown first with "get_" then the name. So in this example "get_Health";

`var health = getHealthEvent.Invoke()`

So instead of coding that you need to attach Player then use Player.Health you can just code the type you need (int).

### Why you should use EventCallback?
Very often I wanted avoid coding. I just want to prepare delegates and reuse them for different stuff.

For instance. Lets say you have Player, Monster. Those are only using MonoBehaviour. Both have property or GetMethod "Health". In real case you would need to code differently. So `Player player` or `Monster monster`. But with EventCallback you just say what you want get actually. This is reusable because you can reassign over the inspector. No recoding needed.