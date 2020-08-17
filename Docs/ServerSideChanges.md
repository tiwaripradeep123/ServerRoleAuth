#How start changes at Server Side.

## Install package: 
[![NuGet package](https://img.shields.io/nuget/v/ServerRoleAuth.svg)](https://www.nuget.org/packages/ServerRoleAuth/)

## Register dependencies
  ```cs
	Container.RegisterType<IRuleEngine, RuleEngine>();

  ```
## Define configuration
[Register Role based configurations](https://github.com/ankitvarmait/ServerRoleAuth/blob/master/Docs/ConfigureJson.md)

## Register with engine options:

 ```cs
 var options = new ConfigurationOptions(configData)
                .WithIgnoreCaseMode(true);
 Container.RegisterType<ISetupConfigurations, DefaultSetupConfigurations>(new InjectionConstructor(options));
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
