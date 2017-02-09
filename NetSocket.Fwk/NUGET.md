# How to generete a nuget package for .NET projects

You have to follow these steps. You can find more accurate information [here](https://docs.microsoft.com/es-es/nuget/quickstart/create-and-publish-a-package):

1. Check if the `nuspec file` already exists in the root folder. In case it doesn't then:

``` sh
nuget spec 
```

2. Open the `nuspec file` and edit what you needed.
3. Now, we're going to create the `package` which is going to be published.

``` sh
nuget pack <name_of_csproj_file> -Prop Configuration=Release
```

4. Finally, before you publish it, it would be nice to check if all the dependencies are being taken into account. You can access the `.nugpk` file using `7zip` or `WinRar` and take a look at the generated `.nuspec`. If you can't see any `<dependencies>` node there, then you'll have to manually generate all the dependencies in your `nuspec` file.