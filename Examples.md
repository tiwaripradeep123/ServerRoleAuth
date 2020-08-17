# How To use

## Step 1
Create one sample configuration.
Refer: RoleBasedConfigurations.json from src/MS.ServerRoleAuthorization.FunctionalTests

## Step 2
Register following dependency

  ```cs
	Container.RegisterType<IRuleEngine, RuleEngine>();

  ```

## Step 3
Use Like 

  ```cs
	ruleEngine.IsAllowed("Role1", "A", "B")
	ruleEngine.IsAllowed("Role1", "A")
  ```

# How to configure in json

* Define Role1 should have access to all requests. 

  ```json
	{
	    "Roles": [ "Role4" ], /* Multiple roles cab be assigned. */
	    "SupportedActions": [
	      {
		"Request": "*",
		"RequestSubType": "*" /* an optional param in case we want to control with additional parameter */
	      }
	    ]
	 }
   ```
 * Define Role2, Role3 should have access to Employee Read requests. 

  ```json
	{
	    "Roles": [ "Role2", "Role3" ],
	    "SupportedActions": [
	      {
		"Request": "EmployeeRead"
	      }
	    ]
	 }
   ```
 * Define Role4 should have access to Employee Read and write requests. 

  ```json
  
	{
	    "Roles": [ "Role4" ],
	    "SupportedActions": [
	      {
		"Request": "EmployeeRead"
	      },
	      {
		"Request": "EmployeeWrite"
	      }
	    ]
	 }
   ```
 * Define all roles should have access to Product Read requests. 
 
  ```json

	{
	    "Roles": [ "*" ], 
	    "SupportedActions": [
	      {
		"Request": "ProductRead"
	      }
	    ]
	 }
   ```
