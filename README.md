| **Package**       | **Version** |
| :----------------:|:----------------------:|
| [`SimplyWorks.CqAPI`](https://www.nuget.org/packages/SimplyWorks.CqApi/)|![Nuget](https://img.shields.io/nuget/v/SimplyWorks.CqApi?style=for-the-badge)|

![License](https://img.shields.io/badge/license-MIT-blue.svg)

_CqApi_ is a library written with the goal of reducing boilerplate code in mind, making building APIs and endpoints a quicker, more efficient process. 

_CqApi_ takes handlers charged with performing specific tasks, attached to a certain interface, and it automatically uses the namespace to determine the route so with this, you longer have to build controllers. All that's left for the developer to do is to have the right namespace for the handler, and this is often handled through the IDE and the folder structure of the files.

### _CqApi_'s folder structure 
_CqApi_ is built in such a way that it allows for users to declare routes through an intuitive design:

```
Resources:
-{routePrefix}:
 -THE HANDLERS
 ```

With this folder structure, the routes in the system will be apparent at a moment's glance with the added of benefit of the namespaces being correct automatically.

## Route Interfaces

_CqApi_ provides the following routes to be injected into the master controller:

- `ICommandHandler`: POST @ /resourceName (No body)

- `ICommandHandler<T>`: POST @ /resourceName (where body is of type T, for deserialization purposes)

- `ICommandHandler<TKey, TBody>`: POST @ /resourceName/key (where key is of type TKey and the body of type TBody)

- `IGetHandler<T>`: GET @ /resourceName/key (where key of type T)

- `IQueryHandler`: GET @ /resourceName

- `IQueryHandler<TRequest>`: GET @ /resourceName?QUERYPARAMS (where query params get deserialized to type TRequest)

- `IQueryHandler<TKey, TRequest>`: GET @ /resourceName/key?QUERYPARAMS (where query params gets deserialized into TRequest, and key to TKey)

- `IDeleteHandler<TKey>`: DELETE @ /resourceName/key 

- `ISearchyHandler`: GET @ /resourceName, where an object formed by the sorts and filters in the query params to make typical search cases convenient.

#### Options to pass _CqApi_

```cpp
public class CqApiOptions
    {
        /// <summary>
        /// The project's name
        /// </summary>
        public string ApplicationName { get; set; }
        /// <summary>
        /// The project's description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The controller prefix, how the API is accessed. 
        /// Do not add a trailing slash.
        /// </summary>
        public string UrlPrefix { get; set; } = "cqapi";
        /// <summary>
        /// Any resource with its key found here, will have its description replaced with the value
        /// </summary>
        public ResourceDescriptions ResourceDescriptions { get; }
        /// <summary>
        /// Authentication settings
        /// </summary>
        public CqApiAuthOptions AuthOptions { get; set; }
        /// <summary>
        /// Maps for types, how they're sent through the API
        /// </summary>
        public TypeMaps Maps { get; }

        /// <summary>
        /// Protect all APIs by default. To unprotect specific APIs, use UnProtect attribute.
        /// </summary>
        public bool ProtectAll { get; set; }

        public string RolePrefix { get; set; } 
 ```


## Getting support 👷
If you encounter any bugs, don't hesitate to submit an [issue](https://github.com/simplify9/CqApi/issues). We'll get back to you promptly! 
 
