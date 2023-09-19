# Object Pooling for Unity

## Difference in this fork

You can set max capacity to a pool. When max capacity reached with all instances active, when you reuse the prefab, it reuses the oldest active instance in the scene with the help of a Queue.

To set max capacity, you need to install the pool via Pool Installer

<img width="344" alt="Screenshot 2023-09-19 at 18 49 37" src="https://github.com/whynames/Object-Pooling-for-Unity/assets/73838734/071661af-f86c-46b5-9f06-88d3e9481afc">
<img width="345" alt="Screenshot 2023-09-19 at 18 49 26" src="https://github.com/whynames/Object-Pooling-for-Unity/assets/73838734/776bc999-32e9-467c-aeb3-b1e489bd62e5">
<img width="345" alt="Screenshot 2023-09-19 at 18 49 12" src="https://github.com/whynames/Object-Pooling-for-Unity/assets/73838734/95c1ec9d-1f8b-4d4a-97a0-6073535fdef6">


- I left some test code inside for future tests and clean ups
- If you don't have odin inspector, just delete warning lines, they don't affect the functionality, just for the inspector.


### TODO
- [x] Rename API to make it more clarifying

## Features
- Faster in terms of performance than Instantiate/Destroy (Test at the end of README)
- Easy to use
- Easy to integrate with already written spawn systems
- Callbacks OnReuse & OnRelease to reset object's state

## How to Install
### Git Installation (Best way to get latest version)

If you have Git on your computer, you can open Package Manager indside Unity, select "Add package from Git url...", and paste link ```https://github.com/IntoTheDev/Object-Pooling-for-Unity.git```

or

Open the manifest.json file of your Unity project.
Add ```"com.intothedev.objectpooling": "https://github.com/IntoTheDev/Object-Pooling-for-Unity.git"```

### Manual Installation (Version can be outdated)
Download latest package from the Release section.
Import ObjectPooling.unitypackage to your Unity Project

## Usage
### How to populate pool:
```csharp
using ToolBox.Pools;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	[SerializeField] private GameObject _prefab = null;
	
	private void Awake()
	{
		_prefab.Populate(count: 50);
	}
}
```

Also, you can just put PoolInstaller component on any object on Scene and select which objects you want to prepopulate

![](https://i.imgur.com/gnyZ0RQ.png)

### How to clear pool and it's instances
```csharp
using ToolBox.Pools;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	[SerializeField] private GameObject _prefab = null;
	
	private void Awake()
	{
		_prefab.Populate(count: 50);
		
		// If destroy active is true then even active instances will be destroyed
		_prefab.Clear(destroyActive: true)
	}
}
```

### How to get object from pool:
```csharp
using ToolBox.Pools;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	[SerializeField] private GameObject _prefab = null;
	
	public void Spawn()
	{
		_prefab.Reuse(transform.position, transform.rotation);
		
		// Get object from pool with component
		_prefab.Reuse<Rigidbody>(transform.position, transform.rotation).isKinematic = true;
	}
}
```

### How to release object:
```csharp
using ToolBox.Pools;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	[SerializeField] private GameObject _prefab = null;
	
	public void Spawn()
	{
		var instance = _prefab.Reuse(transform.position, transform.rotation);
		instance.Release();
	}
}
```

### How to use callbacks:
```csharp
using ToolBox.Pools;
using UnityEngine;

public class Health : MonoBehaviour, IPoolable
{
	[SerializeField] private float _maxHealth = 100f;
	
	private float _health = 0f;
	
	// Awake will be called on first _prefab.Reuse()
	private void Awake() 
	{
		OnReuse();
	}
		
	// IPoolable method
	/// <summary>
	/// This method will be called on 2nd Reuse call.
	/// Use Unity's Awake method for first initialization and this method for others
	/// </summary>
	public void OnReuse()
	{
		_health = _maxHealth;
	}
		
	// IPoolable method
	public void OnRelease() { }
}
```

### Peformance test:
Creating and destroying 1000 objects.

#### Instantiate/Destroy:

```csharp
using Sirenix.OdinInspector;
using System.Diagnostics;
using UnityEngine;

public class Tester : MonoBehaviour
{
	[SerializeField] private GameObject _object = null;
	
	[Button]
	private void Test()
	{
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		
		for (int i = 0; i < 1000; i++)
		{
			var instance = Instantiate(_object);
			Destroy(instance);
		}
		
		stopwatch.Stop();
		print($"Milliseconds: {stopwatch.ElapsedMilliseconds}");
	}
}
```
##### Result: [16:26:15] Milliseconds: 6

#### Get/Release:

```csharp
using Sirenix.OdinInspector;
using System.Diagnostics;
using ToolBox.Pools;
using UnityEngine;

public class Tester : MonoBehaviour
{
	[SerializeField] private GameObject _object = null;
	
	private void Awake()
	{
		_object.Populate(1000);
	}
	
	[Button]
	private void Test()
	{
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		
		for (int i = 0; i < 1000; i++)
		{
			var instance = _object.Reuse();
			instance.Release();
		}
		
		stopwatch.Stop();
		print($"Milliseconds: {stopwatch.ElapsedMilliseconds}");
	}
}
```
##### Result: [16:29:36] Milliseconds: 2
