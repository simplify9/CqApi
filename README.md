| **Package**       | **Version** |
| :----------------:|:----------------------:|
| [`SimplyWorks.CqAPI`](https://www.nuget.org/packages/SimplyWorks.CqApi/)|![Nuget](https://img.shields.io/nuget/v/SimplyWorks.CqApi?style=for-the-badge)|

![License](https://img.shields.io/badge/license-MIT-blue.svg)

_CqApi_ is a library written with the goal of reducing boilerplate code in mind, making building APIs and endpoints a quicker, more efficient process. 

_CqApi_ takes handlers charged with performing specific tasks, attached to a certain interface, and it automatically uses the folder structure to determine the route so with this, you longer have to build controllers. All that's left for the user to do is to build the resource folders appropriately.

### _CqApi_'s folder structure 
_CqApi_ is built in such a way that it allows for users to declare routes through an intuitive design:

```
Resources:
-{routePrefix}:
 -THE HANDLERS
 ```

_CqApi_ automatically uses the folder structure to determine the route of the handlers. With this, you no longer have to build controllers. All there is to do is build the resource folders appropriately, allowing _CqApi_ to study the route and build a controller based on its structure. 

Through this design, all the boilerplate of HTTP methods is handled in the background. 

## Setting up _CqApi_


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
 
