#How start changes at Server Side.

## Install package: 
[![NuGet package](https://img.shields.io/nuget/v/ServerRoleAuth.svg)](https://www.nuget.org/packages/ServerRoleAuth/)

## Define configuration
[Register Role based configurations](https://github.com/ankitvarmait/ServerRoleAuth/blob/master/Docs/ConfigureJson.md)

## Register with engine options:

 ```cs
 var options = new ConfigurationOptions(configData)
                .WithIgnoreCaseMode(true);
 RegisterRules.SetupConfiguration(options);ns>(new InjectionConstructor(options));
 ```
 * WithIgnoreCaseMode: Provides capability to configure case incentive evaluation.
 ```cs
 Role1 == role1 if WithIgnoreCaseMode(true).
 ```
 
## Evaluate

  ```cs
	var isRequestValid = ruleEngine.IsAllowed("Role1", "A", "B")
  // OR
	var isRequestValid = ruleEngine.IsAllowed("Role1", "A")
  ```

  ```cs
	var isRequestValid = ruleEngine.IsAllowed("Role1", "A", "B", "Group1")
  // OR
	var isRequestValid = ruleEngine.IsAllowed("Role1", "A", "Group2")
  ```
