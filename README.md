# dotnetbknd
Backand .NET SDK

# Backand

<table>
<tbody>
<tr>
<td><a href="#beckandexception">BeckandException</a></td>
<td><a href="#gellistresult">GetListResult&ltT&gt</a></td>
</tr>
<tr>
<td><a href="#sdk">Sdk</a></td>
<td><a href="#signinresult">SignInResult</a></td>
</tr>
</tbody>
</table>

## Sdk

This SDK enables you to communicate comfortably and quickly with your Backand app.

### Delete(name, id, deep, parameters)

Delete an existing row in the database

| Name | Description |
| ---- | ----------- |
| name | *System.String*<br>The name of the object |
| id | *System.String*<br>The row id to delete |
| deep | *System.Nullable{System.Boolean}*<br>Delete the object descendents |
| parameters | *System.String*<br>Parameters for triggered actions |

### GelList\<T\>(name, totalRows, pageNumber, pageSize, filter, sort, search, deep)

Get a list of objects.

#### Type Parameters

- T - The object type

| Name | Description |
| ---- | ----------- |
| name | *System.String*<br>The name of the object |
| totalRows | *System.Nullable{System.Int32}@*<br>Returns the total rows in the database regardless the filter or page. Use it for paging |
| pageNumber | *System.Nullable{System.Int32}*<br>Page number start from one |
| pageSize | *System.Nullable{System.Int32}*<br>Number of returned objects in page |
| filter | *System.Object*<br>filter the list either with NoSql: {"q":{"age":{"$lt":25}}} or [{fieldName:"age", operator:"lessThan", value:25}] |
| sort | *System.Object*<br>sort the list with [{fieldName:"age", order:"asc"},{fieldName:"height", order:"desc"}] |
| search | *System.String*<br>critaria to search in all textual fields |
| deep | *System.Nullable{System.Boolean}*<br>When set to true it gets all the parent objects under a relatedObjects node |

#### Returns



### GetOne\<T\>(name, id, deep, level)

Get a single object from Backand

#### Type Parameters

- T - The object type

| Name | Description |
| ---- | ----------- |
| name | *System.String*<br>The name of the object |
| id | *System.String*<br>The primary key value of the compatible row |
| deep | *System.Nullable{System.Boolean}*<br>Get all the direct descendents of the row through child relations |
| level | *System.Nullable{System.Int32}*<br>The descendent deep level, default 3 |

#### Returns

A typed object

### Post\<T\>(name, data, id, deep, returnObject, parameters)

Create a new row in the database

#### Type Parameters

- T - The object type

| Name | Description |
| ---- | ----------- |
| name | *System.String*<br>The name of the object |
| data | *\`\`0*<br>The object to create |
| id | *System.String@*<br>Out the created row id |
| deep | *System.Nullable{System.Boolean}*<br>Create also direct descendents of the object through child relations |
| returnObject | *System.Nullable{System.Boolean}*<br>If set to true it returns the object. Use it if you have triggered actions that modify the created object. |
| parameters | *System.String*<br>Parameters for triggered actions |

#### Returns

A typed object

### Put\<T\>(name, id, data, deep, returnObject, parameters, overwrite)

Update an existing row in the database

#### Type Parameters

- T - The object type

| Name | Description |
| ---- | ----------- |
| name | *System.String*<br>The name of the object |
| id | *System.String*<br>The row id to update |
| data | *System.Object*<br>The object to update |
| deep | *System.Nullable{System.Boolean}*<br>Updates and crteates also direct descendents of the object through child relations |
| returnObject | *System.Nullable{System.Boolean}*<br>If set to true it returns the object. Use it if you have triggered actions that modify the created object |
| parameters | *System.String*<br>Parameters for triggered actions |
| overwrite | *System.Nullable{System.Boolean}*<br>If deep it will also delete descendent that do not exist in the collections |

#### Returns

A typed object

### SetOAuth2Token(accessToken)

When using Backand security, after a user signs in, he gets an OAuth2 access token that identifies him and you can use it to connect to Backand.

| Name | Description |
| ---- | ----------- |
| accessToken | *System.String*<br>Use the Signin to get access token. You can put it in a cockie and reuse it to initiate Backand in every request. |

### SignIn(appName, username, password)

This methods returns OAuth2 access token and other user information

| Name | Description |
| ---- | ----------- |
| appName | *System.String*<br>Your Backand app name |
| username | *System.String*<br>The user username |
| password | *System.String*<br>The user password |

#### Returns




## SignInResult

The OAuth2 sign in results.

### access_token

OAuth2 access token

### appName

The app name

### expires_in

Duration in seconds

### fullName

The user full name

### role

The user role

### token_type

OAuth2 access type (bearer)

### userId

The user id in the users object if such exists

### username

The username

## BeckandException

Beckand general exception

### Constructor(response)

Beckand general exception constructor

| Name | Description |
| ---- | ----------- |
| response | *RestSharp.IRestResponse*<br>Extract the status code and message from the Backand http response |

### Constructor(message, statusCode)

Beckand general exception constructor

| Name | Description |
| ---- | ----------- |
| message | *System.String*<br>The error message |
| statusCode | *System.Net.HttpStatusCode*<br>The http status code |

### StatusCode

The http status code


## GelListResult\<T\>

Get list result

#### Type Parameters

- T - 

### data

The list of objects returned

### totalRows

The total rows in the database, regerdless the page or filter. Use this for paging

