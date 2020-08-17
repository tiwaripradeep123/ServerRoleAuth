# How To configure Role based configurations

### schema
```json
[
 {
   "Roles": [ "" ],
    "SupportedActions": [
      {
        "Request": "",
        "RequestSubType": ""
      }
    ]
 }
]
````
* Request : RequestName or api name or public method name defined at server
* RequestSubType: Optional: An additional parameter to consider auth handling.
* Use * to define condition for all.
E.g.
  * Consider for all roles:  
  
  ```json
     "Roles" : ["*"]
   ```
   * Cosider for all requests:
   ```json
     "Request": "*"
   ```
    * Cosider for all sub requests:
   ```json
     "RequestSubType": "*"
   ```
   
  * Define Role1 should have access to all requests. 
  RequestSubType is an optional param in case we want to control with additional parameter.

  ```json
	{
	    "Roles": [ "Role4" ],
	    "SupportedActions": [
	      {
		"Request": "*",
		"RequestSubType": "*"
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
   
   * Define Role6 is allowed to read only employee data belong to Employer1, but can read and write employee data for Employer2

  ```json
	{
	    "Roles": [ "Role6" ],
	    "SupportedActions": [
	      {
		       "Request": "EmployeeRead",
           "RequestSubType": "Employer1"
	      },
        {
		       "Request": "EmployeeRead",
           "RequestSubType": "Employer2"
	      },
        {
		       "Request": "EmployeeWrite",
           "RequestSubType": "Employer2"
	      }
	    ]
	 }
   ```
